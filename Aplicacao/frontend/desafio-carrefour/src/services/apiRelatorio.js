import axios from "axios";
import { API_RELATORIO } from "../config";

const apiRelatorio = axios.create({
  baseURL: API_RELATORIO,
});

apiRelatorio.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default apiRelatorio;
