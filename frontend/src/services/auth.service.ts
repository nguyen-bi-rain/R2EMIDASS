import { axiosAuthentication } from "../api/axiosAuthenInstance";
import axiosInstance from "../api/axiosInstance";
import { ENDPOINS } from "../constants";
import { LoginRequest, RegisterRequest } from "../models/authType";


class AuthService {
    async login(data: LoginRequest) {
        try {
            const response = await axiosInstance.post(`${ENDPOINS.auth.login}`, data);

            return response.data;
        }
        catch (error) {
            console.error("Login error:", error);
            throw error;
        }
    }
    async register(data: RegisterRequest) {
        try {
            const response = await axiosInstance.post(`${ENDPOINS.auth.register}`, data);
            return response.data;
        }
        catch (error) {
            console.error("Register error:", error);
            throw error;
        }
    }
    logout() {
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
    }
    async refreshToken(refreshToken: string, accessToken: string) {
        try {
            const response = await axiosAuthentication.post("/Auth/refresh-token", {
                refreshToken: refreshToken,
                accessToken: accessToken,
            });
            return response.data;
        }
        catch (error) {
            console.error("Refresh token error:", error);
            throw error;
        }
    }
}

export default new AuthService();