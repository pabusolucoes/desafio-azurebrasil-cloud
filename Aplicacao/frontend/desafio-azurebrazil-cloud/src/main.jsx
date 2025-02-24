import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { AuthProvider } from './AuthContext.jsx'; // Importe o AuthProvider
import "bootstrap/dist/css/bootstrap.min.css";
import './styles/index.css'
import App from './App.jsx'

createRoot(document.getElementById('root')).render(
  <AuthProvider>
  <StrictMode>
    <App />
  </StrictMode>
  </AuthProvider>,
  document.getElementById('root')
)
