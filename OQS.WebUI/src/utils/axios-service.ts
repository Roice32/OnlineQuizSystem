import axios from "axios";
import { config } from "../config";
import { userMock } from "./mocks/userMock";



const axiosService = axios.create({
    baseURL: config.backendURL,
    timeout: 15000,
    withCredentials: true
});

axiosService.interceptors.request.use((request) => {
    const token = document.cookie.split(';').find((cookie) => cookie.includes('token'))?.split('=')[1];
   /*  console.log(token); */
    if(request.headers['Authorization'] === undefined && token !== undefined){
      /*   console.log('setting token'); */
        request.headers['Authorization'] =  `Bearer ${token}`;
    }
    return request;
});

export default axiosService;