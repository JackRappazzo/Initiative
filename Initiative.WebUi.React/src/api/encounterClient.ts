import { HttpClient } from "./httpClient";

export interface EncounterCreatureJsonModel {
  isPlayer: boolean;
  displayName: string;
  creatureName?: string;
  creatureId?: string;
  initiative: number;
  initiativeModifier: number;
  maxHP: number;
  currentHP: number;
  ac: number;
}

export interface EncounterListItem {
  encounterId: string;
  encounterName: string;
  numberOfCreatures: number;
  createdAt: string;
}

export interface FetchEncounterResponse {
  displayName: string;
  encounterId: string;
  createdAt: string;
  creatures: EncounterCreatureJsonModel[];
  turnIndex: number;
  turnCount: number;
  viewersAllowed: boolean;
}

export class EncounterClient {
  private apiClient: HttpClient;

  constructor() {
    this.apiClient = HttpClient.GetInstance();
  }

  // GET /api/encounter
  public async getEncounterList(): Promise<EncounterListItem[]> {
    return this.apiClient.get<EncounterListItem[]>("encounter");
  }

  // POST /api/encounter
  public async createEncounter(encounterName: string): Promise<{ encounterId: string; displayName: string }> {
    return this.apiClient.post<{ encounterId: string; displayName: string }>("encounter", { encounterName });
  }

  // GET /api/encounter/{encounterId}
  public async getEncounter(encounterId: string): Promise<FetchEncounterResponse> {
    return this.apiClient.get<FetchEncounterResponse>(`encounter/${encodeURIComponent(encounterId)}`);
  }

  // POST /api/encounter/{encounterId}/creatures
  public async setCreatures(encounterId: string, creatures: EncounterCreatureJsonModel[]): Promise<void> {
    return this.apiClient.post<void>(`encounter/${encodeURIComponent(encounterId)}/creatures`, creatures);
  }

  // PUT /api/encounter/{encounterId}/setName
  public async renameEncounter(encounterId: string, newName: string): Promise<void> {
    const request = { newName };
    return this.apiClient.put<void>(`encounter/${encodeURIComponent(encounterId)}/setName`, request);
  }

  // PUT /api/encounter/{encounterId}/turnState
  public async setTurnState(encounterId: string, turnIndex: number, turnCount: number): Promise<void> {
    return this.apiClient.put<void>(`encounter/${encodeURIComponent(encounterId)}/turnState`, { turnIndex, turnCount });
  }

  // PUT /api/encounter/{encounterId}/viewersAllowed
  public async setViewersAllowed(encounterId: string, viewersAllowed: boolean): Promise<void> {
    return this.apiClient.put<void>(`encounter/${encodeURIComponent(encounterId)}/viewersAllowed`, { viewersAllowed });
  }

  // DELETE /api/encounter/{encounterId}
  public async deleteEncounter(encounterId: string): Promise<void> {
    return this.apiClient.instance.delete(`encounter/${encodeURIComponent(encounterId)}`);
  }
}