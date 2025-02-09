using FluentAssertions;
using FluxoCaixa.Lancamentos.Services;
using Moq;
using FluxoCaixa.Lancamentos.Extensions;
using Microsoft.Extensions.Hosting;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace FluxoCaixa.Lancamentos.Tests;
public class DynamoDbServiceTests
{

    private readonly Mock<ICustomEnvironment> _mockEnv;
    private readonly Mock<IHostEnvironment> _mockHostEnvironment;
    private readonly DynamoDbService _service;

    public DynamoDbServiceTests()
    {
        _mockEnv = new Mock<ICustomEnvironment>();
        _mockHostEnvironment = new Mock<IHostEnvironment>();

        // Configura o mock do `IHostEnvironment` para simular que o ambiente é "Local"
        _mockHostEnvironment.Setup(e => e.EnvironmentName).Returns("Local");

        // Configura o mock do `ICustomEnvironment` para sempre retornar `true` para `IsLocal()`
        _mockEnv.Setup(e => e.IsLocal()).Returns(true);

        // Criamos um DynamoDBContext FALSO que aponta para DynamoDB Local
        var dynamoClient = new AmazonDynamoDBClient("fakeMyKeyId", "fakeSecretAccessKey", new AmazonDynamoDBConfig
        {
            ServiceURL = "http://localhost:8000"
        });

        var mockDynamoContext = new DynamoDBContext(dynamoClient);

        // Criamos a instância do serviço passando o contexto mockado
        _service = new DynamoDbService(_mockEnv.Object, mockDynamoContext);
    }

    [Fact]
    public async Task Deve_Retornar_Lancamentos_Por_Conta()
    {
        // Arrange
        var contaId = "123456";

        // Act
        var resultado = await _service.ObterLancamentosPorConta(contaId);

        // Assert
        resultado.Should().NotBeEmpty(); // Simula conta sem lançamentos
    }

    [Fact]
    public async Task Deve_Retornar_404_Se_Lancamento_Nao_Existir()
    {
        // Arrange
        var contaId = "1234567";
        var lancamentoId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await _service.ObterLancamentoPorId(contaId, lancamentoId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Deve_Retornar_500_Se_Houver_Erro_No_Servidor()
    {
        // Arrange
        var contaId = "1234567";
        var lancamentoId = Guid.NewGuid();

        // Forçamos um erro interno ao injetar um contexto nulo
        typeof(DynamoDbService)
            .GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(_service, null);

        // Act
        Func<Task> act = async () => await _service.ObterLancamentoPorId(contaId, lancamentoId);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
    /// 🔹 TESTE: Obter um lançamento específico do banco de testes
    [Fact]
    public async Task Deve_Retornar_Lancamento_Por_Id()
    {
        // Arrange
        var contaId = "123456";
        var lancamentoId = Guid.Parse("133e302e-a352-4310-b77c-de4ccab0ec33"); // 🔹 ID que já existe no DynamoDB Local

        // Act
        var resultado = await _service.ObterLancamentoPorId(contaId, lancamentoId);

        // Assert
        resultado.Should().NotBeNull();
    }

    /// 🔹 TESTE: Retornar 404 se um lançamento específico não existir
    [Fact]
    public async Task Deve_Retornar_404_Se_Lancamento_Nao_Existir_na_rota_ObterLancamentoPorId()
    {
        // Arrange
        var contaId = "123456";
        var lancamentoId = Guid.NewGuid(); // 🔹 Gera um ID que não existe no banco

        // Act
        Func<Task> act = async () => await _service.ObterLancamentoPorId(contaId, lancamentoId);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
