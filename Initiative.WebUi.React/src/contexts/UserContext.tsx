import React, { createContext, useContext, useState, useEffect } from 'react';
import { UserClient, UserInformation } from '../api/userClient';
import { AuthContext } from './AuthContext';

// Export the interface
export interface UserContextType {
  userInfo: UserInformation | null;
  isLoading: boolean;
  error: string | null;
  refreshUserInfo: () => Promise<void>;
}

// Export the context
export const UserContext = createContext<UserContextType>({
  userInfo: null,
  isLoading: true,
  error: null,
  refreshUserInfo: async () => {},
});

export const useUser = () => useContext(UserContext);

export const UserProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isLoggedIn } = useContext(AuthContext);
  const [userInfo, setUserInfo] = useState<UserInformation | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const userClient = React.useMemo(() => new UserClient(), []);

  const refreshUserInfo = React.useCallback(async () => {
    if (!isLoggedIn()) return;

    try {
      setIsLoading(true);
      setError(null);
      const info = await userClient.getUserInformation();
      setUserInfo(info);
    } catch (err) {
      setError('Failed to load user information');
      console.error('Error loading user information:', err);
      setUserInfo(null);
    } finally {
      setIsLoading(false);
    }
  }, [userClient, isLoggedIn]);

  // Refresh user info when auth state changes
  useEffect(() => {
    if (isLoggedIn()) {
      refreshUserInfo();
    } else {
      setUserInfo(null);
      setError(null);
      setIsLoading(false);
    }
  }, [isLoggedIn, refreshUserInfo]);

  return (
    <UserContext.Provider value={{ userInfo, isLoading, error, refreshUserInfo }}>
      {children}
    </UserContext.Provider>
  );
};
