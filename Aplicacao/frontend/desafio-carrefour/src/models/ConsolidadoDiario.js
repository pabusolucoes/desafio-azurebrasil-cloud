export class ConsolidadoDiario {
    constructor(
      contaId = "",
      data = new Date().toISOString().split("T")[0],
      totalDebitos = 0.0,
      totalCreditos = 0.0,
      saldo = 0.0
    ) {
      this.contaId = contaId;
      this.data = data;
      this.totalDebitos = totalDebitos;
      this.totalCreditos = totalCreditos;
      this.saldo = saldo;
    }
  }
  