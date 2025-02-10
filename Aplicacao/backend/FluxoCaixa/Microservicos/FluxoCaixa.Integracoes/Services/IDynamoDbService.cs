using FluxoCaixa.Integracoes.Models;

namespace FluxoCaixa.Integracoes.Services
{
    public interface IDynamoDbService
    {
        /// 🔹 Verifica e cria tabelas no DynamoDB, se necessário.
        Task CriarTabelasSeNaoExistirem();

        /// 🔹 Salva um novo lançamento no DynamoDB.
        Task SalvarLancamento(Lancamento lancamento);

        /// 🔹 Obtém todos os lançamentos de uma conta.
        Task<List<Lancamento>> ObterLancamentosPorConta(string contaId);

        /// 🔹 Remove um lançamento específico do DynamoDB.
        Task DeletarLancamento(string contaId, Guid lancamentoId);

        /// 🔹 Reprocessa os dados consolidados em um período específico ou em toda a base.
        Task ReprocessarConsolidado(DateTime? dataInicio = null, DateTime? dataFim = null);
    }
}
