import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './JoinLobby.css';

const JoinLobby: React.FC = () => {
    const [roomCode, setRoomCode] = useState('');
    const navigate = useNavigate();

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value.toUpperCase().slice(0, 6);
        setRoomCode(value);
        
        // Auto-navigate when 6 characters are entered
        if (value.length === 6) {
            navigate(`/lobby/${value}`);
        }
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (roomCode.length === 6) {
            navigate(`/lobby/${roomCode}`);
        }
    };

    return (
        <div className="join-lobby-container">
            <div className="join-lobby-content">
                <h1>Join Lobby</h1>
                <p>Enter the 6-character room code to join a lobby</p>
                
                <form onSubmit={handleSubmit}>
                    <div className="room-code-input-container">
                        <input
                            type="text"
                            value={roomCode}
                            onChange={handleInputChange}
                            className="room-code-input"
                            placeholder="ABCDEF"
                            maxLength={6}
                            autoFocus
                        />
                    </div>
                    
                    {roomCode.length > 0 && roomCode.length < 6 && (
                        <p className="input-hint">
                            {6 - roomCode.length} more character{6 - roomCode.length !== 1 ? 's' : ''} needed
                        </p>
                    )}
                </form>
            </div>
        </div>
    );
};

export default JoinLobby;
