export type LobbyMode = 'Waiting' | 'InProgress';

export interface LobbyState {
    Creatures: string[];
    CurrentCreatureIndex: number;
    CurrentTurn: number;
    CurrentMode: LobbyMode;
}

export const isValidLobbyState = (lobbyState: LobbyState | null): lobbyState is LobbyState => {
    return lobbyState != null && 
           lobbyState.Creatures != null && 
           lobbyState.Creatures.length > 0;
};