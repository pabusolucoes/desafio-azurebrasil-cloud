namespace FluxoCaixa.Lancamentos.Models;

public class Lancamento
{
    public Guid Id { get; set; }
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;

    public Lancamento() {} // Construtor vazio

    public Lancamento(Guid id, DateTime data, decimal valor, string descricao, string tipo, string categoria)
    {
        Id = id;
        Data = data;
        Valor = valor;
        Descricao = descricao;
        Tipo = tipo;
        Categoria = categoria;
    }
}
