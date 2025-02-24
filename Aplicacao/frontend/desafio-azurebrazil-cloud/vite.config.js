import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";

export default defineConfig({
  plugins: [react()],
  server: {
    host: "0.0.0.0",  // Permite acesso externo ao servidor Vite
    port: 5173,        // Porta padrão
    https: {
      key: './certificados/localhost-key.pem',    // Caminho para a chave privada SSL
      cert: './certificados/localhost.pem',       // Caminho para o certificado SSL
    },
  },
  resolve: {
    alias: {
      "@": "/src", // Alias para o diretório src
    },
  },
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          react: ["react", "react-dom"],
          pdf: ["jspdf", "jspdf-autotable"],
          excel: ["xlsx", "file-saver"],
        },
      },
    },
  },
});
