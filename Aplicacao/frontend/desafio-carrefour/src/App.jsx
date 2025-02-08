import "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Login from "./components/login";
import Dashboard from "./components/dashboard";
import RelatorioConsolidado from "./components/relatorioConsolidado";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/relatorio" element={<RelatorioConsolidado />} />
      </Routes>
    </Router>
  );
}

export default App;
