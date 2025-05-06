import { ApiClient } from "./apiClient";
import { LoginResponse } from "./messages/LoginResponse";

export class AdminClient {
    private apiClient:ApiClient = new ApiClient();

    public async Login(email:string, password:string) : Promise<LoginResponse>
    {
        const response = await this.apiClient.post<LoginResponse>("admin/login", {
            email,
            password
        });

        return response;
    }
}