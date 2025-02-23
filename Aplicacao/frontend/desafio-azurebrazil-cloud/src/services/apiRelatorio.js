import axios from "axios";
import { API_RELATORIO } from "../config";
import { getSecureItem } from "../services/storageHelper";

const apiRelatorio = axios.create({
  baseURL: API_RELATORIO,
});

apiRelatorio.interceptors.request.use((config) => {
    const apiKey = getSecureItem("apiKey"); // 🔹 Obtém a API Key descriptografada

  if (apiKey) {
    config.headers["x-api-key"] = apiKey;
  }

  return config;
});

export default apiRelatorio;
