export type LobbyMode = 'Waiting' | 'InProgress';

export interface LobbyState {
    Creatures: string[];
    CurrentCreatureIndex: number;
    CurrentTurn: number;
    CurrentMode: LobbyMode;
}