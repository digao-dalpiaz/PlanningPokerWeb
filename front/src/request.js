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
    //console.log(error);
    toast.error(/*error.response?.data ??*/ error.message)
    return Promise.reject(error);
  }
);