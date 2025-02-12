import "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Login from "./components/login";
import Dashboard from "./components/dashboard";
import RelatorioConsolidado from "./components/relatorioConsolidado";
import Register from "./components/register";
function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/relatorio" element={<RelatorioConsolidado />} />
        <Route path="/register" element={<Register />} /> {/* 🔹 Nova rota para o registro */}

      </Routes>
    </Router>
  );
}

export default App;
