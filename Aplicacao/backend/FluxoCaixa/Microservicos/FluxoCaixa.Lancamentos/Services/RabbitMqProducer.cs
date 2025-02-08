using FluxoCaixa.Lancamentos.Extensions;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
namespace FluxoCaixa.Lancamentos.Services;
public class RabbitMqProducer
{
    private readonly string _queueName = "fluxo-caixa-queue";
    private readonly ConnectionFactory _factory;

    private readonly ICustomEnvironment _env;

    public RabbitMqProducer(ICustomEnvironment env)
    {
        _env = env;

        if (_env.IsLocal())
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "admin"
            };
        }
        else
        {
            _factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                UserName = "admin",
                Password = "admin"
            };
        }
    }

    public void Publish<T>(T message)
    {
        try
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: properties, body: body);

            Console.WriteLine($"[x] Mensagem publicada: {JsonSerializer.Serialize(message)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Erro] Falha ao publicar mensagem: {ex.Message}");
        }
    }
}
