using Amazon.DynamoDBv2.DataModel;
using System;
using System.Text.Json.Serialization;

namespace FluxoCaixa.Lancamentos.Models
{
    [DynamoDBTable("Lancamentos")]
    public class Lancamento
    {
        [DynamoDBHashKey] // Chave de partição
        public string ContaId { get; set; } = string.Empty;

        [DynamoDBRangeKey] // Chave de ordenação
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] //serializa se tiver valor
        public Guid LancamentoId { get; set; }

        [DynamoDBProperty]
        public string Descricao { get; set; } = string.Empty;

        [DynamoDBProperty]
        public string Tipo { get; set; } = string.Empty; // "DÉBITO" ou "CRÉDITO"

        [DynamoDBProperty]
        public string Categoria { get; set; } = string.Empty;

        [DynamoDBProperty]
        public DateTime Data { get; set; }

        [DynamoDBProperty]
        public decimal Valor { get; set; }

        // Construtor padrão
        public Lancamento()
        {
        }

        // Construtor customizado para facilitar a criação de lançamentos
        public Lancamento(string contaId, string descricao, string tipo, string categoria, DateTime data, decimal valor)
        {
            ContaId = contaId;
            LancamentoId = Guid.NewGuid(); // Gera um novo ID automaticamente
            Descricao = descricao;
            Tipo = tipo;
            Categoria = categoria;
            Data = data;
            Valor = valor;
        }
    }
}
