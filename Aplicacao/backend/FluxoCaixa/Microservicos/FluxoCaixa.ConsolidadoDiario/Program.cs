using FluxoCaixa.ConsolidadoDiario.Extensions;
using FluxoCaixa.ConsolidadoDiario.Models;
using FluxoCaixa.ConsolidadoDiario.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Registra o ICustomEnvironment para ser injetado em toda aplicação
builder.Services.AddSingleton<ICustomEnvironment, CustomEnvironment>();

// Adiciona serviços do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fluxo de Caixa-Consolidado Diário API", Version = "v1" });
});

var app = builder.Build();

// Obtém o serviço injetado para usar `IsLocal()`
var env = app.Services.GetRequiredService<ICustomEnvironment>();

var rabbitMqProducer = new RabbitMqProducer(env);

if (env.IsLocal())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fluxo de Caixa-Consolidado Diário API V1");
        c.RoutePrefix = "";  // Deixa o Swagger disponível na raiz "/"
    });
}

List<Lancamento> lancamentos = [];

app.MapPost("/consolidado-diario/reprocessar", () =>
{
    var mensagem = new { Acao = "ReprocessarConsolidado" };
    // Publicar mensagem no RabbitMQ
    rabbitMqProducer.Publish(mensagem);
    return Results.Ok("Reprocessamento iniciado.");
});

app.Run();
public partial class Program { }
