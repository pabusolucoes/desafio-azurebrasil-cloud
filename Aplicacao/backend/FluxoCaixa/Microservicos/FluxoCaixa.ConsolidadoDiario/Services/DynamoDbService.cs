using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using FluxoCaixa.ConsolidadoDiario.Extensions;
using FluxoCaixa.ConsolidadoDiario.Models;
using FluxoCaixa.ConsolidadoDiario.Shared;
namespace FluxoCaixa.ConsolidadoDiario.Services
{
    public class DynamoDbService : IDynamoDbService
    {
        private readonly AmazonDynamoDBClient _client;
        private readonly DynamoDBContext _context;
        private const string ConsolidadosTableName = "ConsolidadosDiarios";
        private readonly ICustomEnvironment _env;

        public DynamoDbService(ICustomEnvironment env, DynamoDBContext? context = null)
        {
            _env = env;

            var dynamoConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = _env.IsLocal() ? "http://localhost:8000" : "http://dynamodb:8000",
                AuthenticationRegion = "us-east-1"
            };

            _client = new AmazonDynamoDBClient("fakeMyKeyId", "fakeSecretAccessKey", dynamoConfig);
            _context = context ?? new DynamoDBContext(_client);

            JsonLogger.Log("INFO", "DynamoDbService inicializado", new { Ambiente = _env.IsLocal() ? "Local" : "Produção" });
        }

        public async Task<List<ConsolidadoDiarioModel>> ObterConsolidadoPorPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            try
            {
                var tables = await _client.ListTablesAsync();
                
                var conditions = new List<ScanCondition>
                {
                    new("Data", ScanOperator.Between, dataInicial, dataFinal)
                };

                var consolidados = await _context.ScanAsync<ConsolidadoDiarioModel>(conditions).GetRemainingAsync();

                if (consolidados == null || consolidados.Count == 0)
                {
                    throw new KeyNotFoundException($"Nenhum consolidado encontrado entre {dataInicial:yyyy-MM-dd} e {dataFinal:yyyy-MM-dd}.");
                }

                return consolidados;
            }
            catch (ResourceNotFoundException)
            {
                throw new Exception($"A tabela '{ConsolidadosTableName}' não foi encontrada no DynamoDB.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar consolidados: {ex.Message}");
            }
        }
    }
}
