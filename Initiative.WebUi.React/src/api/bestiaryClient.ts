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
  sortBy?: CreatureSortBy;
  sortDescending?: boolean;
  pageSize?: number;
  skip?: number;
}

// ── 5etools raw data shape ────────────────────────────────────────────────────

export interface FiveEToolsEntry {
  name?: string;
  entries?: (string | FiveEToolsEntry)[];
}

export interface FiveEToolsSpeedEntry {
  walk?: number;
  fly?: number;
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
    headerEntries?: string[];
    will?: string[];
    daily?: Record<string, string[]>;
    ability?: string;
    displayAs?: string;
  }[];
}

export interface CreatureDetail {
  id: string;
  name: string;
  bestiaryId: string;
  source?: string;
  rawData: FiveEToolsRawData;
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

  public async searchCreatures(params: SearchCreaturesParams): Promise<CreatureListItem[]> {
    const response = await this.apiClient.instance.get<{ creatures: CreatureListItem[] }>("bestiary/creatures", {
      params: this.buildParams(params),
    });
    return response.data.creatures;
  }

  public async countCreatures(params: Omit<SearchCreaturesParams, "pageSize" | "skip">): Promise<number> {
    const response = await this.apiClient.instance.get<{ totalCount: number }>("bestiary/creatures/count", {
      params: this.buildParams(params),
    });
    return response.data.totalCount;
  }

  public async getCreatureById(creatureId: string): Promise<CreatureDetail> {
    const response = await this.apiClient.instance.get<CreatureDetail>(
      `bestiary/creatures/${encodeURIComponent(creatureId)}`
    );
    return response.data;
  }

  private buildParams(params: SearchCreaturesParams): Record<string, unknown> {
    const result: Record<string, unknown> = {};
    if (params.bestiaryIds?.length) result["bestiaryIds"] = params.bestiaryIds;
    if (params.name) result["name"] = params.name;
    if (params.sortBy) result["sortBy"] = params.sortBy;
    if (params.sortDescending) result["sortDescending"] = params.sortDescending;
    if (params.pageSize !== undefined) result["pageSize"] = params.pageSize;
    if (params.skip !== undefined) result["skip"] = params.skip;
    return result;
  }
}

