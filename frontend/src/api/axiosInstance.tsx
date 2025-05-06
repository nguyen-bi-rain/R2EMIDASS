import axios from "axios";
import { BASE_API } from "./config";
import authService from "../services/auth.service";

const axiosInstance = axios.create(BASE_API);

axiosInstance.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem("accessToken");
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

axiosInstance.interceptors.response.use(
    (response) => {
        return response
    },
    async (error) => {
        if (error.response.status === 401) {
            const refreshToken = localStorage.getItem("refreshToken");
            const accessToken = localStorage.getItem("accessToken");
            if (!refreshToken) {
                return Promise.reject(error);
            }
            try {
                if (refreshToken && accessToken) {
                    await authService.refreshToken(refreshToken, accessToken).then((res) => {
                        if (res) {
                            localStorage.setItem("accessToken", res.accessToken);
                            localStorage.setItem("refreshToken", res.refreshToken);
                        }
                        window.location.reload();
                    });
                }
            } catch(error) {
                console.error("Error refreshing token:", error);
                return Promise.reject(error);
            }
            // window.location.href = "/login";
        }
        return Promise.reject(error);
    }
);

export default axiosInstance;