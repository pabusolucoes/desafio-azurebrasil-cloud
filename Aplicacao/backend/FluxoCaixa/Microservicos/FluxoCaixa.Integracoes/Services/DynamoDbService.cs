using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using FluxoCaixa.Integracoes.Extensions;
using FluxoCaixa.Integracoes.Models;
using FluxoCaixa.Integracoes.Shared; // 🔹 Importação do JsonLogger

namespace FluxoCaixa.Integracoes.Services;

public class DynamoDbService:IDynamoDbService
{
    private readonly AmazonDynamoDBClient _client;
    private readonly DynamoDBContext _context;
    private const string LancamentosTableName = "Lancamentos";
    private const string ConsolidadosTableName = "ConsolidadosDiarios";
    private readonly ICustomEnvironment _env;

    public DynamoDbService(ICustomEnvironment env)
    {
        _env = env;

        var dynamoConfig = new AmazonDynamoDBConfig
        {
            ServiceURL = _env.IsLocal() ? "http://localhost:8000" : "http://dynamodb:8000",
            AuthenticationRegion = "us-east-1"
        };

        _client = new AmazonDynamoDBClient("fakeMyKeyId", "fakeSecretAccessKey", dynamoConfig);
        _context = new DynamoDBContext(_client);

        JsonLogger.Log("INFO", "DynamoDbService inicializado", new { Ambiente = _env.IsLocal() ? "Local" : "Produção" });
    }

    public async Task CriarTabelasSeNaoExistirem()
    {
        try
        {
            var tables = await _client.ListTablesAsync();

            // Criar a tabela de Lançamentos se não existir
            if (!tables.TableNames.Contains(LancamentosTableName))
            {
                var request = new CreateTableRequest
                {
                    TableName = LancamentosTableName,
                    AttributeDefinitions =
                    [
                        new() { AttributeName = "ContaId", AttributeType = "S" },
                        new() { AttributeName = "LancamentoId", AttributeType = "S" },
                        new() { AttributeName = "Data", AttributeType = "S" }
                    ],
                    KeySchema =
                    [
                        new() { AttributeName = "ContaId", KeyType = "HASH" },
                        new() { AttributeName = "LancamentoId", KeyType = "RANGE" }
                    ],
                    GlobalSecondaryIndexes =
                    [
                        new() {
                            IndexName = "DataIndex",
                            KeySchema =
                            [
                                new() { AttributeName = "ContaId", KeyType = "HASH" },
                                new() { AttributeName = "Data", KeyType = "RANGE" }
                            ],
                            Projection = new Projection { ProjectionType = "ALL" },
                            ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 5, WriteCapacityUnits = 5 }
                        }
                    ],
                    ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 5, WriteCapacityUnits = 5 }
                };

                await _client.CreateTableAsync(request);
                JsonLogger.Log("INFO", "Tabela de Lançamentos criada com sucesso");
            }

            // Criar a tabela de Consolidados se não existir
            if (!tables.TableNames.Contains(ConsolidadosTableName))
            {
                var request = new CreateTableRequest
                {
                    TableName = ConsolidadosTableName,
                    AttributeDefinitions =
                    [
                        new() { AttributeName = "ContaId", AttributeType = "S" },
                        new() { AttributeName = "Data", AttributeType = "S" }
                    ],
                    KeySchema =
                    [
                        new() { AttributeName = "ContaId", KeyType = "HASH" },
                        new() { AttributeName = "Data", KeyType = "RANGE" }
                    ],
                    ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 5, WriteCapacityUnits = 5 }
                };

                await _client.CreateTableAsync(request);
                JsonLogger.Log("INFO", "Tabela de Consolidados criada com sucesso");
            }
        }
        catch (Exception ex)
        {
            JsonLogger.Log("ERROR", "Erro ao criar tabelas no DynamoDB", new { Erro = ex.Message });
            throw;
        }
    }

    public async Task SalvarLancamento(Lancamento lancamento)
    {
        try
        {
            lancamento.Data = lancamento.Data.ToUniversalTime().Date; 
            await _context.SaveAsync(lancamento);
            JsonLogger.Log("INFO", "Lançamento salvo com sucesso", lancamento);
        }
        catch (Exception ex)
        {
            JsonLogger.Log("ERROR", "Erro ao salvar lançamento", new { LancamentoId = lancamento.LancamentoId, Erro = ex.Message });
            throw;
        }
    }

    public async Task<List<Lancamento>> ObterLancamentosPorConta(string contaId)
    {
        try
        {
            var conditions = new List<ScanCondition> { new("ContaId", ScanOperator.Equal, contaId) };
            var lancamentos = await _context.ScanAsync<Lancamento>(conditions).GetRemainingAsync();

            JsonLogger.Log("INFO", "Consulta de lançamentos por conta realizada", new { ContaId = contaId, Quantidade = lancamentos.Count });

            return lancamentos;
        }
        catch (Exception ex)
        {
            JsonLogger.Log("ERROR", "Erro ao buscar lançamentos por conta", new { ContaId = contaId, Erro = ex.Message });
            throw;
        }
    }

    public async Task DeletarLancamento(string contaId, Guid lancamentoId)
    {
        try
        {
            await _context.DeleteAsync<Lancamento>(contaId, lancamentoId);
            JsonLogger.Log("INFO", "Lançamento removido do DynamoDB", new { ContaId = contaId, LancamentoId = lancamentoId });
        }
        catch (Exception ex)
        {
            JsonLogger.Log("ERROR", "Erro ao remover lançamento", new { ContaId = contaId, LancamentoId = lancamentoId, Erro = ex.Message });
            throw;
        }
    }

public async Task ReprocessarConsolidado(DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        try
        {
            JsonLogger.Log("INFO", "Iniciando reprocessamento do consolidado...", new { DataInicio = dataInicio, DataFim = dataFim });

            var lancamentos = await _context.ScanAsync<Lancamento>(new List<ScanCondition>()).GetRemainingAsync();

            if (lancamentos == null || lancamentos.Count == 0)
            {
                JsonLogger.Log("WARN", "Nenhum lançamento encontrado para reprocessamento.");
                return;
            }

            // 🔹 Filtrando por timestamp
            if (dataInicio.HasValue)
            {
                lancamentos = lancamentos.Where(l => l.Data >= dataInicio.Value).ToList();
            }
            if (dataFim.HasValue)
            {
                lancamentos = lancamentos.Where(l => l.Data <= dataFim.Value).ToList();
            }

            var consolidado = lancamentos
                .Select(l => new 
                {
                    ContaId = l.ContaId,
                    Data = l.Data.ToUniversalTime().Date, // 🔹 Garante que a data esteja em UTC SEM horário
                    Tipo = l.Tipo,
                    Valor = l.Valor
                })
                .GroupBy(l => new { l.Data, l.ContaId }) // 🔹 Agora agrupa corretamente
                .Select(g => new ConsolidadoDiario
                {
                    ContaId = g.Key.ContaId,
                    Data = g.Key.Data, // 🔹 Mantém a data corretamente sem converter
                    TotalDebitos = g.Where(l => l.Tipo == "DÉBITO").Sum(l => l.Valor),
                    TotalCreditos = g.Where(l => l.Tipo == "CRÉDITO").Sum(l => l.Valor),
                    Saldo = g.Sum(l => l.Tipo == "CRÉDITO" ? l.Valor : -l.Valor)
                })
                .ToList();


            foreach (var item in consolidado)
            {
                await _context.SaveAsync(item);
            }

            JsonLogger.Log("INFO", "Reprocessamento concluído com sucesso!", new { Quantidade = consolidado.Count, DataInicio = dataInicio, DataFim = dataFim });
        }
        catch (Exception ex)
        {
            JsonLogger.Log("ERROR", "Erro ao reprocessar consolidado", new { Erro = ex.Message });
            throw;
        }
    }


}
