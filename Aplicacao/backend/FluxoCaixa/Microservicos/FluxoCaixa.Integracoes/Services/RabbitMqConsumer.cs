using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using FluxoCaixa.Integracoes.Models;
using FluxoCaixa.Integracoes.Extensions;
using FluxoCaixa.Integracoes.Shared; // 🔹 Importação do JsonLogger

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

        JsonLogger.Log("INFO", "RabbitMqConsumer inicializado", new { Ambiente = _env.IsLocal() ? "Local" : "Produção" });

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
                // 🔹 Desserializa a mensagem completa (incluindo Ação e Lançamento)
                var wrapper = JsonSerializer.Deserialize<MensagemLancamento>(message);

                if (wrapper == null || string.IsNullOrEmpty(wrapper.Acao))
                {
                    JsonLogger.Log("ERROR", "Mensagem inválida recebida", new { Mensagem = message });
                    return;
                }

                JsonLogger.Log("INFO", "Mensagem recebida do RabbitMQ", new { Acao = wrapper.Acao, Lançamento = wrapper.Lancamento });

                if (wrapper.Acao == "ReprocessarConsolidado")
                {
                    await _dynamoDbService.ReprocessarConsolidado();
                    JsonLogger.Log("INFO", "Reprocessamento solicitado");
                }
                else if ((wrapper.Acao == "CriarLancamento" || wrapper.Acao == "AtualizarLancamento") && wrapper.Lancamento != null)
                {
                    await _dynamoDbService.SalvarLancamento(wrapper.Lancamento);
                    JsonLogger.Log("INFO", "Lançamento salvo/atualizado", new { Acao = wrapper.Acao, LancamentoId = wrapper.Lancamento.LancamentoId });
                }
                else if (wrapper.Acao == "DeletarLancamento" && wrapper.Lancamento != null)
                {
                    await _dynamoDbService.DeletarLancamento(wrapper.Lancamento.ContaId, wrapper.Lancamento.LancamentoId);
                    JsonLogger.Log("INFO", "Lançamento removido", new { Acao = wrapper.Acao, LancamentoId = wrapper.Lancamento.LancamentoId });
                }
                else
                {
                    JsonLogger.Log("WARN", "Ação desconhecida recebida", new { Acao = wrapper.Acao });
                }
            }
            catch (Exception ex)
            {
                JsonLogger.Log("ERROR", "Falha ao processar mensagem", new { Erro = ex.Message, Mensagem = message });
            }
        };

        channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
    }
}
