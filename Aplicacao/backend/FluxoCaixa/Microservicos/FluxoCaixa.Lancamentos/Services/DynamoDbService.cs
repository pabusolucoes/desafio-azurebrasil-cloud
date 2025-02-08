using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using FluxoCaixa.Lancamentos.Extensions;
using FluxoCaixa.Lancamentos.Models;

namespace FluxoCaixa.Lancamentos.Services;

public class DynamoDbService
{
    private readonly AmazonDynamoDBClient _client;
    private readonly DynamoDBContext _context;
        private readonly ICustomEnvironment _env;
    private const string LancamentosTableName = "Lancamentos";

     public DynamoDbService(ICustomEnvironment env)
    {
        _env = env;
        // Configura o cliente DynamoDB apontando para o DynamoDB Local
        if (_env.IsLocal())
        {
            var dynamoConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000", // 🔹 Endereço do DynamoDB rodando no Docker
                AuthenticationRegion = "us-east-1"
            };

            // Configurar credenciais estáticas para evitar erro de IAM
            _client = new AmazonDynamoDBClient("fakeMyKeyId", "fakeSecretAccessKey", dynamoConfig);
        }
        else
        {
            var dynamoConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://dynamodb:8000", // 🔹 Endereço do DynamoDB rodando no Docker
                AuthenticationRegion = "us-east-1"
            };

            // 🔹 Configurar credenciais estáticas para evitar erro de IAM
            _client = new AmazonDynamoDBClient("fakeMyKeyId", "fakeSecretAccessKey", dynamoConfig);
        }
        _context = new DynamoDBContext(_client);

    }

    // Buscar lançamentos por ContaId
    public async Task<List<Lancamento>> ObterLancamentosPorConta(string contaId)
    {
        try
        {
            var conditions = new List<ScanCondition>
            {
                new("ContaId", ScanOperator.Equal, contaId)
            };

            var lancamentos = await _context.ScanAsync<Lancamento>(conditions).GetRemainingAsync();

            if (lancamentos == null || lancamentos.Count == 0)
            {
                throw new KeyNotFoundException($"Nenhum lançamento encontrado para a conta '{contaId}'.");
            }

            return lancamentos;
        }
        catch (ResourceNotFoundException)
        {
            throw new Exception($"A tabela '{LancamentosTableName}' não foi encontrada no DynamoDB. Verifique se o esquema foi criado pelo Microserviço de Integrações.");
        }
    }

    // Buscar um lançamento específico por LancamentoId
    public async Task<Lancamento?> ObterLancamentoPorId(string contaId, Guid lancamentoId)
    {
        try
        {
            var lancamento = await _context.LoadAsync<Lancamento>(contaId, lancamentoId);

            if (lancamento == null)
            {
                throw new KeyNotFoundException($"Nenhum lançamento encontrado com o ID '{lancamentoId}' para a conta '{contaId}'.");
            }

            return lancamento;
        }
        catch (ResourceNotFoundException)
        {
            throw new Exception($"A tabela '{LancamentosTableName}' não foi encontrada no DynamoDB. Verifique se o esquema foi criado pelo Microserviço de Integrações.");
        }
    }
}
