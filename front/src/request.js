import axios from "axios";
import { URL_BACKEND } from "./definicoes";
import { toast } from "react-toastify";

export const api = axios.create({
  baseURL: URL_BACKEND
})

api.interceptors.response.use(
  response => {
    return response;
  },
  error => {
    let msg;
    switch (error.status) {
      case 500:
        msg = 'Erro: ' + error.response.data;
        break;
      default:
        msg = `Erro ${error.status}: ${error.message}`;
    }
    toast.error(msg);
    return Promise.reject(error);
  }
);