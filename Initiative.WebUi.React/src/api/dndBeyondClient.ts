import { HttpClient } from "./httpClient";

export interface DndBeyondCharacter {
  id: number;
  name?: string;
}

export interface DndBeyondCharacterDetail {
  id: number;
  name?: string;
  level: number;
  className?: string;
  maxHP: number;
  currentHP: number;
  temporaryHP: number;
}

export class DndBeyondRateLimitedError extends Error {
  retryAfterSeconds: number;

  constructor(message: string, retryAfterSeconds: number) {
    super(message);
    this.name = "DndBeyondRateLimitedError";
    this.retryAfterSeconds = retryAfterSeconds;
  }
}

const TOKEN_STORAGE_KEY = "dndBeyondToken";

export class DndBeyondClient {
  private apiClient: HttpClient;

  constructor() {
    this.apiClient = HttpClient.GetInstance();
  }

  private getToken(): string {
    const token = localStorage.getItem(TOKEN_STORAGE_KEY);
    if (!token) {
      throw new Error("D&D Beyond session token is not set.");
    }
    return token;
  }

  public async getCharacters(): Promise<DndBeyondCharacter[]> {
    try {
      const response = await this.apiClient.instance.get<{ characters: DndBeyondCharacter[] }>(
        "dndbeyond/characters",
        { headers: { "X-DndBeyond-Token": this.getToken() } }
      );
      return response.data.characters;
    } catch (error) {
      throw this.toError(error);
    }
  }

  public async getCharacter(characterId: string | number): Promise<DndBeyondCharacterDetail> {
    try {
      const response = await this.apiClient.instance.get<DndBeyondCharacterDetail>(
        `dndbeyond/characters/${encodeURIComponent(String(characterId))}`,
        { headers: { "X-DndBeyond-Token": this.getToken() } }
      );
      return response.data;
    } catch (error) {
      throw this.toError(error);
    }
  }

  private toError(error: unknown): unknown {
    const retryAfterSeconds = this.parseRetryAfter(error);
    if (retryAfterSeconds !== null) {
      return new DndBeyondRateLimitedError("D&D Beyond rate limited the request.", retryAfterSeconds);
    }
    return error;
  }

  private parseRetryAfter(error: unknown): number | null {
    const err = error as any;
    const status = err?.response?.status;
    if (status !== 429 && status !== 503) {
      return null;
    }

    const header = err?.response?.headers?.["retry-after"];
    if (header !== undefined && header !== null) {
      const seconds = Number(header);
      if (Number.isFinite(seconds)) {
        return Math.max(0, seconds);
      }
    }

    const body = err?.response?.data;
    if (body && typeof body === "object" && typeof body.retryAfterSeconds === "number") {
      return Math.max(0, body.retryAfterSeconds);
    }

    return null;
  }
}
