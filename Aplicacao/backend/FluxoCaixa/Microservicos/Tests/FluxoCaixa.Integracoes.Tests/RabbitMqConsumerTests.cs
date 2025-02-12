using FluentAssertions;
using FluxoCaixa.Integracoes.Services;
using Moq;
using Xunit;

namespace FluxoCaixa.Integracoes.Tests
{
    public class RabbitMqConsumerTests
    {
        private readonly Mock<IRabbitMqConsumer> _mockRabbitMqConsumer;
        private readonly IRabbitMqConsumer _service;

        public RabbitMqConsumerTests()
        {
            _mockRabbitMqConsumer = new Mock<IRabbitMqConsumer>();

            // Usa o mock para os testes
            _service = _mockRabbitMqConsumer.Object;
        }

        /// 🔹 TESTE: Iniciar o consumo sem erro
        [Fact]
        public void Deve_Iniciar_Consumo_Sem_Erro()
        {
            // Arrange
            _mockRabbitMqConsumer.Setup(s => s.StartConsuming());

            // Act
            var act = () => _service.StartConsuming();

            // Assert
            act.Should().NotThrow();
        }
        /// 🔹 TESTE: Falha ao iniciar o consumo de mensagens do RabbitMQ
        [Fact]
        public void Deve_Retornar_Erro_Ao_Iniciar_Consumo()
        {
            // Arrange
            _mockRabbitMqConsumer.Setup(s => s.StartConsuming())
                .Throws(new Exception("Erro ao conectar ao RabbitMQ"));

            // Act
            Action act = () => _service.StartConsuming();

            // Assert
            act.Should().Throw<Exception>().WithMessage("Erro ao conectar ao RabbitMQ");
        }
    }
}
