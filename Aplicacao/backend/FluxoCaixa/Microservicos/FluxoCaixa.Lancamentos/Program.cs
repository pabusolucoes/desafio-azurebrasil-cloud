using FluxoCaixa.Lancamentos.Services;
using Microsoft.AspNetCore.Mvc;
using FluxoCaixa.Lancamentos.Models;
using Microsoft.OpenApi.Models;
using FluxoCaixa.Lancamentos.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Registra o ICustomEnvironment para ser injetado em toda aplicação
builder.Services.AddSingleton<ICustomEnvironment, CustomEnvironment>();

// Adiciona serviços do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fluxo de Caixa-Lançamentos API", Version = "v1" });
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fluxo de Caixa-Lançamentos API V1");
        c.RoutePrefix = ""; // Faz com que o Swagger seja a página inicial do serviço
    });
}

List<Lancamento> lancamentos = new();

app.MapPost("/fluxo-caixa/lancamentos", ([FromBody] Lancamento novoLancamento) =>
{
    // Gera um novo ID para o lançamento
    novoLancamento.Id = Guid.NewGuid();
    rabbitMqProducer.Publish(new
    {
        Acao = "CriarLancamento",
        Lancamento = novoLancamento
    });
    return Results.Created($"/fluxo-caixa/lancamentos/{novoLancamento.Id}", novoLancamento);
});

// GET: Lista todos os lançamentos (via RabbitMQ)
app.MapGet("/fluxo-caixa/lancamentos", () => 
{
    try {
        rabbitMqProducer.Publish(new
        {
            Acao = "ConsultarTodos"
        });
        return Results.Accepted("/fluxo-caixa/lancamentos", "Consulta de todos os lançamentos enviada.");
    } catch (Exception ex) {
        return Results.BadRequest(ex.Message);
    }   

});

// GET: Consulta detalhes de um lançamento específico (via RabbitMQ)
app.MapGet("/fluxo-caixa/lancamentos/{id:guid}", (Guid id) =>
{
    rabbitMqProducer.Publish(new
    {
        Acao = "ConsultarPorId",
        Id = id
    });
    return Results.Accepted($"/fluxo-caixa/lancamentos/{id}", $"Consulta do lançamento com ID {id} enviada.");
});

// PUT: Atualiza os dados de um lançamento específico
app.MapPut("/fluxo-caixa/lancamentos/{id:guid}", ([FromBody] Lancamento updateLancamento, Guid id) =>
{
    // Garante que o ID seja consistente
    updateLancamento.Id = id;
    rabbitMqProducer.Publish(new
    {
        Acao = "AtualizarLancamento",
        Lancamento = updateLancamento
    });
    return Results.Accepted($"/fluxo-caixa/lancamentos/{id}",$"Atualização do lançamento com ID {id} enviada.");
});

app.MapDelete("/fluxo-caixa/lancamentos/{id:guid}", (Guid id) =>
{
    rabbitMqProducer.Publish(new
    {
        Acao = "DeletarLancamento",
        Id = id
    });
    return Results.Accepted($"/fluxo-caixa/lancamentos/{id}",$"Remoção do lançamento com ID {id} enviada.");
});

app.Run();
public partial class Program { }
