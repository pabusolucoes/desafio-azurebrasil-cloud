using FluxoCaixa.Integracoes.Extensions;
using FluxoCaixa.Integracoes.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

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

List<MensagemFila> mensagens = new();

await Task.Run(() => rabbitMqConsumer.StartConsuming());

app.MapGet("/integracoes/mensagens", () => Results.Ok(mensagens));

app.MapPost("/integracoes/processar", ([FromBody] MensagemFila mensagem) =>
{
    var novaMensagem = new MensagemFila(Guid.NewGuid(), mensagem.Conteudo);
    mensagens.Add(mensagem);
    return Results.Accepted($"/integracoes/mensagens/{mensagem.Id}", mensagem);
});
app.MapGet("/integracoes/lancamentos/{contaId}", async (string contaId, DynamoDbService dynamoDbService) =>
{
    var lancamentos = await dynamoDbService.ObterLancamentosPorConta(contaId);
    
    if (lancamentos == null || lancamentos.Count == 0)
        return Results.NotFound($"Nenhum lançamento encontrado para a conta '{contaId}'.");

    return Results.Ok(lancamentos);
});

await app.RunAsync();

record MensagemFila(Guid Id, string Conteudo);
public partial class Program { }
