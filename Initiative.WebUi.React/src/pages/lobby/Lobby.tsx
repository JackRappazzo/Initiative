import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { LobbyClient } from '../../signalR/LobbyClient';
import './Lobby.css';

const Lobby: React.FC = () => {
    const { roomCode } = useParams<{ roomCode: string }>();
    const [lobbyClient, setLobbyClient] = useState<LobbyClient | null>(null);
    const [isConnected, setIsConnected] = useState(false);
    const [connectionError, setConnectionError] = useState<string | null>(null);

    useEffect(() => {
        if (!roomCode) return;

        // Initialize the lobby client
        // Note: You'll need to replace this with your actual SignalR hub URL
        const hubUrl = '/lobbyhub'; // Update this to match your backend hub URL
        const client = new LobbyClient(hubUrl, roomCode);
        
        setLobbyClient(client);

        // Subscribe to connection status
        const connectedSub = client.isConnected$.subscribe(setIsConnected);
        const errorSub = client.error$.subscribe(setConnectionError);

        // Start the connection and join the lobby
        const initializeConnection = async () => {
            try {
                await client.connect();
                await client.joinLobby(roomCode);
            } catch (err: any) {
                console.error('Failed to start lobby connection:', err);
                setConnectionError('Failed to connect to lobby');
            }
        };

        initializeConnection();

        // Cleanup on component unmount
        return () => {
            connectedSub.unsubscribe();
            errorSub.unsubscribe();
            if (lobbyClient) {
                lobbyClient.disconnect();
            }
        };
    }, [roomCode, lobbyClient]);

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

            <div className="lobby-content">
                <p>Welcome to the lobby! This page will be expanded with lobby functionality.</p>
                {/* Lobby functionality will be added here in the next iteration */}
            </div>
        </div>
    );
};

export default Lobby;
