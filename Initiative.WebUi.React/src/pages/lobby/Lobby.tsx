import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { useLobbyConnection } from '../../hooks';
import { LobbyState, isValidLobbyState } from '../../signalR/LobbyState';
import './Lobby.css';

const Lobby: React.FC = () => {
    const { roomCode } = useParams<{ roomCode: string }>();
    const [connectionError, setConnectionError] = useState<string | null>(null);
    const [lobbyState, setLobbyState] = useState<LobbyState | null>(null);
    const [isWaiting, setIsWaiting] = useState<boolean>(true);

    // Use the lobby connection hook - handles all connection logic
    const { lobbyClient, isConnected } = useLobbyConnection({
        roomCode,
        autoConnect: true,
        autoJoinRoom: true
    });

    useEffect(() => {
        if (!lobbyClient) return;

        console.log('[Lobby] Setting up subscriptions');
        
        // Subscribe to lobby state updates and errors
        const errorSub = lobbyClient.error$.subscribe((error: string) => {
            console.log('[Lobby] Error received:', error);
            setConnectionError(error);
        });

        const lobbyStateSub = lobbyClient.receivedLobbyState$.subscribe((newState: LobbyState) => {
            console.log('[Lobby] Received lobby state update:', newState);
            setLobbyState(newState);
            
            // Update waiting state based on lobby state
            if (newState && newState.CurrentMode) {
                setIsWaiting(newState.CurrentMode === 'Waiting');
            }
        });

        // Cleanup subscriptions
        return () => {
            console.log('[Lobby] Cleaning up subscriptions');
            errorSub.unsubscribe();
            lobbyStateSub.unsubscribe();
        };
    }, [lobbyClient]);

    // Separate effect for requesting initial lobby state
    useEffect(() => {
        if (!lobbyClient || !roomCode) return;

        const requestLobbyState = async () => {
            try {
                console.log('[Lobby] Requesting lobby state for room:', roomCode);
                await lobbyClient.getLobbyState();
                
                setConnectionError(null);
            } catch (err: any) {
                console.error('Failed to get lobby state:', err);
                setConnectionError('Failed to get lobby state');
            }
        };

        // Only request state when connected
        if (lobbyClient.isConnected$.value) {
            console.log('[Lobby] Already connected, requesting state');
            // Add delay to ensure room joining is complete
            setTimeout(() => {
                requestLobbyState();
            }, 1000);
        } else {
            console.log('[Lobby] Not connected yet, waiting for connection');
            const connectedSub = lobbyClient.isConnected$.subscribe(async (connected: boolean) => {
                if (connected) {
                    console.log('[Lobby] Connected, requesting state');
                    setTimeout(() => {
                        requestLobbyState();
                    }, 1000);
                    connectedSub.unsubscribe(); // Only request once
                }
            });

            return () => {
                connectedSub.unsubscribe();
            };
        }
    }, [lobbyClient, roomCode]);

    if (!roomCode) {
        return (
            <div className="lobby-container">
                <div className="lobby-error">
                    <h2>Invalid Room Code</h2>
                    <p>No room code provided.</p>
                </div>
            </div>
        );
    }

    const getCurrentCreature = () => {
        if (!isValidLobbyState(lobbyState)) {
            return null;
        }
        return lobbyState.Creatures[lobbyState.CurrentCreatureIndex] || null;
    };

    const getNextCreature = () => {
        if (!isValidLobbyState(lobbyState)) {
            return null;
        }
        const nextIndex = (lobbyState.CurrentCreatureIndex + 1) % lobbyState.Creatures.length;
        return lobbyState.Creatures[nextIndex] || null;
    };

    return (
        <div className="lobby-container">
            <div className="lobby-header">
                <h1>Lobby: {roomCode}</h1>
                <div className="connection-status">
                    {isConnected ? (
                        <span className="status-connected">Connected</span>
                    ) : (
                        <span className="status-connecting">Connecting...</span>
                    )}
                </div>
            </div>

            {connectionError && (
                <div className="lobby-error">
                    <p>Error: {connectionError}</p>
                </div>
            )}

            <div className="lobby-main">
                {/* Initiative List - Left Side */}
                <div className="initiative-list">
                    <h3>Initiative Order</h3>
                    {!isWaiting && isValidLobbyState(lobbyState) ? (
                        <ul>
                            {lobbyState.Creatures.map((creature, index) => (
                                <li 
                                    key={index} 
                                    className={index === lobbyState.CurrentCreatureIndex ? 'current-creature' : ''}
                                >
                                    {creature}
                                </li>
                            ))}
                        </ul>
                    ) : (
                        <p>{isWaiting ? "Waiting for adventure to begin..." : "No creatures in initiative"}</p>
                    )}
                </div>

                {/* Central Display */}
                <div className="central-display">
                    {isWaiting ? (
                        <div className="waiting-display">
                            <h2>Waiting for adventure!</h2>
                        </div>
                    ) : (
                        <div className="creature-display">
                            <div className="current-creature">
                                {getCurrentCreature() || 'No current creature'}
                            </div>
                            <div className="next-creature">
                                Next: {getNextCreature() || 'No next creature'}
                            </div>
                            <div className="turn-info">
                                <span>Turn {lobbyState?.CurrentTurn || 0}</span>
                                <span>Creature {(lobbyState?.CurrentCreatureIndex || 0) + 1}</span>
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default Lobby;
