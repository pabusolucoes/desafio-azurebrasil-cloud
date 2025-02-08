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

app.MapGet("/consolidado-diario", () =>
{
    var consolidado = lancamentos
        .GroupBy(l => l.Data.Date)
        .Select(g => new
        {
            Data = g.Key,
            TotalDebitos = g.Where(l => l.Tipo == "DÉBITO").Sum(l => l.Valor),
            TotalCreditos = g.Where(l => l.Tipo == "CRÉDITO").Sum(l => l.Valor),
            Saldo = g.Sum(l => l.Tipo == "CRÉDITO" ? l.Valor : -l.Valor)
        })
        .ToList();

    return Results.Ok(consolidado);
});

app.MapGet("/consolidado-diario/{data}", (DateTime data) =>
{
    var consolidado = lancamentos
        .Where(l => l.Data.Date == data.Date)
        .GroupBy(l => l.Data.Date)
        .Select(g => new
        {
            Data = g.Key,
            TotalDebitos = g.Where(l => l.Tipo == "DÉBITO").Sum(l => l.Valor),
            TotalCreditos = g.Where(l => l.Tipo == "CRÉDITO").Sum(l => l.Valor),
            Saldo = g.Sum(l => l.Tipo == "CRÉDITO" ? l.Valor : -l.Valor)
        })
        .FirstOrDefault();

    return consolidado is not null ? Results.Ok(consolidado) : Results.NotFound();
});

app.MapPost("/consolidado-diario/reprocessar", () =>
{
    var mensagem = new { Acao = "ReprocessarConsolidado" };
    // Publicar mensagem no RabbitMQ
    rabbitMqProducer.Publish(mensagem);
    return Results.Ok("Reprocessamento iniciado.");
});

app.Run();
public partial class Program { }
