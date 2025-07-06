import { HttpClient } from "./httpClient";

export interface UserInformation {
    displayName: string;
    roomCode: string;
}

export class UserClient {
    private apiClient: HttpClient;

    constructor() {
        this.apiClient = HttpClient.GetInstance();
    }

    // GET /api/user/information
    public async getUserInformation(): Promise<UserInformation> {
        return await this.apiClient.get<UserInformation>("user/information");
    }
}
