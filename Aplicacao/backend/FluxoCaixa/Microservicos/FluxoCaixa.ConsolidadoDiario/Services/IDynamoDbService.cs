using System.Runtime.CompilerServices;
using FluxoCaixa.ConsolidadoDiario.Models;

namespace FluxoCaixa.ConsolidadoDiario.Services
{
    public interface IDynamoDbService
    {
        Task<List<ConsolidadoDiarioModel>> ObterConsolidadoPorPeriodo(DateTime dataInicial, DateTime dataFinal, string contaId);
    }
}
