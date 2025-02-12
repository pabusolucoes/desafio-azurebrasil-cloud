import CryptoJS from "crypto-js";

const SECRET_KEY = import.meta.env.VITE_SECRET_KEY || "chave-padrao";; // 🔹 Altere para uma chave segura

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

export const removeSecureItem = (key) => {
  localStorage.removeItem(key);
};
