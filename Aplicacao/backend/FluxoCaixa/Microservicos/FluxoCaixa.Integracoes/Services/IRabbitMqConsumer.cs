using System.Threading.Tasks;

namespace FluxoCaixa.Integracoes.Services
{
    public interface IRabbitMqConsumer
    {
        /// 🔹 Inicia o consumo de mensagens do RabbitMQ.
        void StartConsuming();
    }
}
