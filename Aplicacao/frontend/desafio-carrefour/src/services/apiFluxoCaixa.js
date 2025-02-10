import axios from "axios";
import { API_FLUXO_CAIXA } from "../config";

const apiFluxoCaixa = axios.create({
  baseURL: API_FLUXO_CAIXA,
});

apiFluxoCaixa.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default apiFluxoCaixa;
