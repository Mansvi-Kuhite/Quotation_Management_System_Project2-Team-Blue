import { createContext, useContext, useMemo, useState } from "react";
import apiClient from "../api/client";
import { extractUserFromToken, isTokenExpired } from "../utils/auth";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => {
    const stored = localStorage.getItem("qms_token");
    if (!stored) return null;
    if (isTokenExpired(stored)) {
      localStorage.removeItem("qms_token");
      return null;
    }
    return stored;
  });
  const [user, setUser] = useState(() => {
    const stored = localStorage.getItem("qms_token");
    if (!stored || isTokenExpired(stored)) {
      localStorage.removeItem("qms_token");
      return null;
    }
    return extractUserFromToken(stored);
  });

  const login = async (username, password) => {
    const response = await apiClient.post("/auth/login", { username, password });
    const nextToken = response.data?.token;
    if (!nextToken) {
      throw new Error("Token not returned by server.");
    }
    localStorage.setItem("qms_token", nextToken);
    setToken(nextToken);
    setUser(extractUserFromToken(nextToken));
  };

  const logout = () => {
    localStorage.removeItem("qms_token");
    setToken(null);
    setUser(null);
  };

  const value = useMemo(
    () => ({
      token,
      user,
      isAuthenticated: Boolean(token),
      isSalesRep: user?.role === "SalesRep",
      isManager: user?.role === "SalesManager",
      isAdmin: user?.role === "Admin",
      login,
      logout
    }),
    [token, user]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used inside AuthProvider.");
  }
  return context;
}
