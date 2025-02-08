using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using FluxoCaixa.Integracoes.Models;
using FluxoCaixa.Integracoes.Extensions; // 🔹 Adicione essa linha para reconhecer a model

namespace FluxoCaixa.Integracoes.Services;

public class RabbitMqConsumer
{
    private readonly string _queueName = "fluxo-caixa-queue";
    private readonly DynamoDbService _dynamoDbService;

    private readonly ICustomEnvironment _env;

    public RabbitMqConsumer(ICustomEnvironment env)
    {
        _env = env;
        _dynamoDbService = new DynamoDbService(_env);
        _dynamoDbService.CriarTabelasSeNaoExistirem().Wait(); // 🔹 Criar a tabela ao iniciar o serviço
    }
    public void StartConsuming()
    {
        var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "admin", Password = "admin" };
        if (_env.IsLocal())
        {
            factory = new ConnectionFactory() { HostName = "localhost", UserName = "admin", Password = "admin" };
        }
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            try
            {
                var lancamento = JsonSerializer.Deserialize<Lancamento>(message);

                if (lancamento == null)
                {
                    Console.WriteLine("[Erro] Mensagem recebida inválida: Deserialização resultou em null.");
                    return;
                }
                Console.WriteLine($"[x] Recebido e processado: {JsonSerializer.Serialize(lancamento)}");

                // 🔹 Salvar no DynamoDB
                await _dynamoDbService.SalvarLancamento(lancamento);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Erro] Falha ao processar mensagem: {ex.Message}");
            }
        };

        channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
    }
}
