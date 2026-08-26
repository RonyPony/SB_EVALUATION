import { createContext, useContext, useEffect, useState } from "react";
import { authApi, configureApi, type User } from "./api";
const storageKey = "sb.session";
type Stored = { token: string; expiresAt: string };
type AuthState = {
  user: User | null;
  loading: boolean;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
  hasRole: (...roles: string[]) => boolean;
};
const AuthContext = createContext<AuthState | null>(null);
function read(): Stored | null {
  try {
    const value = sessionStorage.getItem(storageKey);
    return value ? (JSON.parse(value) as Stored) : null;
  } catch {
    return null;
  }
}
export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [session, setSession] = useState<Stored | null>(read);
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const logout = () => {
    sessionStorage.removeItem(storageKey);
    setSession(null);
    setUser(null);
    location.hash = "login";
  };
  configureApi(() => session?.token ?? null, logout);
  useEffect(() => {
    if (!session || Date.parse(session.expiresAt) <= Date.now()) {
      logout();
      setLoading(false);
      return;
    }
    setLoading(true);
    authApi
      .me()
      .then(setUser)
      .catch(() => logout())
      .finally(() => setLoading(false));
    const timer = window.setTimeout(
      logout,
      Math.max(0, Date.parse(session.expiresAt) - Date.now()),
    );
    return () => clearTimeout(timer);
  }, [session?.token]);
  const login = async (username: string, password: string) => {
    const result = await authApi.login({ username, password });
    const next = { token: result.accessToken, expiresAt: result.expiresAt };
    sessionStorage.setItem(storageKey, JSON.stringify(next));
    setSession(next);
  };
  return (
    <AuthContext.Provider
      value={{
        user,
        loading,
        login,
        logout,
        hasRole: (...wanted) =>
          !!user?.roles.some((role) => wanted.includes(role)),
      }}>
      {children}
    </AuthContext.Provider>
  );
}
export function useAuth() {
  const value = useContext(AuthContext);
  if (!value) throw new Error("AuthProvider no está disponible");
  return value;
}
