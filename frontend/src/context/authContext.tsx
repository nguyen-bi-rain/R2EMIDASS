import React, { createContext, useContext, useState, ReactNode, useEffect } from 'react';
import { decodeAndExtractUserId } from '../utils/jwt-decode';

interface AuthContextType {
    isAuthenticated: boolean;
    role: string | null;
    user: string | null;
    setIsAuthenticated: (value: boolean) => void;
    setRole: (role: string | null) => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false); // Default to true for testing purposes
    const [role, setRole] = useState<string | null>(null);
    const [user, setUser] = useState<string | null>(null);
    useEffect(() => {
        const token = localStorage.getItem('accessToken');
        const decodedToken = decodeAndExtractUserId(token as string) as { role: string | null; userId: string | null };
        console.log('Decoded Token:', decodedToken);
        
        if (token) {
            setIsAuthenticated(true);
            if (decodedToken) {
                setRole(decodedToken.role);
                setUser(decodedToken.userId);
            }
        } else {
            setIsAuthenticated(false);
            setRole(null);
            setUser(null);
        }
    }, [])
    return (
        <AuthContext.Provider value={{ isAuthenticated, role, setIsAuthenticated, setRole, user }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuthContext = (): AuthContextType => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuthContext must be used within an AuthProvider');
    }
    return context;
};
