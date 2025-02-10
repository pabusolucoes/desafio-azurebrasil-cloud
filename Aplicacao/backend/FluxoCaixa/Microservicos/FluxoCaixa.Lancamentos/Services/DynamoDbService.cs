using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using FluxoCaixa.Lancamentos.Extensions;
using FluxoCaixa.Lancamentos.Models;
using FluxoCaixa.Lancamentos.Shared;

namespace FluxoCaixa.Lancamentos.Services;

public class DynamoDbService:IDynamoDbService
{
    private readonly AmazonDynamoDBClient _client;
    private readonly DynamoDBContext _context;
    private readonly ICustomEnvironment _env;
    private const string LancamentosTableName = "Lancamentos";

    public DynamoDbService(ICustomEnvironment env, DynamoDBContext? context = null)
    {
        _env = env;

        // Configuração do cliente DynamoDB
        if (_env.IsLocal())
        {
            var dynamoConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000",
                AuthenticationRegion = "us-east-1"
            };

            _client = new AmazonDynamoDBClient("fakeMyKeyId", "fakeSecretAccessKey", dynamoConfig);
        }
        else
        {
            var dynamoConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://dynamodb:8000",
                AuthenticationRegion = "us-east-1"
            };

            _client = new AmazonDynamoDBClient("fakeMyKeyId", "fakeSecretAccessKey", dynamoConfig);
        }

        _context = context ?? new DynamoDBContext(_client);

        // 🔹 Log de inicialização do serviço
        JsonLogger.Log("INFO", "DynamoDbService inicializado", new { Ambiente = _env.IsLocal() ? "Local" : "Produção" });
    }

    // 🔹 Buscar lançamentos por ContaId
    public async Task<List<Lancamento>> ObterLancamentosPorConta(string contaId)
    {
        try
        {
            JsonLogger.Log("INFO", "Consulta de lançamentos iniciada", new { ContaId = contaId });

            var conditions = new List<ScanCondition>
            {
                new("ContaId", ScanOperator.Equal, contaId)
            };

            var lancamentos = await _context.ScanAsync<Lancamento>(conditions).GetRemainingAsync();

            if (lancamentos == null || lancamentos.Count == 0)
            {
                JsonLogger.Log("WARN", "Nenhum lançamento encontrado", new { ContaId = contaId });
                throw new KeyNotFoundException($"Nenhum lançamento encontrado para a conta '{contaId}'.");
            }

            JsonLogger.Log("INFO", "Consulta de lançamentos concluída", new { ContaId = contaId, Quantidade = lancamentos.Count });

            return lancamentos;
        }
        catch (ResourceNotFoundException)
        {
            JsonLogger.Log("ERROR", "Tabela não encontrada no DynamoDB", new { Tabela = LancamentosTableName });
            throw new Exception($"A tabela '{LancamentosTableName}' não foi encontrada no DynamoDB. Verifique se o esquema foi criado pelo Microserviço de Integrações.");
        }
        catch (Exception ex)
        {
            JsonLogger.Log("ERROR", "Erro ao buscar lançamentos", new { ContaId = contaId, Erro = ex.Message });
            throw;
        }
    }

    // 🔹 Buscar um lançamento específico por LancamentoId
    public async Task<Lancamento?> ObterLancamentoPorId(string contaId, Guid lancamentoId)
    {
        try
        {
            JsonLogger.Log("INFO", "Consulta de lançamento iniciada", new { ContaId = contaId, LancamentoId = lancamentoId });

            var lancamento = await _context.LoadAsync<Lancamento>(contaId, lancamentoId);

            if (lancamento == null)
            {
                JsonLogger.Log("WARN", "Lançamento não encontrado", new { ContaId = contaId, LancamentoId = lancamentoId });
                throw new KeyNotFoundException($"Nenhum lançamento encontrado com o ID '{lancamentoId}' para a conta '{contaId}'.");
            }

            JsonLogger.Log("INFO", "Consulta de lançamento concluída", new { ContaId = contaId, LancamentoId = lancamentoId });

            return lancamento;
        }
        catch (ResourceNotFoundException)
        {
            JsonLogger.Log("ERROR", "Tabela não encontrada no DynamoDB", new { Tabela = LancamentosTableName });
            throw new Exception($"A tabela '{LancamentosTableName}' não foi encontrada no DynamoDB. Verifique se o esquema foi criado pelo Microserviço de Integrações.");
        }
        catch (Exception ex)
        {
            JsonLogger.Log("ERROR", "Erro ao buscar lançamento", new { ContaId = contaId, LancamentoId = lancamentoId, Erro = ex.Message });
            throw;
        }
    }
}
