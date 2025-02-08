import  { useState, useEffect, useCallback, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import TabelaLancamentos from "../components/tabelaLancamentos";
import "../styles/dashboard.css";

function Dashboard() {
  const navigate = useNavigate();

  const [lancamentos, setLancamentos] = useState([]);
  const [filteredLancamentos, setFilteredLancamentos] = useState([]);
  const [sortConfig, setSortConfig] = useState({ key: null, direction: "asc" });
  const [filter, setFilter] = useState("");

  const [form, setForm] = useState({
    id: null,
    data: "",
    valor: "",
    descricao: "",
    tipo: "CRÉDITO",
    categoria: "",
  });

  // Mock de lançamentos (para teste sem API)
  const mockLancamentos = useMemo(() => [
    { id: 1, data: "2025-02-01", valor: 100.5, descricao: "Compra supermercado", tipo: "DÉBITO", categoria: "Alimentação" },
    { id: 2, data: "2025-02-01", valor: 20.0, descricao: "Padaria", tipo: "DÉBITO", categoria: "Alimentação" },
    { id: 3, data: "2025-02-01", valor: 50.0, descricao: "Gasolina", tipo: "DÉBITO", categoria: "Transporte" },
    { id: 4, data: "2025-02-02", valor: 200.0, descricao: "Salário", tipo: "CRÉDITO", categoria: "Renda" },
    { id: 5, data: "2025-02-03", valor: 120.0, descricao: "Aluguel", tipo: "DÉBITO", categoria: "Moradia" },
    { id: 6, data: "2025-02-03", valor: 45.0, descricao: "Farmácia", tipo: "DÉBITO", categoria: "Saúde" },
    { id: 7, data: "2025-02-04", valor: 300.0, descricao: "Consultoria", tipo: "CRÉDITO", categoria: "Renda Extra" },
    { id: 8, data: "2025-02-05", valor: 80.0, descricao: "Restaurante", tipo: "DÉBITO", categoria: "Entretenimento" },
    { id: 9, data: "2025-02-05", valor: 35.0, descricao: "Cinema", tipo: "DÉBITO", categoria: "Entretenimento" },
    { id: 10, data: "2025-02-06", valor: 25.0, descricao: "Café", tipo: "DÉBITO", categoria: "Alimentação" },
    { id: 11, data: "2025-02-06", valor: 70.0, descricao: "Uber", tipo: "DÉBITO", categoria: "Transporte" },
    { id: 12, data: "2025-02-07", valor: 150.0, descricao: "Mercado", tipo: "DÉBITO", categoria: "Alimentação" },
    { id: 13, data: "2025-02-07", valor: 50.0, descricao: "Oficina", tipo: "DÉBITO", categoria: "Manutenção" },
    { id: 14, data: "2025-02-08", valor: 40.0, descricao: "Livraria", tipo: "DÉBITO", categoria: "Educação" },
    { id: 15, data: "2025-02-09", valor: 500.0, descricao: "Venda online", tipo: "CRÉDITO", categoria: "Renda Extra" },
  ],[]);

  // Evita re-renderizações desnecessárias
  const fetchLancamentos = useCallback(async () => {
    try {
      const response = await api.get("/lancamentos");
      setLancamentos(response.data);
      setFilteredLancamentos(response.data);
    } catch {
      console.warn("API indisponível, usando mock.");
      setLancamentos(mockLancamentos);
      setFilteredLancamentos(mockLancamentos);
    }
  }, [mockLancamentos]);

  useEffect(() => {
    fetchLancamentos();
  }, [fetchLancamentos]);

  // Apenas atualiza a lista filtrada sem re-renderizar o dashboard inteiro
  useEffect(() => {
    setFilteredLancamentos(
      filter
        ? lancamentos.filter((lanc) => lanc.tipo === filter || lanc.categoria === filter)
        : lancamentos
    );
  }, [filter, lancamentos]);

  const handleSort = (key) => {
    let direction = "asc";
    if (sortConfig.key === key && sortConfig.direction === "asc") {
      direction = "desc";
    }
    setSortConfig({ key, direction });

    setFilteredLancamentos((prev) =>
      [...prev].sort((a, b) => {
        if (a[key] < b[key]) return direction === "asc" ? -1 : 1;
        if (a[key] > b[key]) return direction === "asc" ? 1 : -1;
        return 0;
      })
    );
  };

  return (
    <div className="dashboard-container">
      <h1>Dashboard</h1>
      <button
        className="btn btn-secondary"
        onClick={() => navigate("/relatorio")}
      >
        📊 Relatório Consolidado
      </button>
      {/* Filtro */}
      <div className="filter-container">
        <label>Filtrar por:</label>
        <select onChange={(e) => setFilter(e.target.value)} value={filter}>
          <option value="">Todos</option>
          <option value="CRÉDITO">Crédito</option>
          <option value="DÉBITO">Débito</option>
          <option value="Alimentação">Alimentação</option>
          <option value="Renda">Renda</option>
          <option value="Transporte">Transporte</option>
        </select>
        {filter && <span className="filter-tag">{filter} ✖</span>}
      </div>

      {/* Formulário de lançamentos */}
      <form className="form-lancamento" onSubmit={fetchLancamentos}>
        <input type="date" value={form.data} onChange={(e) => setForm({ ...form, data: e.target.value })} required />
        <input type="number" value={form.valor} onChange={(e) => setForm({ ...form, valor: e.target.value })} required placeholder="Valor" />
        <input type="text" value={form.descricao} onChange={(e) => setForm({ ...form, descricao: e.target.value })} required placeholder="Descrição" />
        <select value={form.tipo} onChange={(e) => setForm({ ...form, tipo: e.target.value })}>
          <option value="CRÉDITO">CRÉDITO</option>
          <option value="DÉBITO">DÉBITO</option>
        </select>
        <input type="text" value={form.categoria} onChange={(e) => setForm({ ...form, categoria: e.target.value })} required placeholder="Categoria" />
        <button type="submit" className="btn btn-primary">Salvar</button>
      </form>

      {/* Grade de lançamentos separada */}
      <TabelaLancamentos
        lancamentos={filteredLancamentos}
        handleSort={handleSort}
        sortConfig={sortConfig}
        handleEdit={setForm}
        handleDelete={fetchLancamentos}
      />
    </div>
  );
}

export default Dashboard;
