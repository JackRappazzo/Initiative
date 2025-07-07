import { useEffect, useMemo, useRef, useCallback } from 'react';
import { LobbyClient } from '../signalR/LobbyClient';

// Global LobbyClient instance to prevent multiple connections
let globalLobbyClient: LobbyClient | null = null;
let isGloballyConnecting = false;
let globalClientReferenceCount = 0;
let lastJoinedRoomCode: string | null = null;

export interface UseLobbyConnectionOptions {
  roomCode?: string | null;
  hubUrl?: string;
  autoConnect?: boolean;
  autoJoinRoom?: boolean;
}

export interface UseLobbyConnectionReturn {
  lobbyClient: LobbyClient;
  isConnected: boolean;
  isConnecting: boolean;
  connect: () => Promise<void>;
  disconnect: () => Promise<void>;
  joinRoom: (roomCode: string) => Promise<void>;
}

export const useLobbyConnection = (options: UseLobbyConnectionOptions = {}): UseLobbyConnectionReturn => {
  const {
    roomCode,
    hubUrl = 'https://localhost:7034/lobby',
    autoConnect = true,
    autoJoinRoom = true
  } = options;

  // Track if this hook instance has joined a room
  const hasJoinedRef = useRef(false);
  const currentRoomCodeRef = useRef<string | null>(null);

  // Create or get the global LobbyClient instance
  const lobbyClient = useMemo(() => {
    if (!globalLobbyClient) {
      console.log('[useLobbyConnection] Creating new LobbyClient instance');
      globalLobbyClient = new LobbyClient(hubUrl);
    } else {
      console.log('[useLobbyConnection] Using existing LobbyClient instance');
    }
    
    // Increment reference count
    globalClientReferenceCount++;
    console.log('[useLobbyConnection] Global client reference count:', globalClientReferenceCount);
    
    return globalLobbyClient;
  }, [hubUrl]);

  // Connect to lobby
  const connect = useCallback(async (): Promise<void> => {
    // Prevent multiple global connection attempts
    if (isGloballyConnecting) {
      console.log('[useLobbyConnection] Global connection already in progress, skipping...');
      return;
    }

    try {
      console.log('[useLobbyConnection] Connecting to lobby...');
      isGloballyConnecting = true;
      
      // Connect to the SignalR hub
      await lobbyClient.connect();
      console.log('[useLobbyConnection] Connected to lobby hub');
      
    } catch (err) {
      console.error('[useLobbyConnection] Failed to connect to lobby:', err);
      throw err;
    } finally {
      isGloballyConnecting = false;
    }
  }, [lobbyClient]);

  // Disconnect from lobby
  const disconnect = useCallback(async (): Promise<void> => {
    try {
      await lobbyClient.disconnect();
      console.log('[useLobbyConnection] Disconnected from lobby');
    } catch (err) {
      console.error('[useLobbyConnection] Error during disconnect:', err);
      throw err;
    }
  }, [lobbyClient]);

  // Join a specific room
  const joinRoom = useCallback(async (targetRoomCode: string): Promise<void> => {
    if (!targetRoomCode) {
      console.log('[useLobbyConnection] No room code provided, skipping room join');
      return;
    }

    // Skip if already joined this room globally
    if (targetRoomCode === lastJoinedRoomCode) {
      console.log('[useLobbyConnection] Already joined room globally:', targetRoomCode);
      return;
    }

    try {
      console.log('[useLobbyConnection] Joining lobby room:', targetRoomCode);
      await lobbyClient.joinLobby(targetRoomCode);
      lastJoinedRoomCode = targetRoomCode;
      currentRoomCodeRef.current = targetRoomCode;
      hasJoinedRef.current = true;
      console.log('[useLobbyConnection] Successfully joined lobby room:', targetRoomCode);
    } catch (err) {
      console.error('[useLobbyConnection] Failed to join lobby room:', err);
      throw err;
    }
  }, [lobbyClient]);

  // Auto-connect effect
  useEffect(() => {
    if (!autoConnect) return;

    connect().catch(err => {
      console.error('[useLobbyConnection] Auto-connect failed:', err);
    });

    // Cleanup: handle reference counting on unmount
    return () => {
      console.log('[useLobbyConnection] Hook unmounting...');
      
      // Decrement reference count
      globalClientReferenceCount--;
      console.log('[useLobbyConnection] Global client reference count after unmount:', globalClientReferenceCount);
      
      // If no more hooks are using the client, clean it up
      if (globalClientReferenceCount <= 0 && globalLobbyClient) {
        console.log('[useLobbyConnection] No more references, cleaning up global client...');
        globalLobbyClient.disconnect().catch(err => {
          console.error('[useLobbyConnection] Error during cleanup disconnect:', err);
        });
        globalLobbyClient = null;
        isGloballyConnecting = false;
        globalClientReferenceCount = 0;
        lastJoinedRoomCode = null;
      }
    };
  }, [autoConnect, connect]);

  // Auto-join room effect
  useEffect(() => {
    if (!autoJoinRoom || !roomCode) {
      console.log('[useLobbyConnection] Auto-join disabled or no room code available');
      return;
    }

    // Skip if already joined this room for this hook instance
    if (roomCode === currentRoomCodeRef.current && hasJoinedRef.current) {
      console.log('[useLobbyConnection] Already joined room for this hook instance:', roomCode);
      return;
    }

    // Skip if already joined this room globally
    if (roomCode === lastJoinedRoomCode) {
      console.log('[useLobbyConnection] Already joined room globally:', roomCode);
      currentRoomCodeRef.current = roomCode;
      hasJoinedRef.current = true;
      return;
    }

    const handleRoomJoin = async () => {
      try {
        await joinRoom(roomCode);
      } catch (err) {
        console.error('[useLobbyConnection] Auto-join room failed:', err);
      }
    };

    // Check if already connected, if so join immediately
    if (lobbyClient.isConnected$.value) {
      handleRoomJoin();
    } else {
      // Subscribe to connection state changes
      const subscription = lobbyClient.isConnected$.subscribe((isConnected) => {
        if (isConnected && roomCode === currentRoomCodeRef.current) {
          handleRoomJoin().catch(err => {
            console.error('[useLobbyConnection] Error in subscription room join:', err);
          });
        }
      });

      return () => {
        subscription.unsubscribe();
      };
    }
  }, [roomCode, autoJoinRoom, lobbyClient, joinRoom]);

  // Reset room tracking when roomCode changes
  useEffect(() => {
    if (roomCode !== currentRoomCodeRef.current) {
      hasJoinedRef.current = false;
      currentRoomCodeRef.current = roomCode || null;
    }
  }, [roomCode]);

  return {
    lobbyClient,
    isConnected: lobbyClient.isConnected$.value,
    isConnecting: isGloballyConnecting,
    connect,
    disconnect,
    joinRoom
  };
};
