namespace FluxoCaixa.Lancamentos.Services
{
    public interface IRabbitMqProducer
    {
        void Publish<T>(T message);
    }
}
