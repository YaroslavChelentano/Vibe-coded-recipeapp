import { createContext, useCallback, useContext, useEffect, useState } from 'react';
import * as authApi from '../api/auth';

interface User {
  userId: number;
  email: string;
  displayName: string;
}

interface AuthContextValue {
  user: User | null;
  token: string | null;
  loading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, displayName: string, confirmPassword: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

const TOKEN_KEY = 'token';
const USER_KEY = 'user';

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(() => {
    try {
      const u = localStorage.getItem(USER_KEY);
      return u ? JSON.parse(u) : null;
    } catch {
      return null;
    }
  });
  const [token, setToken] = useState<string | null>(() => localStorage.getItem(TOKEN_KEY));
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!token) {
      setUser(null);
      setLoading(false);
      return;
    }
    setLoading(false);
  }, [token]);

  const login = useCallback(async (email: string, password: string) => {
    const res = await authApi.login({ email, password });
    localStorage.setItem(TOKEN_KEY, res.token);
    const u = { userId: res.userId, email: res.email, displayName: res.displayName };
    localStorage.setItem(USER_KEY, JSON.stringify(u));
    setToken(res.token);
    setUser(u);
  }, []);

  const register = useCallback(async (email: string, password: string, displayName: string, confirmPassword: string) => {
    const res = await authApi.register({ email, password, confirmPassword, displayName });
    localStorage.setItem(TOKEN_KEY, res.token);
    const u = { userId: res.userId, email: res.email, displayName: res.displayName };
    localStorage.setItem(USER_KEY, JSON.stringify(u));
    setToken(res.token);
    setUser(u);
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    setToken(null);
    setUser(null);
  }, []);

  return (
    <AuthContext.Provider value={{ user, token, loading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
