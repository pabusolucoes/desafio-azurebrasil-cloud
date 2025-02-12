# 📄 Documentação do Front-End - Fluxo de Caixa

## 📝 **Visão Geral**

O front-end do **Fluxo de Caixa** é uma aplicação desenvolvida em **React (Vite)**, responsável por consumir os microserviços do sistema. Ele realiza autenticação, integração com as APIs e armazena a API Key de forma segura para autenticar requisições aos microserviços.

## 📌 **Principais Funcionalidades**

### 🔹 **Autenticação e Armazenamento Seguro da API Key**

- O usuário realiza login e recebe um **token JWT** contendo a **API Key**.
- O token é decodificado usando **jwt-decode** e a **API Key** é extraída.
- A **API Key é criptografada** com **AES** (`crypto-js`) antes de ser armazenada no `localStorage`.
- O `username` retornado no login é armazenado como `contaId`.

### 🔹 **Interceptadores Axios para Comunicação Segura**

- O front-end **não acessa a API de Integrações diretamente**.
- Todas as chamadas para os microserviços **Lançamentos e Consolidado Diário** incluem a **API Key** automaticamente no cabeçalho.
- Foi criado um **interceptor Axios** para garantir que a **API Key** seja sempre enviada:

```javascript
import axios from "axios";
import { API_FLUXO_CAIXA } from "../config";
import { getSecureItem } from "../services/storageHelper";

const apiFluxoCaixa = axios.create({
  baseURL: API_FLUXO_CAIXA,
});

apiFluxoCaixa.interceptors.request.use((config) => {
  const apiKey = getSecureItem("apiKey");
  const contaId = getSecureItem("contaId");

  if (apiKey) {
    config.headers["x-api-key"] = apiKey;
  }
  if (contaId) {
    config.headers["x-conta-id"] = contaId;
  }
  return config;
});

export default apiFluxoCaixa;
```

### 🔹 **Rotas e Navegação**

- Implementamos um **breadcrumb (trilha de navegação)** para melhorar a usabilidade.
- O **botão de logout** remove as credenciais do usuário e o redireciona para a tela de login.
- As rotas foram estruturadas corretamente no `App.jsx`:

```javascript
<Routes>
  <Route path="/dashboard" element={isAuthenticated ? <Dashboard /> : <Navigate to="/login" />} />
  <Route path="/relatorio" element={isAuthenticated ? <RelatorioConsolidado /> : <Navigate to="/login" />} />
  <Route path="/login" element={<Login />} />
  <Route path="*" element={<Navigate to="/dashboard" />} />
</Routes>
```

### 🔹 **Relatório Consolidado e Lançamentos**

- O `contaId` do usuário é utilizado para buscar os lançamentos corretamente.
- O saldo total é exibido no relatório e inclui um **ícone de informação ℹ️** com tooltip explicativo.
- O front-end valida que a **data inicial não pode ser maior que a final** ao gerar relatórios.

### 🔹 **Criptografia da API Key no `localStorage`**

A API Key é armazenada de forma segura no `localStorage` utilizando **AES**:

```javascript
import CryptoJS from "crypto-js";

const SECRET_KEY = import.meta.env.VITE_SECRET_KEY || "chave-padrao";

export const setSecureItem = (key, value) => {
  const encryptedValue = CryptoJS.AES.encrypt(value, SECRET_KEY).toString();
  localStorage.setItem(key, encryptedValue);
};

export const getSecureItem = (key) => {
  const encryptedValue = localStorage.getItem(key);
  if (!encryptedValue) return null;

  try {
    const bytes = CryptoJS.AES.decrypt(encryptedValue, SECRET_KEY);
    return bytes.toString(CryptoJS.enc.Utf8);
  } catch (error) {
    console.error("Erro ao descriptografar:", error);
    return null;
  }
};
```

### 🔹 **Logout Seguro**

Ao sair, todas as credenciais são removidas do `localStorage`:

```javascript
const logout = () => {
  removeSecureItem("apiKey");
  removeSecureItem("contaId");
  navigate("/login");
};
```

## ✅ **Conclusão**

- O **front-end está completamente integrado aos microserviços**, garantindo segurança e eficiência.
- **A API Key é armazenada e utilizada de forma segura** em todas as requisições.
- **As funcionalidades implementadas seguem boas práticas de autenticação e segurança.**
- **O código está modularizado, bem estruturado e pronto para manutenção e evolução.**