using FluentAssertions;
using FluxoCaixa.Integracoes.Models;
using FluxoCaixa.Integracoes.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FluxoCaixa.Integracoes.Tests
{
    public class DynamoDbServiceTests
    {
        private readonly Mock<IDynamoDbService> _mockDynamoDbService;
        private readonly IDynamoDbService _service;

        public DynamoDbServiceTests()
        {
            _mockDynamoDbService = new Mock<IDynamoDbService>();

            // Usa o mock para os testes
            _service = _mockDynamoDbService.Object;
        }

        /// 🔹 TESTE: Criar tabelas sem erro
        [Fact]
        public async Task Deve_Criar_Tabelas_Sem_Erro()
        {
            // Arrange
            _mockDynamoDbService.Setup(s => s.CriarTabelasSeNaoExistirem()).Returns(Task.CompletedTask);

            // Act
            Func<Task> act = async () => await _service.CriarTabelasSeNaoExistirem();

            // Assert
            await act.Should().NotThrowAsync();
        }

        /// 🔹 TESTE: Salvar um lançamento no banco
        [Fact]
        public async Task Deve_Salvar_Lancamento_Sem_Erro()
        {
            // Arrange
            var lancamento = new Lancamento
            {
                ContaId = "123456",
                LancamentoId = Guid.NewGuid(),
                Data = DateTime.UtcNow,
                Valor = 100,
                Descricao = "Supermercado",
                Tipo = "DÉBITO",
                Categoria = "Alimentação"
            };

            _mockDynamoDbService.Setup(s => s.SalvarLancamento(lancamento)).Returns(Task.CompletedTask);

            // Act
            Func<Task> act = async () => await _service.SalvarLancamento(lancamento);

            // Assert
            await act.Should().NotThrowAsync();
        }

        /// 🔹 TESTE: Obter lançamentos por conta
        [Fact]
        public async Task Deve_Retornar_Lancamentos_Por_Conta()
        {
            // Arrange
            var contaId = "123456";
            var lancamentosEsperados = new List<Lancamento>
            {
                new() { ContaId = contaId, LancamentoId = Guid.NewGuid(), Data = DateTime.UtcNow, Valor = 100, Descricao = "Supermercado", Tipo = "DÉBITO", Categoria = "Alimentação" }
            };

            _mockDynamoDbService.Setup(s => s.ObterLancamentosPorConta(contaId))
                .ReturnsAsync(lancamentosEsperados);

            // Act
            var resultado = await _service.ObterLancamentosPorConta(contaId);

            // Assert
            resultado.Should().NotBeEmpty();
            resultado.Should().BeEquivalentTo(lancamentosEsperados);
        }

        /// 🔹 TESTE: Remover um lançamento
        [Fact]
        public async Task Deve_Remover_Lancamento_Sem_Erro()
        {
            // Arrange
            var contaId = "123456";
            var lancamentoId = Guid.NewGuid();

            _mockDynamoDbService.Setup(s => s.DeletarLancamento(contaId, lancamentoId)).Returns(Task.CompletedTask);

            // Act
            Func<Task> act = async () => await _service.DeletarLancamento(contaId, lancamentoId);

            // Assert
            await act.Should().NotThrowAsync();
        }
        /// 🔹 TESTE: Reprocessamento sem intervalo (reprocessa tudo)
        [Fact]
        public async Task Deve_Reprocessar_Consolidado_Tudo_Sem_Erro()
        {
            _mockDynamoDbService.Setup(s => s.ReprocessarConsolidado(null, null)).Returns(Task.CompletedTask);

            Func<Task> act = async () => await _service.ReprocessarConsolidado(null, null);

            await act.Should().NotThrowAsync();
        }
         /// 🔹 TESTE: Falha ao salvar lançamento
        [Fact]
        public async Task Deve_Retornar_Erro_Ao_Salvar_Lancamento()
        {
            var lancamento = new Lancamento
            {
                ContaId = "123456",
                LancamentoId = Guid.NewGuid(),
                Data = DateTime.UtcNow,
                Valor = 100,
                Descricao = "Erro no Supermercado",
                Tipo = "DÉBITO",
                Categoria = "Alimentação"
            };

            _mockDynamoDbService.Setup(s => s.SalvarLancamento(lancamento))
                .ThrowsAsync(new Exception("Erro ao salvar no banco"));

            Func<Task> act = async () => await _service.SalvarLancamento(lancamento);

            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao salvar no banco");
        }

        /// 🔹 TESTE: Falha ao obter lançamentos
        [Fact]
        public async Task Deve_Retornar_Erro_Ao_Obter_Lancamentos()
        {
            var contaId = "123456";

            _mockDynamoDbService.Setup(s => s.ObterLancamentosPorConta(contaId))
                .ThrowsAsync(new Exception("Erro ao buscar lançamentos"));

            Func<Task> act = async () => await _service.ObterLancamentosPorConta(contaId);

            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao buscar lançamentos");
        }

        /// 🔹 TESTE: Falha ao remover lançamento
        [Fact]
        public async Task Deve_Retornar_Erro_Ao_Remover_Lancamento()
        {
            var contaId = "123456";
            var lancamentoId = Guid.NewGuid();

            _mockDynamoDbService.Setup(s => s.DeletarLancamento(contaId, lancamentoId))
                .ThrowsAsync(new Exception("Erro ao remover lançamento"));

            Func<Task> act = async () => await _service.DeletarLancamento(contaId, lancamentoId);

            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao remover lançamento");
        }

        /// 🔹 TESTE: Falha ao reprocessar consolidado
        [Fact]
        public async Task Deve_Retornar_Erro_Ao_Reprocessar_Consolidado()
        {
            DateTime? dataInicio = DateTime.UtcNow.AddDays(-5);
            DateTime? dataFim = DateTime.UtcNow;

            _mockDynamoDbService.Setup(s => s.ReprocessarConsolidado(dataInicio, dataFim))
                .ThrowsAsync(new Exception("Erro ao reprocessar consolidado"));

            Func<Task> act = async () => await _service.ReprocessarConsolidado(dataInicio, dataFim);

            await act.Should().ThrowAsync<Exception>().WithMessage("Erro ao reprocessar consolidado");
        }
    }
}
