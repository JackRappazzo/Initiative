import { Subject, BehaviorSubject } from "rxjs";
import * as signalR from "@microsoft/signalr";
import { LobbyState } from "./LobbyState";

export class LobbyClient {
    public nextTurn$ = new Subject<void>();
    public creatureList$ = new Subject<string[]>();
    public receivedCreatureList$ = new Subject<string[]>();
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
    public roomCode: string;

    constructor(hubUrl: string, roomCode: string) {
        this.roomCode = roomCode;

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

        this.connection.on("UserJoined", (connectionId: string) => {
            console.log("[SignalR] Received: UserJoined", connectionId);
            this.userJoined$.next(connectionId);
        });

        this.connection.on("UserLeft", (connectionId: string) => {
            console.log("[SignalR] Received: UserLeft", connectionId);
            this.userLeft$.next(connectionId);
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
        try {
            await this.connection.start();
            this.connected$.next();
            this.isConnected$.next(true);
        } catch (err) {
            this.error$.next("Failed to connect: " + err);
            this.isConnected$.next(false);
            throw err;
        }
    }

    public async disconnect(): Promise<void> {
        try {
            await this.connection.invoke("LeaveLobby", this.roomCode);
            await this.connection.stop();
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

    public async getLobbyState(): Promise<LobbyState> {
        try {
            return await this.connection.invoke("GetLobbyState");
        } catch (err) {
            this.error$.next("Failed to get lobby state: " + err);
            throw err;
        }
    }
}