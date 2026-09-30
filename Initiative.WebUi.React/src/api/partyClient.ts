import { HttpClient } from "./httpClient";
import { FiveEToolsRawData } from "./bestiaryClient";

export type SidekickClass = 'Expert' | 'Spellcaster' | 'Warrior';
export type SidekickSpellcasterKind = 'Magician' | 'Healer' | 'Prodigy';
export type SidekickWarriorRole = 'Attacker' | 'Defender';

export interface SidekickAbilityScores {
  strength: number;
  dexterity: number;
  constitution: number;
  intelligence: number;
  wisdom: number;
  charisma: number;
  strengthModifier?: number;
  dexterityModifier?: number;
  constitutionModifier?: number;
  intelligenceModifier?: number;
  wisdomModifier?: number;
  charismaModifier?: number;
}

export interface SidekickHpRoll {
  level: number;
  isFixed: boolean;
  dieValue: number;
  constitutionModifier: number;
  result: number;
}

export interface SidekickChoice {
  level: number;
  type: string;
  value?: string | null;
  values: string[];
}

export interface SidekickSpellcasting {
  kind: SidekickSpellcasterKind;
  ability: string;
  cantrips: string[];
  slots: Record<number, number>;
  knownSpells: Record<number, string[]>;
}

export interface Sidekick {
  baseCreatureId: string;
  baseCreatureName: string;
  class: SidekickClass;
  level: number;
  abilityScores: SidekickAbilityScores;
  maxHitPoints: number;
  hitDiceCount: number;
  hitDieSize: string;
  hpRolls: SidekickHpRoll[];
  spellcasting?: SidekickSpellcasting | null;
  choices: SidekickChoice[];
  rawData: FiveEToolsRawData;
}

export interface PartyMember {
  id?: string;
  name: string;
  level: number;
  dndBeyondCharacterId?: string;
  sidekick?: Sidekick | null;
}

export interface Party {
  partyId: string;
  name: string;
  members: PartyMember[];
}

export interface SaveSidekickPayload {
  name: string;
  baseCreatureId: string;
  baseCreatureName?: string;
  class: SidekickClass;
  level: number;
  abilityScores: SidekickAbilityScores;
  hpRolls: SidekickHpRoll[];
  spellcasterKind?: SidekickSpellcasterKind;
  warriorRole?: SidekickWarriorRole;
  cantrips: string[];
  knownSpells: Record<number, string[]>;
  choices: SidekickChoice[];
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

  // PUT /api/party/{partyId}/members
  public async updatePartyMembers(partyId: string, members: PartyMember[]): Promise<void> {
    return this.apiClient.put<void>(`party/${encodeURIComponent(partyId)}/members`, { members });
  }

  // POST /api/party/{partyId}/sidekicks
  public async createSidekick(partyId: string, payload: SaveSidekickPayload): Promise<{ memberId: string }> {
    return this.apiClient.post<{ memberId: string }>(`party/${encodeURIComponent(partyId)}/sidekicks`, payload);
  }

  // PUT /api/party/{partyId}/sidekicks/{memberId}
  public async updateSidekick(partyId: string, memberId: string, payload: SaveSidekickPayload): Promise<void> {
    return this.apiClient.put<void>(`party/${encodeURIComponent(partyId)}/sidekicks/${encodeURIComponent(memberId)}`, payload);
  }

  // DELETE /api/party/{partyId}/sidekicks/{memberId}
  public async deleteSidekick(partyId: string, memberId: string): Promise<void> {
    return this.apiClient.delete<void>(`party/${encodeURIComponent(partyId)}/sidekicks/${encodeURIComponent(memberId)}`);
  }

  // DELETE /api/party/{partyId}
  public async deleteParty(partyId: string): Promise<void> {
    return this.apiClient.delete<void>(`party/${encodeURIComponent(partyId)}`);
  }
}
