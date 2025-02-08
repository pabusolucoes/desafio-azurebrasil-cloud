import  { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import "../styles/login.css"; // Estilos centralizados na pasta styles

function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();

    // Mock de autenticação
    const mockAuthenticate = (username, password) => {
        const MOCK_USER = "admin";
        const MOCK_PASSWORD = "123456";
    
        return username === MOCK_USER && password === MOCK_PASSWORD;
      };

  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const response = await api.post("/login", { username, password });
      localStorage.setItem("token", response.data.token);
      navigate("/dashboard");
    } catch {
        console.warn("API indisponível, usando mock para autenticação.");
        if (mockAuthenticate(username, password)) {
            localStorage.setItem("token", "mock_token");
            navigate("/dashboard");
        } else {
            alert("Usuário ou senha incorretos.");
        }
    }
  };

  return (
    <div className="login-page">
      <div className="login-container">
        <div className="login-header">
          <img
            src="/src/assets/logo.png" /* Adicione uma logo aqui */
            alt="Logo"
            className="login-logo"
          />
          <h2 className="login-title">Bem-vindo</h2>
          <p className="login-subtitle">Acesse o painel com suas credenciais</p>
        </div>
        <form onSubmit={handleLogin}>
          <div className="form-group">
            <label htmlFor="username">Usuário</label>
            <input
              type="text"
              id="username"
              className="form-control"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              placeholder="Digite seu usuário"
            />
          </div>
          <div className="form-group">
            <label htmlFor="password">Senha</label>
            <input
              type="password"
              id="password"
              className="form-control"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              placeholder="Digite sua senha"
            />
          </div>
          <button type="submit" className="btn btn-primary btn-block">
            Entrar
          </button>
        </form>
      </div>
    </div>
  );
}

export default Login;
