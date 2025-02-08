using Amazon.DynamoDBv2.DataModel;

namespace FluxoCaixa.Integracoes.Models;

[DynamoDBTable("ConsolidadosDiarios")] // Define a tabela no DynamoDB
public class ConsolidadoDiario
{
    [DynamoDBHashKey] // Chave de Partição (Partition Key)
    public string ContaId { get; set; } = string.Empty;

    [DynamoDBRangeKey] // Chave de Ordenação (Sort Key)
    public DateTime Data { get; set; } // Data do consolidado

    [DynamoDBProperty]
    public decimal TotalDebitos { get; set; }

    [DynamoDBProperty]
    public decimal TotalCreditos { get; set; }

    [DynamoDBProperty]
    public decimal Saldo { get; set; }

    public ConsolidadoDiario() {} // Construtor vazio exigido pelo DynamoDB

    public ConsolidadoDiario(string contaId, DateTime data, decimal totalDebitos, decimal totalCreditos, decimal saldo)
    {
        ContaId = contaId;
        Data = data;
        TotalDebitos = totalDebitos;
        TotalCreditos = totalCreditos;
        Saldo = saldo;
    }
}
