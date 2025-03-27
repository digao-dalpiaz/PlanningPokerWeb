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
    if (error.code === 'ERR_NETWORK') {
      msg = 'Não foi possível se comunicar com o servidor'
    } else {
      switch (error.status) {
        case 422:
          msg = error.response.data;
          break;
        case 500:
          msg = 'Erro: ' + error.response.data;
          break;
        default:
          msg = 'Erro ' + (error.status ? '#' + error.status : error.code) + ': ' + error.message;
      }
    }
    toast.error(msg);
    return Promise.reject(error);
  }
);