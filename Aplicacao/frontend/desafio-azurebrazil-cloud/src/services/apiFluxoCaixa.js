import axios from "axios";
import { API_FLUXO_CAIXA } from "../config";
import { getSecureItem } from "../services/storageHelper";

const apiFluxoCaixa = axios.create({
  baseURL: API_FLUXO_CAIXA,
});

apiFluxoCaixa.interceptors.request.use((config) => {
    const apiKey = getSecureItem("apiKey"); // 🔹 Obtém a API Key descriptografada

  if (apiKey) {
    config.headers["x-api-key"] = apiKey; // 🔹 Adiciona a API Key no cabeçalho
  }

  return config;
});

export default apiFluxoCaixa;
