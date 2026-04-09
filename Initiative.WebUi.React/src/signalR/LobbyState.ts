export type LobbyMode = 'Waiting' | 'InProgress';

export type LobbyHealthStatus = 'Healthy' | 'Hurt' | 'Bloodied' | '';

export interface LobbyCreature {
    DisplayName?: string;
    displayName?: string;
    Statuses?: string[];
    statuses?: string[];
    HealthStatus?: LobbyHealthStatus;
    healthStatus?: LobbyHealthStatus;
    IsPlayer?: boolean;
    isPlayer?: boolean;
    IsHidden?: boolean;
    isHidden?: boolean;
}

export type LobbyCreatureEntry = string | LobbyCreature;

export interface LobbyState {
    Creatures: LobbyCreatureEntry[];
    CurrentCreatureIndex: number;
    CurrentTurn: number;
    CurrentMode: LobbyMode;
}

export const isValidLobbyState = (lobbyState: LobbyState | null): lobbyState is LobbyState => {
    return lobbyState != null && 
           lobbyState.Creatures != null && 
           lobbyState.Creatures.length > 0;
};