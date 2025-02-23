import { useState } from "react";
import { useNavigate } from "react-router-dom";
import apiRelatorio from "../services/apiRelatorio";
import { getSecureItem } from "../services/storageHelper";
import "../styles/relatorio.css";
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";
import * as XLSX from "xlsx";
import { saveAs } from "file-saver";

function RelatorioConsolidado() {

  const navigate = useNavigate(); // Para navegação
  const [dataInicial, setDataInicio] = useState("");
  const [dataFinal, setDataFim] = useState("");
  const [relatorio, setRelatorio] = useState([]);
  const [saldoTotal, setSaldoTotal] = useState(0); // 🔹 Novo estado para o saldo total
  const [paginaAtual, setPaginaAtual] = useState(1);
  const [erro, setErro] = useState(""); // 🔹 Novo estado para mensagens de erro
  const itensPorPagina = 10; // 🔹 Define quantos registros mostrar por página
  const contaId = getSecureItem("contaId"); //Obtém o contaId do usuário logado

  const gerarRelatorio = async (e) => {
    e.preventDefault();

        // 🔹 Validação: Data inicial não pode ser maior que a data final
        if (new Date(dataInicial) > new Date(dataFinal)) {
          setErro("A data inicial não pode ser maior que a data final.");
          return;
        }
    
        setErro(""); // Limpa a mensagem de erro caso as datas sejam válidas
    try {
      const response = await apiRelatorio.get("/consolidado-diario", {
        params: { dataInicial, dataFinal, contaId},
      });
      setRelatorio(response.data);
      setSaldoTotal(
        response.data.reduce((total, item) => total + parseFloat(item.saldo), 0) // 🔹 Calcula o saldo total
      );
      setPaginaAtual(1); // 🔹 Resetar para a primeira página
    } catch {
      console.warn("API indisponível, usando mock.");
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

  /** 🔹 Paginação */
  const indexUltimoItem = paginaAtual * itensPorPagina;
  const indexPrimeiroItem = indexUltimoItem - itensPorPagina;
  const dadosPaginados = relatorio.slice(indexPrimeiroItem, indexUltimoItem);
  const totalPaginas = Math.ceil(relatorio.length / itensPorPagina);

  return (
    



    <div className="relatorio-container">
      {/* 🔹 Breadcrumb para navegação */}
      <nav className="breadcrumb">
        <span className="breadcrumb-item" onClick={() => navigate("/dashboard")}>🏠 Dashboard</span>
        <span className="breadcrumb-separator"> / </span>
        <span className="breadcrumb-item active">📊 Relatório Consolidado</span>
      </nav>      
      <h1>Relatório Consolidado Diário</h1>

      <form className="input-group" onSubmit={gerarRelatorio}>
        <input type="date" value={dataInicial} onChange={(e) => setDataInicio(e.target.value)} required />
        <input type="date" value={dataFinal} onChange={(e) => setDataFim(e.target.value)} required />
        <button type="submit" id="gerar-relatorio">Gerar Relatório</button>
      </form>
      {/* 🔹 Exibe a mensagem de erro, se houver */}
      {erro && <p className="error-message">{erro}</p>}
      <div className="export-buttons">
        <button onClick={exportarPDF}>Exportar PDF</button>
        <button onClick={exportarExcel}>Exportar Excel</button>
        <button onClick={exportarMarkdown}>Exportar Markdown</button>
      </div>
      <div className="saldo-container">
        <strong>Saldo Total: </strong>
        <span className="saldo-total">
          R$ {saldoTotal.toFixed(2)}
          <span className="info-icon" title="O saldo total considera apenas os consolidados do período selecionado.">
            ℹ️
          </span>
        </span>
      </div>

      {/* 🔹 Container Responsivo para a Tabela */}
      <div className="tabela-container">
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
            {dadosPaginados.map((item, index) => (
              <tr key={index}>
                <td>{new Date(item.data).toLocaleDateString("pt-BR")}</td>
                <td>R$ {item.totalDebitos.toFixed(2)}</td>
                <td>R$ {item.totalCreditos.toFixed(2)}</td>
                <td>R$ {item.saldo.toFixed(2)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* 🔹 Paginação */}
      {totalPaginas > 1 && (
        <div className="paginacao">
          <button disabled={paginaAtual === 1} onClick={() => setPaginaAtual(paginaAtual - 1)}>
            ◀ Anterior
          </button>
          <span>Página {paginaAtual} de {totalPaginas}</span>
          <button disabled={paginaAtual === totalPaginas} onClick={() => setPaginaAtual(paginaAtual + 1)}>
            Próxima ▶
          </button>
        </div>
      )}
    </div>
  );
}

export default RelatorioConsolidado;
