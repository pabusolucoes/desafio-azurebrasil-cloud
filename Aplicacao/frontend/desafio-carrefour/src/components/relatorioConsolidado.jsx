import { useState } from "react";
import api from "../services/api";
import "../styles/relatorio.css";
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";
import * as XLSX from "xlsx";
import { saveAs } from "file-saver";

function RelatorioConsolidado() {
  const [dataInicio, setDataInicio] = useState("");
  const [dataFim, setDataFim] = useState("");
  const [relatorio, setRelatorio] = useState([]);

  const mockRelatorio = [
    { data: "2025-02-01", totalDebitos: 170.5, totalCreditos: 0, saldo: -170.5 },
    { data: "2025-02-02", totalDebitos: 0, totalCreditos: 200.0, saldo: 200.0 },
    { data: "2025-02-03", totalDebitos: 165.0, totalCreditos: 0, saldo: -165.0 },
    { data: "2025-02-04", totalDebitos: 0, totalCreditos: 300.0, saldo: 300.0 },
    { data: "2025-02-05", totalDebitos: 115.0, totalCreditos: 0, saldo: -115.0 },
    { data: "2025-02-06", totalDebitos: 95.0, totalCreditos: 0, saldo: -95.0 },
    { data: "2025-02-07", totalDebitos: 200.0, totalCreditos: 0, saldo: -200.0 },
    { data: "2025-02-08", totalDebitos: 40.0, totalCreditos: 0, saldo: -40.0 },
    { data: "2025-02-09", totalDebitos: 0, totalCreditos: 500.0, saldo: 500.0 },
  ];

  const gerarRelatorio = async (e) => {
    e.preventDefault();
    try {
      const response = await api.get("/relatorio", {
        params: { dataInicio, dataFim },
      });
      setRelatorio(response.data);
    } catch {
      console.warn("API indisponível, usando mock.");
      setRelatorio(mockRelatorio);
    }
  };

  const exportarPDF = () => {
    const doc = new jsPDF();
    doc.text("Relatório Consolidado Diário", 14, 10);
    autoTable(doc, {
      head: [["Data", "Total Débitos", "Total Créditos", "Saldo"]],
      body: relatorio.map((item) => [
        item.data,
        item.totalDebitos.toFixed(2),
        item.totalCreditos.toFixed(2),
        item.saldo.toFixed(2),
      ]),
    });
    doc.save("relatorio-consolidado-diario.pdf");
  };

  const exportarExcel = () => {
    const worksheet = XLSX.utils.json_to_sheet(relatorio);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Relatório Diário");
    const excelBuffer = XLSX.write(workbook, { bookType: "xlsx", type: "array" });
    const data = new Blob([excelBuffer], { type: "application/octet-stream" });
    saveAs(data, "relatorio-consolidado-diario.xlsx");
  };

  const exportarMarkdown = () => {
    let markdown = "# Relatório Consolidado Diário\n\n| Data | Total Débitos | Total Créditos | Saldo |\n|------|---------------|----------------|-------|\n";
    relatorio.forEach((item) => {
      markdown += `| ${item.data} | ${item.totalDebitos.toFixed(2)} | ${item.totalCreditos.toFixed(2)} | ${item.saldo.toFixed(2)} |\n`;
    });
    const blob = new Blob([markdown], { type: "text/markdown;charset=utf-8" });
    saveAs(blob, "relatorio-consolidado-diario.md");
  };

  return (
    <div className="relatorio-container">
      <h1>Relatório Consolidado Diário</h1>

      <form className="input-group" onSubmit={gerarRelatorio}>
        <input
          type="date"
          value={dataInicio}
          onChange={(e) => setDataInicio(e.target.value)}
          required
        />
        <input
          type="date"
          value={dataFim}
          onChange={(e) => setDataFim(e.target.value)}
          required
        />
        <button type="submit" id="gerar-relatorio">
          Gerar Relatório
        </button>
      </form>

      <div className="export-buttons">
        <button onClick={exportarPDF}>Exportar PDF</button>
        <button onClick={exportarExcel}>Exportar Excel</button>
        <button onClick={exportarMarkdown}>Exportar Markdown</button>
      </div>

      <table className="relatorio-tabela">
        <thead>
          <tr>
            <th>Data</th>
            <th>Total Débitos</th>
            <th>Total Créditos</th>
            <th>Saldo</th>
          </tr>
        </thead>
        <tbody>
          {relatorio.map((item, index) => (
            <tr key={index}>
              <td>{item.data}</td>
              <td>{item.totalDebitos.toFixed(2)}</td>
              <td>{item.totalCreditos.toFixed(2)}</td>
              <td>{item.saldo.toFixed(2)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default RelatorioConsolidado;
