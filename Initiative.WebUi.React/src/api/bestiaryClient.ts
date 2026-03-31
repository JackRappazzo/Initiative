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

export interface SearchCreaturesParams {
  bestiaryIds?: string[];
  pageSize?: number;
  skip?: number;
}

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

  private buildParams(params: SearchCreaturesParams): Record<string, unknown> {
    const result: Record<string, unknown> = {};
    if (params.bestiaryIds?.length) result["bestiaryIds"] = params.bestiaryIds;
    if (params.pageSize !== undefined) result["pageSize"] = params.pageSize;
    if (params.skip !== undefined) result["skip"] = params.skip;
    return result;
  }
}
