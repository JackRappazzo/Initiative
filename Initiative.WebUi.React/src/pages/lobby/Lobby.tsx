import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { useLobbyConnection } from '../../hooks';
import { LobbyCreatureEntry, LobbyHealthStatus, LobbyState, isValidLobbyState } from '../../signalR/LobbyState';
import './Lobby.css';

const HEALTH_STATUS_SET = new Set(['healthy', 'hurt', 'bloodied']);

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

    const getCreatureName = (creature: LobbyCreatureEntry): string => {
        if (typeof creature === 'string') {
            return creature;
        }

        return creature.DisplayName || creature.displayName || 'Unnamed Creature';
    };

    const getCreatureStatuses = (creature: LobbyCreatureEntry): string[] => {
        if (typeof creature === 'string') {
            return [];
        }

        const statuses = creature.Statuses || creature.statuses || [];
        return statuses
            .map(status => status.trim())
            .filter((status) => Boolean(status) && !HEALTH_STATUS_SET.has(status.toLowerCase()));
    };

    const getCreatureHealthStatus = (creature: LobbyCreatureEntry): LobbyHealthStatus => {
        if (typeof creature === 'string') {
            return '';
        }

        return creature.HealthStatus || creature.healthStatus || '';
    };

    const getCreatureIsPlayer = (creature: LobbyCreatureEntry): boolean => {
        if (typeof creature === 'string') {
            return false;
        }

        return creature.IsPlayer ?? creature.isPlayer ?? false;
    };

    const isCreatureVisible = (creature: LobbyCreatureEntry): boolean => {
        if (typeof creature === 'string') {
            return true;
        }

        return !(creature.IsHidden ?? creature.isHidden ?? false);
    };

    const formatCreatureForDisplay = (creature: LobbyCreatureEntry): string => {
        const name = getCreatureName(creature);
        const statuses = getCreatureStatuses(creature);

        if (statuses.length === 0) {
            return name;
        }

        return `${name} (${statuses.join(', ')})`;
    };

    const getHealthStatusClassName = (creature: LobbyCreatureEntry): string => {
        const healthStatus = getCreatureHealthStatus(creature);
        switch (healthStatus) {
            case 'Bloodied':
                return 'health-dot bloodied';
            case 'Hurt':
                return 'health-dot hurt';
            case 'Healthy':
                return 'health-dot healthy';
            default:
                return 'health-dot';
        }
    };

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

    const currentCreature = getCurrentCreature();
    const nextCreature = getNextCreature();

    const currentTurn = isValidLobbyState(lobbyState)
        ? Math.max(lobbyState.CurrentTurn, 1)
        : 1;

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
                    <h3>Turn {currentTurn}</h3>
                    {!isWaiting && isValidLobbyState(lobbyState) ? (
                        <ul>
                            {lobbyState.Creatures.map((creature, index) => (
                                isCreatureVisible(creature) ? (
                                    <li
                                        key={index}
                                        className={index === lobbyState.CurrentCreatureIndex ? 'current-creature' : ''}
                                    >
                                        <div className="creature-name-row">
                                            {!getCreatureIsPlayer(creature) && getCreatureHealthStatus(creature) && (
                                                <span className={getHealthStatusClassName(creature)} title={`Health Status: ${getCreatureHealthStatus(creature)}`} />
                                            )}
                                            <div className="creature-name">{getCreatureName(creature)}</div>
                                        </div>
                                        {getCreatureStatuses(creature).length > 0 && (
                                            <div className="creature-statuses">
                                                {getCreatureStatuses(creature).map((status, statusIndex) => (
                                                    <span key={`${status}-${statusIndex}`} className="status-chip">
                                                        {status}
                                                    </span>
                                                ))}
                                            </div>
                                        )}
                                    </li>
                                ) : null
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
                            <div className="current-creature-row">
                                {currentCreature && !getCreatureIsPlayer(currentCreature) && getCreatureHealthStatus(currentCreature) && (
                                    <span className={getHealthStatusClassName(currentCreature)} title={`Health Status: ${getCreatureHealthStatus(currentCreature)}`} />
                                )}
                                <div className="current-creature">
                                    {currentCreature ? getCreatureName(currentCreature) : 'No current creature'}
                                </div>
                            </div>
                            {currentCreature && getCreatureStatuses(currentCreature).length > 0 && (
                                <div className="current-creature-statuses">
                                    {getCreatureStatuses(currentCreature).map((status, statusIndex) => (
                                        <span key={`current-${status}-${statusIndex}`} className="status-chip">
                                            {status}
                                        </span>
                                    ))}
                                </div>
                            )}
                            <div className="next-creature">
                                Next: {nextCreature ? formatCreatureForDisplay(nextCreature) : 'No next creature'}
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default Lobby;
