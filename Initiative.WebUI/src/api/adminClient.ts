
import { LoginResponse } from "./messages/LoginResponse";
import { AxiosResponse } from "axios";
import { HttpClient } from "./httpClient";

export class AdminClient {
    
    private apiClient: HttpClient;
    constructor() {
        this.apiClient = HttpClient.GetInstance();
    }

    public async Login(email:string, password:string) : Promise<LoginResponse>
    {
        const response = await this.apiClient.post<LoginResponse>("admin/login", {
            emailAddress: email,
            password
        });

        return response;
    }

    public async Register(displayName:string, email:string, password:string) : Promise<boolean>
    {
        const response:AxiosResponse<any> = await this.apiClient.post<any>("admin/register", { displayName, emailAddress: email, password});
        
        return response.status === 200;
    }
}