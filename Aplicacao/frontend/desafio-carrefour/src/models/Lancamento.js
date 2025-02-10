export class Lancamento {
    constructor(
      contaId = "",
      lancamentoId = null,
      descricao = "",
      tipo = "DÉBITO",
      categoria = "",
      data = new Date().toISOString().split("T")[0], // Formato YYYY-MM-DD
      valor = 0.0
    ) {
      this.contaId = contaId;
      this.lancamentoId = lancamentoId;
      this.descricao = descricao;
      this.tipo = tipo;
      this.categoria = categoria;
      this.data = data;
      this.valor = valor;
    }
  }
  