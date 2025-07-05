import { HttpClient } from "./httpClient";

export interface CreatureJsonModel {
  name: string;
  hitPoints: number;
  maximumHitPoints: number;
  armorClass: number;
  initiative: number;
  initiativeModifier: number;
}

export interface FetchEncounterResponse {
  displayName: string;
  encounterId: string;
  createdAt: string;
  creatures: CreatureJsonModel[];
}

export class EncounterClient {
  private apiClient: HttpClient;

  constructor() {
    this.apiClient = HttpClient.GetInstance();
  }

  // POST /api/encounter
  public async createEncounter(encounterName: string): Promise<{ encounterId: string; displayName: string }> {
    return await this.apiClient.post<{ encounterId: string; displayName: string }>("encounter", { encounterName });
  }

  // GET /api/encounter
  public async getEncounterList(): Promise<FetchEncounterResponse[]> {
    return await this.apiClient.get<FetchEncounterResponse[]>("encounter");
  }

  // GET /api/encounter/{encounterId}
  public async getEncounter(encounterId: string): Promise<FetchEncounterResponse> {
    return await this.apiClient.get<FetchEncounterResponse>(`encounter/${encodeURIComponent(encounterId)}`);
  }

  // POST /api/encounter/{encounterId}/creatures
  public async setCreatures(encounterId: string, creatures: CreatureJsonModel[]): Promise<void> {
    await this.apiClient.post<void>(`encounter/${encodeURIComponent(encounterId)}/creatures`, creatures);
  }

  // PUT /api/encounter/{encounterId}/setName
  public async renameEncounter(encounterId: string, newName: string): Promise<void> {
    await this.apiClient.post<void>(`encounter/${encodeURIComponent(encounterId)}/setName`, { newName });
  }

  // DELETE /api/encounter/{encounterId}
  public async deleteEncounter(encounterId: string): Promise<void> {
    await this.apiClient.instance.delete(`encounter/${encodeURIComponent(encounterId)}`);
  }
}