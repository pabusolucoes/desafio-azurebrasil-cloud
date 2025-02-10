using FluxoCaixa.Lancamentos.Services;
using Microsoft.AspNetCore.Mvc;
using FluxoCaixa.Lancamentos.Models;
using Microsoft.OpenApi.Models;
using FluxoCaixa.Lancamentos.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Registra o ICustomEnvironment para ser injetado em toda aplicação
builder.Services.AddSingleton<ICustomEnvironment, CustomEnvironment>();
builder.Services.AddSingleton<IDynamoDbService,DynamoDbService>();
builder.Services.AddSingleton<IRabbitMqProducer, RabbitMqProducer>();

// Adiciona serviços do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fluxo de Caixa-Lançamentos API", Version = "v1" });
});

var app = builder.Build();

// Obtém o serviço injetado para usar `IsLocal()`
var env = app.Services.GetRequiredService<ICustomEnvironment>();

//var rabbitMqProducer = new RabbitMqProducer(env);

if (env.IsLocal() || env.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fluxo de Caixa-Lançamentos API V1");
        c.RoutePrefix = ""; // Faz com que o Swagger seja a página inicial do serviço
    });
}

List<Lancamento> lancamentos = [];

// POST: Cria um novo lançamento (via RabbitMQ)
app.MapPost("/fluxo-caixa/lancamentos", ([FromBody] Lancamento request, IRabbitMqProducer rabbitMqProducer) =>
{
    var novoLancamento = new Lancamento(contaId:request.ContaId, descricao:request.Descricao, tipo:request.Tipo, 
                                        categoria:request.Categoria, data:request.Data, valor:request.Valor );
    // Gera um novo ID para o lançamento
    rabbitMqProducer.Publish(new
    {
        Acao = "CriarLancamento",
        Lancamento = novoLancamento
    });
    return Results.Ok(novoLancamento);
});

// GET: Lista todos os lançamentos de uma conta (via DynamoDB)
app.MapGet("/fluxo-caixa/lancamentos/{contaId}", async (string contaId, IDynamoDbService dynamoDbService) =>
{
    try
    {
        var lancamentos = await dynamoDbService.ObterLancamentosPorConta(contaId);
        return Results.Ok(lancamentos);
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

// GET: Consulta detalhes de um lançamento específico de uma conta (via DynamoDB)
app.MapGet("/fluxo-caixa/lancamentos/{contaId}/{lancamentoId:guid}", async (string contaId, Guid lancamentoId, IDynamoDbService dynamoDbService) =>
{
    try
    {
        var lancamento = await dynamoDbService.ObterLancamentoPorId(contaId, lancamentoId);
        if (lancamento == null)
            return Results.NotFound($"Nenhum lançamento encontrado com o ID '{lancamentoId}' para a conta '{contaId}'.");
        
        return Results.Ok(lancamento);
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

// PUT: Atualiza os dados de um lançamento específico
app.MapPut("/fluxo-caixa/lancamentos", ([FromBody] Lancamento updateLancamento, Guid lancamentoId, IRabbitMqProducer rabbitMqProducer) =>
{
    // Garante que o ID seja consistente
    updateLancamento.LancamentoId = lancamentoId;
    rabbitMqProducer.Publish(new
    {
        Acao = "AtualizarLancamento",
        Lancamento = updateLancamento
    });
    return Results.Ok($"Atualização do lançamento com ID {lancamentoId} enviada.");
});

// DELETE: Remove um lançamento específico
app.MapDelete("/fluxo-caixa/lancamentos/{contaId}/{lancamentoId:guid}", (string contaId, Guid lancamentoId, IRabbitMqProducer rabbitMqProducer) =>
{
    var removerLancamento = new Lancamento();
    removerLancamento.LancamentoId= lancamentoId;
    removerLancamento.ContaId = contaId;
    rabbitMqProducer.Publish(new
    {
        Acao = "DeletarLancamento",
        Lancamento =removerLancamento
    });
    return Results.Ok($"Remoção do lançamento com ID {lancamentoId} enviada.");
});

app.Run();
public partial class Program { }

