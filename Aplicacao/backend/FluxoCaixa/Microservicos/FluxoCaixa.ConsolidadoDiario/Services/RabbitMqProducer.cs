using FluxoCaixa.ConsolidadoDiario.Extensions;
using FluxoCaixa.ConsolidadoDiario.Shared; // 🔹 Importação do JsonLogger
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FluxoCaixa.ConsolidadoDiario.Services;

public class RabbitMqProducer:IRabbitMqProducer
{
    private readonly string _queueName = "fluxo-caixa-queue";
    private readonly IConnectionFactory  _factory;
    private readonly ICustomEnvironment _env;

        public RabbitMqProducer(ICustomEnvironment env, IConnectionFactory factory)
    {
        _env = env;
        _factory = factory;

        JsonLogger.Log("INFO", "RabbitMqProducer inicializado", new { Ambiente = _env.IsLocal() ? "Local" : "Produção" });
    }

    public void Publish<T>(T message)
    {
        try
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            // 🔹 Serializa a mensagem para JSON
            var messageJson = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: properties, body: body);

            // 🔹 Loga a mensagem publicada no RabbitMQ
            JsonLogger.Log("INFO", "Mensagem publicada no RabbitMQ", new { Fila = _queueName, Mensagem = message });
        }
        catch (Exception ex)
        {
            // 🔹 Loga erro na publicação da mensagem
            JsonLogger.Log("ERROR", "Falha ao publicar mensagem no RabbitMQ", new { Erro = ex.Message });
        }
    }
}
