import * as signalR from "@microsoft/signalr";

export class LobbyClient {
    private connection: signalR.HubConnection;
    private roomCode: string;

    constructor(hubUrl: string, roomCode: string) {
        this.roomCode = roomCode;

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl)
            .withAutomaticReconnect()
            .build();

        this.registerHandlers();
    }

    private registerHandlers() {
        this.connection.on("NextTurn", () => {
            console.log("Next turn triggered.");
            // Handle turn progression
        });

        this.connection.on("CreatureList", (creatures: string[]) => {
            console.log("Received creature list:", creatures);
            // Update local creature list
        });

        this.connection.on("UserJoined", (connectionId: string) => {
            console.log("User joined:", connectionId);
        });

        this.connection.on("UserLeft", (connectionId: string) => {
            console.log("User left:", connectionId);
        });

        this.connection.on("Error", (error: string) => {
            console.error("Server error:", error);
        });
    }

    public async connect(): Promise<void> {
        try {
            await this.connection.start();
            console.log("Connected to LobbyHub");

            await this.connection.invoke("JoinLobby", this.roomCode);
        } catch (err) {
            console.error("Failed to connect:", err);
        }
    }

    public async disconnect(): Promise<void> {
        try {
            await this.connection.invoke("LeaveLobby", this.roomCode);
            await this.connection.stop();
        } catch (err) {
            console.error("Error during disconnect:", err);
        }
    }

    public async sendNextTurn(): Promise<void> {
        try {
            await this.connection.invoke("SendNextTurn");
        } catch (err) {
            console.error("Failed to send next turn:", err);
        }
    }

    public async sendCreatureList(creatures: string[]): Promise<void> {
        try {
            await this.connection.invoke("SendCreatureList", creatures);
        } catch (err) {
            console.error("Failed to send creature list:", err);
        }
    }
}