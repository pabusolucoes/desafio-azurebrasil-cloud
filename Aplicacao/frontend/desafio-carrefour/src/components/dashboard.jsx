import { useState, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { Lancamento } from "../models/Lancamento.js";
import apiFluxoCaixa from "../services/apiFluxoCaixa";
import TabelaLancamentos from "../components/tabelaLancamentos";

import "../styles/dashboard.css";

function Dashboard() {
  const navigate = useNavigate();

  const logout = () => {
    localStorage.removeItem("username"); // 🔹 Remove o usuário do localStorage
    navigate("/"); // 🔹 Redireciona para a tela de login
  };
  const contaId = localStorage.getItem("username"); //Obtém o contaId do usuário logado

  const [filteredLancamentos, setFilteredLancamentos] = useState([]);
  const [sortConfig, setSortConfig] = useState({ key: null, direction: "asc" });

  const [form, setForm] = useState(new Lancamento(contaId));

  /**Busca os lançamentos da conta do usuário */
  const fetchLancamentos = useCallback(async () => {
    if (!contaId) return;
    try {
      const response = await apiFluxoCaixa.get(`/fluxo-caixa/lancamentos/${contaId}`);
  
      // Garante que cada lançamento tenha um ID correto
      const dataCorrigida = response.data.map(lanc => ({
        ...lanc,
        lancamentoId: lanc.lancamentoId || crypto.randomUUID(), // Se não tiver ID, gera um temporário
      }));
  
      setFilteredLancamentos(dataCorrigida);
    } catch (error) {
      console.error("Erro ao buscar lançamentos:", error);
    }
  }, [contaId]);

  useEffect(() => {
    fetchLancamentos();
  }, [fetchLancamentos]);

  /**Criação de um novo lançamento */
  const criarLancamento = async (e) => {
    e.preventDefault();
    try {
      const { lancamentoId, ...dadosLancamento } = form; //Remove o `lancamentoId`
  
      await apiFluxoCaixa.post("/fluxo-caixa/lancamentos", {
        ...dadosLancamento, // Envia apenas os dados sem `lancamentoId`
        contaId, //Garante que o contaId seja incluído
      });
  
      setTimeout(() => {fetchLancamentos();}, 500);  // Atualiza a tabela após a inclusão
      
      setForm(new Lancamento(contaId)); // Reseta o formulário mantendo o contaId
    } catch (error) {
      console.error("Erro ao criar lançamento:", error);
    }
  };

  /**Atualização de um lançamento */
const atualizarLancamento = async (e) => {
  e.preventDefault();
  if (!form.lancamentoId) return;

  try {
    // Garante que a data seja enviada no formato correto (YYYY-MM-DD)
    const dataFormatada = form.data.split("T")[0];

    await apiFluxoCaixa.put(`/fluxo-caixa/lancamentos?lancamentoId=${form.lancamentoId}`, { 
      ...form, 
      data: dataFormatada, //Converte antes de enviar
      contaId 
    });

    setTimeout(() => {fetchLancamentos();}, 500); //Atualiza a grade após o update

    setForm(new Lancamento(contaId)); //Reseta o formulário mantendo contaId
  } catch (error) {
    console.error("Erro ao atualizar lançamento:", error);
  }
};

  /** Exclusão de um lançamento */
  const excluirLancamento = async (id) => {
    if (window.confirm("Tem certeza que deseja excluir este lançamento?")) {
      try {
        await apiFluxoCaixa.delete(`/fluxo-caixa/lancamentos/${contaId}/${id}`);
        setTimeout(() => {fetchLancamentos();}, 500);
      } catch (error) {
        console.error("Erro ao excluir lançamento:", error);
      }
    }
  };

  /**Editar lançamento */
  const editarLancamento = (lanc) => {
    const dataFormatada = lanc.data.split("T")[0]; //Converte para "YYYY-MM-DD"
  
    setForm({
      ...lanc,
      data: dataFormatada, //Agora o input de data aceita o valor correto
    });
  };

  return (
    <div className="dashboard-container">
      <div className="dashboard-header">
        <h1>Dashboard</h1>
        <button className="btn btn-danger logout-button" onClick={logout}>
          🚪 Sair
        </button>
      </div>      
      <button className="btn btn-secondary" onClick={() => navigate("/relatorio")}>
        📊 Relatório Consolidado
      </button>

      {/*Formulário de lançamentos */}
      <form className="form-lancamento" onSubmit={form.lancamentoId ? atualizarLancamento : criarLancamento}>
        <input
          type="date"
          value={form.data}
          onChange={(e) => setForm({ ...form, data: e.target.value })}
          required
        />
        <input
          type="number"
          value={form.valor}
          onChange={(e) => {
            let valor = e.target.value.replace(",", "."); // Substitui ',' por '.'
            valor = parseFloat(valor);
            
            if (isNaN(valor) || valor < 0) valor = 0; // Bloqueia negativos
            setForm({ ...form, valor });
          }}
          step="0.01" // Permite casas decimais
          min="0" // Bloqueia valores negativos
          required
          placeholder="Valor"
        />
        <input
          type="text"
          value={form.descricao}
          onChange={(e) => setForm({ ...form, descricao: e.target.value })}
          required
          placeholder="Descrição"
        />
        <select value={form.tipo} onChange={(e) => setForm({ ...form, tipo: e.target.value })}>
          <option value="CRÉDITO">CRÉDITO</option>
          <option value="DÉBITO">DÉBITO</option>
        </select>
        <input
          type="text"
          value={form.categoria}
          onChange={(e) => setForm({ ...form, categoria: e.target.value })}
          required
          placeholder="Categoria"
        />
        <button type="submit" className="btn btn-primary">
          {form.lancamentoId ? "Atualizar" : "Salvar"}
        </button>
        {form.lancamentoId && (
          <button type="button" className="btn btn-warning" onClick={() => setForm(new Lancamento(contaId))}>
            Cancelar Edição
          </button>
        )}
      </form>

      {/*Grade de lançamentos */}
      <TabelaLancamentos
        lancamentos={filteredLancamentos}
        handleSort={(key) => {
          const direction = sortConfig.key === key && sortConfig.direction === "asc" ? "desc" : "asc";
          setSortConfig({ key, direction });
          setFilteredLancamentos((prev) =>
            [...prev].sort((a, b) => (a[key] < b[key] ? (direction === "asc" ? -1 : 1) : a[key] > b[key] ? (direction === "asc" ? 1 : -1) : 0))
          );
        }}
        sortConfig={sortConfig}
        handleEdit={editarLancamento}
        handleDelete={excluirLancamento}
      />
    </div>
  );
}

export default Dashboard;
