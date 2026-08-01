import { HttpClient } from "./httpClient";

export interface ConditionRawData {
  name: string;
  source: string;
  entries?: (string | Record<string, unknown>)[];
  page?: number;
}

export interface ConditionDetail {
  id: string;
  name: string;
  source: string;
  type: string;
  rawData: ConditionRawData;
}

export class ConditionClient {
  private apiClient: HttpClient;

  constructor() {
    this.apiClient = HttpClient.GetInstance();
  }

  public async resolveCondition(name: string): Promise<ConditionDetail | null> {
    try {
      const params = new URLSearchParams({ name });
      const response = await this.apiClient.instance.get<ConditionDetail>(
        `condition/conditions/resolve?${params.toString()}`
      );
      return response.data;
    } catch {
      return null;
    }
  }
}

export const conditionClient = new ConditionClient();
