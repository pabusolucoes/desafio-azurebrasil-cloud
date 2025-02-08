using Amazon.DynamoDBv2.DataModel;

namespace FluxoCaixa.Integracoes.Models;

[DynamoDBTable("Lancamentos")] // Define a tabela no DynamoDB
public class Lancamento
{
    [DynamoDBHashKey] // Chave de Partição (Partition Key)
    public string ContaId { get; set; } = string.Empty;

    [DynamoDBRangeKey] // Chave de Ordenação (Sort Key)
    public Guid LancamentoId { get; set; }

    [DynamoDBProperty]
    public DateTime Data { get; set; }

    [DynamoDBProperty]
    public decimal Valor { get; set; }

    [DynamoDBProperty]
    public string Descricao { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Tipo { get; set; } = string.Empty; // DÉBITO ou CRÉDITO

    [DynamoDBProperty]
    public string Categoria { get; set; } = string.Empty;
    
    public Lancamento() {} // Construtor vazio exigido pelo DynamoDB

    public Lancamento(string contaId, Guid lancamentoId, DateTime data, decimal valor, string descricao, string tipo, string categoria)
    {
        ContaId = contaId;
        LancamentoId = lancamentoId;
        Data = data;
        Valor = valor;
        Descricao = descricao;
        Tipo = tipo;
        Categoria = categoria;
    }
}
