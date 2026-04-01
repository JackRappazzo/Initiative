import { HttpClient } from "./httpClient";

export interface PartyMember {
  name: string;
  level: number;
}

export interface Party {
  partyId: string;
  name: string;
  members: PartyMember[];
}

export class PartyClient {
  private apiClient: HttpClient;

  constructor() {
    this.apiClient = HttpClient.GetInstance();
  }

  // GET /api/party
  public async getParties(): Promise<Party[]> {
    return this.apiClient.get<Party[]>("party");
  }

  // GET /api/party/{partyId}
  public async getParty(partyId: string): Promise<Party> {
    return this.apiClient.get<Party>(`party/${encodeURIComponent(partyId)}`);
  }

  // POST /api/party
  public async createParty(name: string, members: PartyMember[]): Promise<{ partyId: string; name: string }> {
    return this.apiClient.post<{ partyId: string; name: string }>("party", { name, members });
  }

  // DELETE /api/party/{partyId}
  public async deleteParty(partyId: string): Promise<void> {
    return this.apiClient.delete<void>(`party/${encodeURIComponent(partyId)}`);
  }
}
