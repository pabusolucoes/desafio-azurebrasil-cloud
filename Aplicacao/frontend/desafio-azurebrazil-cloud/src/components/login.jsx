import { useState, useEffect } from "react";
import { setSecureItem } from "../services/storageHelper";
import { useAuth } from "../AuthContext"; // Importa o contexto de autenticação
import "../styles/login.css";

function Login() {
  const [error, setError] = useState(""); // Gerencia o erro
  const { login, isAuthenticated } = useAuth(); // Acesso ao login e estado de autenticação
  

  // Função para lidar com o redirecionamento após a autenticação
  const handleRedirectResponse = async () => {
    try {
      const response = await msalInstance.handleRedirectPromise(); // Obtém a resposta do redirect

      if (response) {
        const { account, accessToken } = response;

        if (accessToken) {
          // Armazenar o token e o contaId no localStorage de forma segura
          setSecureItem("apiKey", accessToken);
          setSecureItem("contaId", account.username); // Salva o username como contaId
        }
      }
    } catch (error) {
      console.warn("Erro de autenticação:", error);
      setError("Falha na autenticação. Tente novamente."); // Exibe a mensagem de erro para o usuário
    }
  };

  // Verifica se houve resposta após o redirecionamento ao carregar a página
  useEffect(() => {
    if (!isAuthenticated) {
      handleRedirectResponse();
    }
     // Verifica se a autenticação foi realizada ao retornar do login
  }, [isAuthenticated]);

  if (isAuthenticated) {
    return <div>Você já está logado!</div>;
    
  }

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

        <form onSubmit={(e) => { e.preventDefault(); login(); }}>
          <div className="form-group">
            <label htmlFor="username">Usuário</label>
            <input
              type="text"
              id="username"
              className="form-control"
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
              required
              placeholder="Digite sua senha"
            />
          </div>

          <button type="submit" className="btn btn-primary btn-block">
            Entrar
          </button>
        </form>

        {error && <p className="error-message">{error}</p>}

        <p>
          Não tem uma conta? <a href="/register">Registre-se aqui</a>
        </p>
      </div>
    </div>
  );
}

export default Login;
