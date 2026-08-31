import { createContext, useContext, useState, ReactNode } from "react";

interface DndBeyondContextType {
  token: string | null;
  setToken: (token: string) => void;
  clearToken: () => void;
  hasToken: boolean;
}

const STORAGE_KEY = "dndBeyondToken";

export const DndBeyondContext = createContext<DndBeyondContextType>({
  token: null,
  setToken: () => {},
  clearToken: () => {},
  hasToken: false,
});

export const useDndBeyondSession = () => useContext(DndBeyondContext);

export const DndBeyondProvider = ({ children }: { children: ReactNode }) => {
  const [token, setTokenState] = useState<string | null>(() =>
    localStorage.getItem(STORAGE_KEY)
  );

  const setToken = (newToken: string) => {
    localStorage.setItem(STORAGE_KEY, newToken);
    setTokenState(newToken);
  };

  const clearToken = () => {
    localStorage.removeItem(STORAGE_KEY);
    setTokenState(null);
  };

  return (
    <DndBeyondContext.Provider
      value={{ token, setToken, clearToken, hasToken: !!token }}
    >
      {children}
    </DndBeyondContext.Provider>
  );
};
