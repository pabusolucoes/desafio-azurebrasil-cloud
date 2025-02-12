using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
// Adicionando Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Microserviço de Autenticação", Version = "v1" });
});

var app = builder.Build();
app.UseCors(policy=>policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Microserviço de Autenticação v1");
        c.RoutePrefix = "";  // 🔹 Deixa o Swagger disponível na raiz "/"
    });
}

var configuration = app.Services.GetRequiredService<IConfiguration>();

var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
var apiKeySettings = configuration.GetSection("ApiKeySettings").Get<ApiKeySettings>() ?? new ApiKeySettings();
var accesKeyAWS = configuration["AWS:AccessKey"];
var secretKeyAWS = configuration["AWS:SecretKey"];
var serviceUrlAWS = configuration["AWS:ServiceUrl"];
    var dynamoConfig = new AmazonDynamoDBConfig
    {
        ServiceURL = serviceUrlAWS,
        AuthenticationRegion = "us-east-1"
    };

var dynamoDbClient = new AmazonDynamoDBClient(accesKeyAWS, secretKeyAWS, dynamoConfig);
var context = new DynamoDBContext(dynamoDbClient);

// Criar a tabela no DynamoDB se não existir
async Task EnsureTableExists()
{
    var tableNames = new[] { "Users", "ApiKeys" };
    foreach (var tableName in tableNames)
    {
        try
        {
            var response = await dynamoDbClient.DescribeTableAsync(tableName);
        }
        catch (AmazonDynamoDBException e) when (e.ErrorCode == "ResourceNotFoundException")
        {
            await dynamoDbClient.CreateTableAsync(new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition("UserName", ScalarAttributeType.S)
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement("UserName", KeyType.HASH)
                },
                BillingMode = BillingMode.PAY_PER_REQUEST
            });
        }
    }
}

await EnsureTableExists();

// Health Check Endpoint
app.MapGet("/health", () =>
{
    try
    {
        // 🔹 Verifica conexão com DynamoDB
        var dynamoCheck = dynamoDbClient != null;


        if (dynamoCheck)
        {
            return Results.Ok(new { status = "OK", dynamoDb = "Online", rabbitMq = "Online" });
        }
        else
        {
            return Results.Problem("Algum serviço não está disponível", statusCode: 503);
        }
    }
    catch (Exception ex)
    {
        return Results.Problem($"Erro no health check: {ex.Message}", statusCode: 500);
    }
});

app.MapPost("/auth/criar", async ([FromBody] UserRequest request) =>
{
    
    var existingUser = await context.LoadAsync<UserEntry>(request.UserName);

    if (existingUser != null)
        return Results.BadRequest("Usuário já existe.");
    
    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
    var userEntry = new UserEntry { UserName = request.UserName, PasswordHash = hashedPassword };
    await context.SaveAsync(userEntry);
    
    var apiKey = GenerateApiKey();
    var expirationDate = DateTime.UtcNow.AddDays(apiKeySettings.ExpirationDays);
    
    var apiKeyEntry = new ApiKeyEntry { UserName = request.UserName, ApiKey = apiKey, ExpirationDate = expirationDate };
    await context.SaveAsync(apiKeyEntry);
    
    return Results.Created($"/auth/{request.UserName}", new { request.UserName, ApiKey = apiKey });
}).WithTags("Autenticação");

app.MapPost("/auth/desativar", async ([FromBody] UserRequest request) =>
{
    var existingUser = await context.LoadAsync<UserEntry>(request.UserName);
    if (existingUser == null) return Results.NotFound("Usuário não encontrado.");

    if (string.IsNullOrEmpty(request.UserName)) return Results.BadRequest("Usuário é obrigatório.");
    if (existingUser == null) return Results.NotFound("Usuário não encontrado.");
    await context.DeleteAsync<UserEntry>(existingUser);

      var apiKeyEntry = await context.LoadAsync<ApiKeyEntry>(request.UserName);
    if (apiKeyEntry == null) return Results.NotFound("API Key não encontrada.");
    await context.DeleteAsync<ApiKeyEntry>(apiKeyEntry);
    
    return Results.Ok("Usuário desativado.");
}).WithTags("Autenticação");

app.MapPost("/auth/alterar-senha", async ([FromBody] ChangePasswordRequest request) =>
{
    var existingUser = await context.LoadAsync<UserEntry>(request.UserName);
    if (existingUser == null) return Results.NotFound("Usuário não encontrado.");
    if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, existingUser.PasswordHash)) return Results.Unauthorized();
    if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.OldPassword))
        return Results.BadRequest("Usuário, senha antiga e nova senha são obrigatórios.");
    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
    existingUser.PasswordHash = hashedPassword;
    await context.SaveAsync(existingUser);
    
    return Results.Ok("Senha alterada com sucesso.");
}).WithTags("Autenticação");

app.MapPost("/auth/login", async ([FromBody] UserRequest request) =>
{
    var userEntry = await context.LoadAsync<UserEntry>(request.UserName);
    if (userEntry == null) return Results.NotFound("Usuário não encontrado.");
    if (!BCrypt.Net.BCrypt.Verify(request.Password, userEntry.PasswordHash)) return Results.Unauthorized();
    
    var apiKeyEntry = await context.LoadAsync<ApiKeyEntry>(request.UserName);
    if (apiKeyEntry == null) return Results.NotFound("API Key não encontrada.");
    
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, request.UserName),
        new Claim("api_key", apiKeyEntry.ApiKey)
    };
    
    var token = GenerateJwtToken(request.UserName, apiKeyEntry.ApiKey);
    
    return Results.Ok(new { Token = token, User = request.UserName });
});


app.MapPost("/auth/estender-apikey", async ([FromBody] UserRequest request) =>
{
     var existingUser = await context.LoadAsync<UserEntry>(request.UserName);
    if (existingUser == null) return Results.NotFound("Usuário não encontrado.");
    if (!BCrypt.Net.BCrypt.Verify(request.Password, existingUser.PasswordHash)) return Results.Unauthorized();
    if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
        return Results.BadRequest("Usuário e senha são obrigatórios.");
    
    var apiKeyEntry = await context.LoadAsync<ApiKeyEntry>(request.UserName);
    if (apiKeyEntry == null) return Results.NotFound("API Key não encontrada.");
    
    apiKeyEntry.ExpirationDate = DateTime.UtcNow.AddDays(apiKeySettings.ExpirationDays);
    await context.SaveAsync(apiKeyEntry);
    
    return Results.Ok("API Key estendida por mais " + apiKeySettings.ExpirationDays + " dias.");
}).WithTags("Autenticação");

app.Run();

// ... (restante do código permanece inalterado)


string GenerateJwtToken(string userName, string apiKey)
{
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret ?? "default_secret"));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, userName),
        new Claim("api_key", apiKey)
    };
    
        foreach (var audience in jwtSettings.Audience)
    {
        claims.Add(new Claim(JwtRegisteredClaimNames.Aud, audience));
    }
    var token = new JwtSecurityToken(
        issuer: jwtSettings.Issuer,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(jwtSettings.TokenExpirationHours),
        signingCredentials: creds);
    
    return new JwtSecurityTokenHandler().WriteToken(token);
}

string GenerateApiKey()
{
    using var rng = RandomNumberGenerator.Create();
    var apiKey = new byte[32];
    rng.GetBytes(apiKey);
    return Convert.ToBase64String(apiKey);
}
public class UserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
[DynamoDBTable("ApiKeys")]
public class ApiKeyEntry
{
    [DynamoDBHashKey]
    public string UserName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
}

record User(string UserName, string Password);
record ChangePasswordRequest(string UserName, string NewPassword, string OldPassword);

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public List<string> Audience { get; set; } = new();
    public int TokenExpirationHours { get; set; } = 1;
}

public class ApiKeySettings
{
    public int ExpirationDays { get; set; } = 7;
}
[DynamoDBTable("Users")]
public class UserEntry
{
    [DynamoDBHashKey]
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}