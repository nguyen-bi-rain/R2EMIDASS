import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
    [key: string]: string;
}

export function decodeAndExtractUserId(token: string): object | null {
    try {
        const decoded: JwtPayload = jwtDecode(token);
        const response = {
            userId : decoded["sid"],
            role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
        }
        return response;
    } catch (error) {
        console.error('Invalid token:', error);
        return null;
    }
}