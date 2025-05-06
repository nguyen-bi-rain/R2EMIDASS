import axios from "axios";

export const axiosAuthentication = axios.create({
    baseURL: import.meta.env.VITE_BASE_URL || "http://localhost:5171/api",
    headers: {
        "Content-Type": "application/json",
    },
    timeout: 10_000,
    validateStatus: (status) => status < 400,
});

axiosAuthentication.interceptors.request.use((config) => {
    const token = localStorage.getItem("accessToken") || "{}"
    // if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

axiosAuthentication.interceptors.response.use((response) => {
    return response.data?.data;
});

axiosAuthentication.interceptors.response.use(
    (response) => {
        return response;
    },
    async (error) => {
        if (error.response.status === 401) {
            window.location.href = "/login";
        }
        return Promise.reject(error);
    }
);