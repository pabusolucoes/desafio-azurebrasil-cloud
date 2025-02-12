
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import apiLogin from "../services/apiLogin";
import "../styles/register.css";

function Register() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const handleRegister = async (e) => {
    e.preventDefault();

    if (password !== confirmPassword) {
      setError("As senhas não coincidem.");
      return;
    }

    try {
      await apiLogin.post("/auth/criar", { username, password });
      alert("Registro realizado com sucesso! Faça login para continuar.");
      navigate("/");
    } catch (error) {
      setError(`Erro ao registrar: ${error.message}. Tente novamente.`);
    }
  };

  return (
    <div className="register-page">
      <div className="register-container">
        <h2>Registro de Usuário</h2>
        {error && <p className="error-message">{error}</p>}
        <form onSubmit={handleRegister}>
          <input
            type="text"
            placeholder="Usuário"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
          <input
            type="password"
            placeholder="Senha"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
          <input
            type="password"
            placeholder="Confirme sua senha"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            required
          />
          <button type="submit" className="btn btn-primary">Registrar</button>
        </form>
        <p>Já tem uma conta? <a href="/">Faça login</a></p>
      </div>
    </div>
  );
}

export default Register;
