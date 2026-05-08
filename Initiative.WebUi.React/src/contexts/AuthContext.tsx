// src/context/AuthContext.tsx
import { createContext, useState, ReactNode } from "react";

interface AuthContextType {
  token: string | null;
  login: (token: string) => void;
  logout: () => void;
  isLoggedIn: () => boolean;
}

export const AuthContext = createContext<AuthContextType>({
  token: null,
  login: () => { },
  logout: () => {},
  isLoggedIn: () => {return false;}
});

const isTokenExpired = (token: string): boolean => {
  try {
    const payloadBase64 = token.split('.')[1];
    const payload = JSON.parse(atob(payloadBase64));
    const now = Math.floor(Date.now() / 1000);
    return payload.exp <= now;
  } catch {
    return true;
  }
};

const getStoredToken = (): string | null => {
  const stored = localStorage.getItem("token");
  if (stored && !isTokenExpired(stored)) {
    return stored;
  }
  if (stored) {
    localStorage.removeItem("token");
  }
  return null;
};

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [token, setToken] = useState<string | null>(getStoredToken);

  const login = (token: string) => {
    localStorage.setItem("token", token);
    setToken(token);
  };

  const logout = () => {
    localStorage.removeItem("token");
    setToken(null);
  };

  const isLoggedIn = () => {
    return token != null && token !== "" && !isTokenExpired(token);
  }

  return (
    <AuthContext.Provider value={{ token, isLoggedIn, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};