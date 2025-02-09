using FluxoCaixa.Integracoes.Extensions;
using FluxoCaixa.Integracoes.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DynamoDbService>();

// Registra o ICustomEnvironment para ser injetado em toda aplicação
builder.Services.AddSingleton<ICustomEnvironment, CustomEnvironment>();

// Adiciona serviços do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fluxo de Caixa-Integrações API", Version = "v1" });
});

var app = builder.Build();

// Obtém o serviço injetado para usar `IsLocal()`
var env = app.Services.GetRequiredService<ICustomEnvironment>();

// Configura o Swagger no pipeline de middleware
if (env.IsLocal())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fluxo de Caixa-Integrações API V1");
        c.RoutePrefix = "";  // Deixa o Swagger disponível na raiz "/"
    });
}


var rabbitMqConsumer = new RabbitMqConsumer(env);
await Task.Run(() => rabbitMqConsumer.StartConsuming());

// Obtém o serviço de DynamoDB
var dynamoDbService = app.Services.GetRequiredService<DynamoDbService>();

// 🔹 Endpoint para reprocessar consolidado por dia, período ou tudo
app.MapPost("/integracoes/reprocessar", async ([FromQuery] string? dataInicio, [FromQuery] string? dataFim) =>
{
    try
    {
        DateTime? inicio = null, fim = null;

        if (!string.IsNullOrEmpty(dataInicio))
        {
            if (DateTime.TryParseExact(dataInicio, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedInicio))
            {
                inicio = parsedInicio.Date; // 🔹 Garante que seja exatamente 00:00:00
            }
            else
            {
                return Results.BadRequest(new { Erro = "Formato de data inválido. Use 'yyyy-MM-dd'.", ValorRecebido = dataInicio });
            }
        }

        if (!string.IsNullOrEmpty(dataFim))
        {
            if (DateTime.TryParseExact(dataFim, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedFim))
            {
                fim = parsedFim.Date.AddHours(23).AddMinutes(59).AddSeconds(59); // 🔹 Garante que seja até 23:59:59
            }
            else
            {
                return Results.BadRequest(new { Erro = "Formato de data inválido. Use 'yyyy-MM-dd'.", ValorRecebido = dataFim });
            }
        }

        // 🔹 Regra: A data inicial não pode ser maior que a data final
        if (inicio.HasValue && fim.HasValue && inicio > fim)
        {
            return Results.BadRequest(new { Erro = "A data inicial não pode ser maior que a data final.", DataInicio = inicio, DataFim = fim });
        }

        await dynamoDbService.ReprocessarConsolidado(inicio, fim);
        return Results.Ok(new { Mensagem = "Reprocessamento iniciado", Inicio = inicio?.ToString("yyyy-MM-dd HH:mm:ss"), Fim = fim?.ToString("yyyy-MM-dd HH:mm:ss") });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Erro = "Falha ao iniciar o reprocessamento", Detalhes = ex.Message });
    }
});



await app.RunAsync();

record MensagemFila(Guid Id, string Conteudo);
public partial class Program { }
