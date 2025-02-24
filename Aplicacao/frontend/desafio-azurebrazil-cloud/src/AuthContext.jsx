import { createContext, useContext, useState, useEffect } from 'react';
import PropTypes from 'prop-types'; // Importando prop-types
import { PublicClientApplication } from '@azure/msal-browser';



// Configuração do MSAL
const msalConfig = {
  auth: {
    clientId: '11ae38c2-e53b-4316-93be-2342b70d9973',
    authority: 'https://login.microsoftonline.com/6c476540-4831-46d5-98dc-a47b3a54828d',
    redirectUri: 'https://localhost:5173/', // O mesmo redirectUri configurado no Azure AD
  },
};

const msalInstance = new PublicClientApplication(msalConfig);
msalInstance.initialize();
// Criar o contexto de autenticação
const AuthContext = createContext();

// Criar o provider do contexto
export const AuthProvider = ({ children }) => {
  const [account, setAccount] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    // Verificar se o usuário já está autenticado
    const currentAccount = msalInstance.getAllAccounts()[0];
    if (currentAccount) {
      setAccount(currentAccount);
      setIsAuthenticated(true);
    }
  }, []);

  // Função de login
  const login = async () => {
    try {
      await msalInstance.loginPopup({ scopes: ['openid', 'profile', 'email'] });
      const currentAccount = msalInstance.getAllAccounts()[0];
      setAccount(currentAccount);
      setIsAuthenticated(true);
      window.location.href = 'https://localhost:5173/dashboard';
    } catch (error) {
      console.error("Login failed", error);
    }
  };

  // Função de logout
  const logout = () => {
    msalInstance.logoutPopup().then(() => {
      setAccount(null);
      setIsAuthenticated(false);
      window.location.href = 'https://localhost:5173/'; // Redirecionar para a página de login
    }).catch(error => {
      console.error('Logout failed', error);
    });
  };

  return (
    <AuthContext.Provider value={{ account, isAuthenticated, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

// Validação das props, incluindo 'children'
AuthProvider.propTypes = {
    children: PropTypes.node.isRequired, // Aqui estamos validando que 'children' deve ser passado e ser do tipo 'node'
  };
// Hook para acessar o contexto de autenticação
export const useAuth = () => {
  return useContext(AuthContext);
};
