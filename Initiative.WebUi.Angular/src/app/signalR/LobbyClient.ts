import { Subject, BehaviorSubject } from "rxjs";
import * as signalR from "@microsoft/signalr";

export class LobbyClient {
    public nextTurn$ = new Subject<void>();
    public creatureList$ = new Subject<string[]>();
    public receivedCreatureList$ = new Subject<string[]>();
    public receivedLobbyState$ = new Subject<any>();
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
        this.connection.on("NextTurn", () => {
            console.log("[SignalR] Received: NextTurn");
            this.nextTurn$.next();
        });

        this.connection.on("CreatureList", (creatures: string[]) => {
            console.log("[SignalR] Received: CreatureList", creatures);
            this.creatureList$.next(creatures);
        });

        this.connection.on("ReceivedCreatureList", (creatures: string[]) =>  {
            console.log("[SignalR] Received: ReceivedCreatureList", creatures);
            this.receivedCreatureList$.next(creatures);
        });

        this.connection.on("ReceivedLobbyState", (state: any) => {
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

    public async sendNextTurn(): Promise<void> {
        try {
            await this.connection.invoke("SendNextTurn");
        } catch (err) {
            this.error$.next("Failed to send next turn: " + err);
        }
    }

    public async sendCreatureList(creatures: string[]): Promise<void> {
        try {
            await this.connection.invoke("SendCreatureList", creatures);
        } catch (err) {
            this.error$.next("Failed to send creature list: " + err);
        }
    }
    
    public async setEncounterState(encounterState: any): Promise<void> {
        try {
            await this.connection.invoke("SetEncounterState", encounterState);
        } catch (err) {
            this.error$.next("Failed to set encounter state: " + err);
        }
    }

    public async startEncounter(creatures: string[]): Promise<void> {
        try {
            await this.connection.invoke("StartEncounter", creatures);
        } catch (err) {
            this.error$.next("Failed to start encounter: " + err);
        }
    }

    public async endEncounter(): Promise<void> {
        try {
            await this.connection.invoke("EndEncounter");
        } catch (err) {
            this.error$.next("Failed to end encounter: " + err);
        }
    }

    public async joinLobby(roomCode: string): Promise<void> {
        try {
            await this.connection.invoke("JoinLobby", roomCode);
        } catch (err) {
            this.error$.next("Failed to join lobby: " + err);
        }
    }
}