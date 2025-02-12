namespace FluxoCaixa.Integracoes.Models
{
    public class MensagemLancamento
    {
        public string Acao { get; set; } = string.Empty;
        public Lancamento? Lancamento { get; set; }
    }
}