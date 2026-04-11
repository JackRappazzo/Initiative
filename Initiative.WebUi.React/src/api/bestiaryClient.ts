import { HttpClient } from "./httpClient";

export interface BestiaryListItem {
  id: string;
  name: string;
  source?: string;
  ownerId?: string;
}

export interface CreatureListItem {
  id: string;
  name: string;
  bestiaryId: string;
  source?: string;
  creatureType?: string;
  challengeRating?: string;
  isLegendary: boolean;
}

export type CreatureSortBy = 'Name' | 'ChallengeRating' | 'Type';

export interface SearchCreaturesParams {
  bestiaryIds?: string[];
  name?: string;
  creatureType?: string;
  sortBy?: CreatureSortBy;
  sortDescending?: boolean;
  pageSize?: number;
  skip?: number;
}

// ── 5etools raw data shape ────────────────────────────────────────────────────

export interface FiveEToolsEntry {
  type?: string;
  name?: string;
  entries?: (string | FiveEToolsEntry)[];
  items?: (string | FiveEToolsEntry)[];
  style?: string;
}

export interface FiveEToolsSpeedValue {
  number: number;
  condition?: string;
}

export interface FiveEToolsSpeedEntry {
  walk?: number;
  fly?: number | FiveEToolsSpeedValue;
  swim?: number;
  burrow?: number;
  climb?: number;
  canHover?: boolean;
}

export interface FiveEToolsHp {
  average: number;
  formula?: string;
}

export interface FiveEToolsAc {
  ac?: number;
  from?: string[];
  condition?: string;
}

export interface FiveEToolsCr {
  cr: string;
  xpLair?: number;
}

export interface FiveEToolsRawData {
  name: string;
  source?: string;
  size?: string[];
  type?: string | { type: string; tags?: string[] };
  alignment?: string[];
  ac?: (number | FiveEToolsAc)[];
  hp?: FiveEToolsHp;
  speed?: FiveEToolsSpeedEntry;
  str?: number;
  dex?: number;
  con?: number;
  int?: number;
  wis?: number;
  cha?: number;
  save?: Record<string, string>;
  skill?: Record<string, string>;
  passive?: number;
  senses?: string[];
  languages?: string[];
  cr?: string | FiveEToolsCr;
  immune?: (string | { immune: string[]; note?: string })[];
  resist?: (string | { resist: string[]; note?: string })[];
  vulnerable?: (string | { vulnerable: string[]; note?: string })[];
  conditionImmune?: (string | { conditionImmune: string[]; note?: string })[];
  trait?: FiveEToolsEntry[];
  action?: FiveEToolsEntry[];
  bonus?: FiveEToolsEntry[];
  reaction?: FiveEToolsEntry[];
  legendary?: FiveEToolsEntry[];
  legendaryActionsLair?: number;
  spellcasting?: {
    name: string;
    type?: string;
    headerEntries?: string[];
    will?: string[];
    daily?: Record<string, string[]>;
    /** Leveled spell slots: key = level ("1"–"9"), value = { slots, spells } */
    spells?: Record<string, { slots?: number; spells: string[] }>;
    ability?: string;
    displayAs?: string;
    hidden?: string[];
  }[];
}

export interface CreatureDetail {
  id: string;
  name: string;
  bestiaryId: string;
  source?: string;
  rawData: FiveEToolsRawData;
}

export interface CustomCreatureAbilityScores {
  str?: number;
  dex?: number;
  con?: number;
  int?: number;
  wis?: number;
  cha?: number;
}

export interface CustomCreatureSpeed {
  walk?: number;
  fly?: number;
  swim?: number;
  burrow?: number;
  climb?: number;
  canHover?: boolean;
}

export interface CustomCreatureEntry {
  name: string;
  description: string;
}

export interface CustomCreatureSpellcasting {
  ability?: string;
  spellSaveDc?: number;
  spellAttackBonus?: number;
  /** Slot-based: key = "0" (cantrips/at-will) through "9" */
  slotSpells?: Record<string, string[]>;
  /** X/day entries, e.g. "3/day: Fireball" */
  dailySpells?: string[];
}

export interface CustomCreaturePayload {
  name: string;
  size?: string;
  creatureType?: string;
  subtype?: string;
  alignment?: string;
  challengeRating?: string;
  isLegendary: boolean;
  proficiencyBonus?: number;
  /** Flat HP value — neither hp nor hitDice is required */
  hp?: number;
  /** Hit dice formula, e.g. "8d8+16" — independent of hp */
  hitDice?: string;
  ac?: number;
  acNote?: string;
  abilityScores?: CustomCreatureAbilityScores;
  speed?: CustomCreatureSpeed;
  /** key = ability shorthand e.g. "str", value = modifier string e.g. "+5" */
  savingThrows?: Record<string, string>;
  /** key = skill name, value = modifier string */
  skills?: Record<string, string>;
  damageResistances?: string[];
  damageImmunities?: string[];
  damageVulnerabilities?: string[];
  conditionImmunities?: string[];
  senses?: string[];
  languages?: string[];
  traits?: string;
  actions?: CustomCreatureEntry[];
  bonusActions?: CustomCreatureEntry[];
  reactions?: CustomCreatureEntry[];
  legendaryActions?: CustomCreatureEntry[];
  legendaryActionCount?: number;
  spellcasting?: CustomCreatureSpellcasting;
}

// ── Client ────────────────────────────────────────────────────────────────────

export class BestiaryClient {
  private apiClient: HttpClient;

  constructor() {
    this.apiClient = HttpClient.GetInstance();
  }

  public async getAvailableBestiaries(): Promise<BestiaryListItem[]> {
    const response = await this.apiClient.instance.get<{ bestiaries: BestiaryListItem[] }>("bestiary");
    return response.data.bestiaries;
  }

  public async searchCreatures(params: SearchCreaturesParams): Promise<{ creatures: CreatureListItem[]; totalCount: number }> {
    const response = await this.apiClient.instance.get<{ creatures: CreatureListItem[]; totalCount: number }>("bestiary/creatures", {
      params: this.buildParams(params),
    });
    return response.data;
  }

  public async getCreatureById(creatureId: string): Promise<CreatureDetail> {
    const response = await this.apiClient.instance.get<CreatureDetail>(
      `bestiary/creatures/${encodeURIComponent(creatureId)}`
    );
    return response.data;
  }

  public async createBestiary(name: string): Promise<{ id: string; name: string }> {
    const response = await this.apiClient.instance.post<{ id: string; name: string }>("bestiary", { name });
    return response.data;
  }

  public async renameBestiary(bestiaryId: string, name: string): Promise<void> {
    await this.apiClient.instance.put(`bestiary/${encodeURIComponent(bestiaryId)}`, { name });
  }

  public async deleteBestiary(bestiaryId: string): Promise<void> {
    await this.apiClient.instance.delete(`bestiary/${encodeURIComponent(bestiaryId)}`);
  }

  public async createCustomCreature(bestiaryId: string, payload: CustomCreaturePayload): Promise<{ id: string; name: string }> {
    const response = await this.apiClient.instance.post<{ id: string; name: string }>(
      `bestiary/${encodeURIComponent(bestiaryId)}/creatures`,
      payload
    );
    return response.data;
  }

  public async updateCustomCreature(bestiaryId: string, creatureId: string, payload: CustomCreaturePayload): Promise<void> {
    await this.apiClient.instance.put(
      `bestiary/${encodeURIComponent(bestiaryId)}/creatures/${encodeURIComponent(creatureId)}`,
      payload
    );
  }

  public async deleteCustomCreature(bestiaryId: string, creatureId: string): Promise<void> {
    await this.apiClient.instance.delete(
      `bestiary/${encodeURIComponent(bestiaryId)}/creatures/${encodeURIComponent(creatureId)}`
    );
  }

  private buildParams(params: SearchCreaturesParams): Record<string, unknown> {
    const result: Record<string, unknown> = {};
    if (params.bestiaryIds !== undefined) result["bestiaryIds"] = params.bestiaryIds;
    if (params.name) result["name"] = params.name;
    if (params.creatureType) result["creatureType"] = params.creatureType;
    if (params.sortBy) result["sortBy"] = params.sortBy;
    if (params.sortDescending) result["sortDescending"] = params.sortDescending;
    if (params.pageSize !== undefined) result["pageSize"] = params.pageSize;
    if (params.skip !== undefined) result["skip"] = params.skip;
    return result;
  }
}

