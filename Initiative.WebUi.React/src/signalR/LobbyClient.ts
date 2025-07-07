import { Subject, BehaviorSubject } from "rxjs";
import * as signalR from "@microsoft/signalr";
import { LobbyState } from "./LobbyState";

export class LobbyClient {
    public receivedLobbyState$ = new Subject<LobbyState>();
    public userJoined$ = new Subject<string>();
    public userLeft$ = new Subject<string>();
    public error$ = new Subject<string>();
    public connected$ = new Subject<void>();
    public reconnected$ = new Subject<void>();
    public closed$ = new Subject<void>();

    public isConnected$ = new BehaviorSubject<boolean>(false);
    public isInLobby = new BehaviorSubject<boolean>(false);

    private connection: signalR.HubConnection;
    private isConnecting = false;

    constructor(hubUrl: string) {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl)
            .withAutomaticReconnect()
            .build();

        this.registerHandlers();

        this.connection.onclose(() => {
            this.closed$.next();
            this.isConnected$.next(false);
        });
        this.connection.onreconnected(() => {
            this.reconnected$.next();
            this.isConnected$.next(true);
        });
        this.connection.onreconnecting(() => {
            // Optionally handle reconnecting
        });
    }

    private registerHandlers() {

        this.connection.on("ReceivedLobbyState", (state: LobbyState) => {
            console.log("[SignalR] Received: ReceivedLobbyState", state);
            this.receivedLobbyState$.next(state);
        });

        this.connection.on("RoomJoined", (state: any) => {
            console.log("[SignalR] Received: RoomJoined", state);
            this.receivedLobbyState$.next(state);
        });

        this.connection.on("Error", (error: string) => {
            console.log("[SignalR] Received: Error", error);
            this.error$.next(error);
        });

        this.connection.on("LobbyJoined", (state: any) => {
            console.log("[SignalR] Received: LobbyJoined", state);
            this.receivedLobbyState$.next(state);
            this.isConnected$.next(true);
            this.isInLobby.next(true);
        });
    }

    public async connect(): Promise<void> {
        // Prevent multiple connection attempts
        if (this.isConnecting) {
            console.log('[LobbyClient] Connection already in progress, waiting...');
            return this.waitForConnection();
        }

        // Check if already connected
        if (this.connection.state === signalR.HubConnectionState.Connected) {
            console.log('[LobbyClient] Already connected');
            this.isConnected$.next(true);
            return;
        }

        // Check if currently connecting
        if (this.connection.state === signalR.HubConnectionState.Connecting) {
            console.log('[LobbyClient] Already connecting, waiting...');
            return this.waitForConnection();
        }

        this.isConnecting = true;

        try {
            console.log('[LobbyClient] Starting connection...');
            await this.connection.start();
            console.log('[LobbyClient] Connection established');
            this.connected$.next();
            this.isConnected$.next(true);
        } catch (err) {
            console.error('[LobbyClient] Connection failed:', err);
            this.error$.next("Failed to connect: " + err);
            this.isConnected$.next(false);
            throw err;
        } finally {
            this.isConnecting = false;
        }
    }

    private async waitForConnection(): Promise<void> {
        // Wait for the current connection attempt to complete
        return new Promise((resolve, reject) => {
            const checkConnection = () => {
                if (!this.isConnecting) {
                    if (this.connection.state === signalR.HubConnectionState.Connected) {
                        resolve();
                    } else {
                        reject(new Error('Connection failed'));
                    }
                } else {
                    setTimeout(checkConnection, 100); // Check again in 100ms
                }
            };
            checkConnection();
        });
    }

    public async disconnect(): Promise<void> {
        this.isConnecting = false;
        try {
            if (this.connection.state !== signalR.HubConnectionState.Disconnected) {
                console.log('[LobbyClient] Disconnecting...');
                await this.connection.stop();
            }
            this.isConnected$.next(false);
        } catch (err) {
            this.error$.next("Error during disconnect: " + err);
        }
    }
    
    public async setLobbyState(creatureList: string[], currentCreatureIndex: number, currentTurn: number, lobbyMode: string): Promise<void> {
        try {
            await this.connection.invoke("SetLobbyState", { 
                creatures: creatureList, 
                currentCreatureIndex: currentCreatureIndex, 
                currentTurn: currentTurn,
                currentMode: lobbyMode
            });
        } catch (err) {
            this.error$.next("Failed to set lobby state: " + err);
        }
    }

    public async joinLobby(roomCode: string): Promise<void> {
        try {
            await this.connection.invoke("JoinLobby", roomCode);
        } catch (err) {
            this.error$.next("Failed to join lobby: " + err);
        }
    }

    public async getLobbyState(): Promise<void> {
        try {
            await this.connection.invoke("GetLobbyState");
        } catch (err) {
            this.error$.next("Failed to get lobby state: " + err);
            throw err;
        }
    }
}