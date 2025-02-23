import axios from "axios";
import { API_AUTENTICACAO  } from "../config";

const apiLogin = axios.create({
  baseURL: API_AUTENTICACAO,
});

apiLogin.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default apiLogin;
