using FluxoCaixa.ConsolidadoDiario.Extensions;
using FluxoCaixa.ConsolidadoDiario.Models;
using FluxoCaixa.ConsolidadoDiario.Services;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
// 🔹 Registra serviços para injeção de dependência
builder.Services.AddSingleton<IDynamoDbService, DynamoDbService>();
builder.Services.AddSingleton<ICustomEnvironment, CustomEnvironment>();

// 🔹 Registra a fábrica de conexões do RabbitMQ considerando o ambiente
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var env = sp.GetRequiredService<ICustomEnvironment>();

    return new ConnectionFactory
    {
        HostName = env.IsLocal() ? "localhost" : "rabbitmq",
        UserName = "admin",
        Password = "admin"
    };
});

// 🔹 Registra o produtor do RabbitMQ
builder.Services.AddSingleton<IRabbitMqProducer, RabbitMqProducer>();

// 🔹 Adiciona serviços do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fluxo de Caixa-Consolidado Diário API", Version = "v1" });
});

var app = builder.Build();
app.UseCors(policy=>policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// 🔹 Obtém o serviço injetado para usar `IsLocal()`
var env = app.Services.GetRequiredService<ICustomEnvironment>();

if (env.IsLocal() || env.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fluxo de Caixa-Consolidado Diário API V1");
        c.RoutePrefix = "";  // 🔹 Deixa o Swagger disponível na raiz "/"
    });
}

List<Lancamento> lancamentos = [];

// 🔹 Endpoint para Buscar Consolidado por Período
app.MapGet("/consolidado-diario", async (DateTime dataInicial, DateTime dataFinal, string contaId, IDynamoDbService dynamoDbService) =>
{
    try
    {
        var consolidados = await dynamoDbService.ObterConsolidadoPorPeriodo(dataInicial, dataFinal, contaId);
        return Results.Ok(consolidados);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// 🔹 Endpoint de Reprocessamento do Consolidado Diário
app.MapPost("/consolidado-diario/reprocessar", (IRabbitMqProducer rabbitMqProducer) =>
{
    var mensagem = new { Acao = "ReprocessarConsolidado" };

    // 🔹 Publicar mensagem no RabbitMQ
    rabbitMqProducer.Publish(mensagem);
    
    return Results.Ok("Reprocessamento iniciado.");
});

app.Run();
public partial class Program { }
