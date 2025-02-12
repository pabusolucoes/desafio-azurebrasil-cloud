namespace FluxoCaixa.ConsolidadoDiario.Services
{
    public interface IRabbitMqProducer
    {
        /// 🔹 Publica uma mensagem na fila do RabbitMQ.
        void Publish<T>(T message);
    }
}
