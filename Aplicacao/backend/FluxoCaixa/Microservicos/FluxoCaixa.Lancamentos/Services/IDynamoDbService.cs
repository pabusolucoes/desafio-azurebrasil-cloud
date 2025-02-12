using FluxoCaixa.Lancamentos.Models;

namespace FluxoCaixa.Lancamentos.Services
{
    public interface IDynamoDbService
    {
        Task<List<Lancamento>> ObterLancamentosPorConta(string contaId);
        Task<Lancamento?> ObterLancamentoPorId(string contaId, Guid lancamentoId);
    }
}