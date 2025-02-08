using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using FluxoCaixa.Integracoes.Extensions;
using FluxoCaixa.Integracoes.Models; // 🔹 Adicione essa linha para reconhecer a model

namespace FluxoCaixa.Integracoes.Services;
public class DynamoDbService
{
    private readonly AmazonDynamoDBClient _client;
    private readonly DynamoDBContext _context;
    private const string LancamentosTableName = "Lancamentos";
    private const string ConsolidadosTableName = "ConsolidadosDiarios";

    private readonly ICustomEnvironment _env;

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

            // 🔹 Configurar credenciais estáticas para evitar erro de IAM
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

    public async Task CriarTabelasSeNaoExistirem()
    {
        var tables = await _client.ListTablesAsync();

        // Criando a tabela Lancamentos se não existir
        if (!tables.TableNames.Contains(LancamentosTableName))
        {
            var request = new CreateTableRequest
            {
                TableName = LancamentosTableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new() { AttributeName = "ContaId", AttributeType = "S" },
                    new() { AttributeName = "LancamentoId", AttributeType = "S" },
                    new() { AttributeName = "Data", AttributeType = "S" }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new() { AttributeName = "ContaId", KeyType = "HASH" },
                    new() { AttributeName = "LancamentoId", KeyType = "RANGE" }
                },
                GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
                {
                    new() {
                        IndexName = "DataIndex",
                        KeySchema = new List<KeySchemaElement>
                        {
                            new() { AttributeName = "ContaId", KeyType = "HASH" },
                            new() { AttributeName = "Data", KeyType = "RANGE" }
                        },
                        Projection = new Projection { ProjectionType = "ALL" },
                        ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 5, WriteCapacityUnits = 5 }
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 5, WriteCapacityUnits = 5 }
            };

            await _client.CreateTableAsync(request);
        }

        // Criando a tabela ConsolidadosDiarios se não existir
        if (!tables.TableNames.Contains(ConsolidadosTableName))
        {
            var request = new CreateTableRequest
            {
                TableName = ConsolidadosTableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new() { AttributeName = "ContaId", AttributeType = "S" },
                    new() { AttributeName = "Data", AttributeType = "S" }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new() { AttributeName = "ContaId", KeyType = "HASH" },
                    new() { AttributeName = "Data", KeyType = "RANGE" }
                },
                ProvisionedThroughput = new ProvisionedThroughput { ReadCapacityUnits = 5, WriteCapacityUnits = 5 }
            };

            await _client.CreateTableAsync(request);
        }
    }
    // FUNÇÃO PARA SALVAR LANÇAMENTOS NO DYNAMODB
    public async Task SalvarLancamento(Lancamento lancamento)
    {
        await _context.SaveAsync(lancamento);
        Console.WriteLine($"[DynamoDB] Lançamento salvo: {lancamento.LancamentoId}");
    }

    // FUNÇÃO PARA OBTER LANÇAMENTOS POR CONTA
    public async Task<List<Lancamento>> ObterLancamentosPorConta(string contaId)
    {
        var conditions = new List<ScanCondition>
        {
            new("ContaId", ScanOperator.Equal, contaId)
        };

        return await _context.ScanAsync<Lancamento>(conditions).GetRemainingAsync();
    }
    public async Task DeletarLancamento(string contaId, Guid lancamentoId)
{
    try
    {
        await _context.DeleteAsync<Lancamento>(contaId, lancamentoId);
        Console.WriteLine($"[DynamoDB] Lançamento {lancamentoId} removido com sucesso.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Erro] Falha ao remover lançamento {lancamentoId}: {ex.Message}");
        throw;
    }
}
}
