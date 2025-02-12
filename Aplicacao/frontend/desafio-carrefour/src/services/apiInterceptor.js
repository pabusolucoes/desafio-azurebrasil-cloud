import axios from "axios";

const api = axios.create();

api.interceptors.request.use(
  (config) => {
    const apiKey = localStorage.getItem("apiKey");
    if (apiKey) {
      config.headers["x-api-key"] = apiKey; // 🔹 Inclui a API Key no cabeçalho
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default api;
