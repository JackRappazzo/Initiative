import { HttpClient } from "./httpClient";

// ── Spell raw data shape (5etools) ────────────────────────────────────────────

export interface SpellTime {
  number: number;
  unit: string;
}

export interface SpellRange {
  type: string;
  distance?: {
    type: string;
    amount?: number;
  };
}

export interface SpellComponents {
  v?: boolean;
  s?: boolean;
  m?: string | { text: string; cost?: number; consume?: boolean };
}

export interface SpellDuration {
  type: string;
  duration?: { type: string; amount?: number };
  concentration?: boolean;
}

export interface SpellHigherLevel {
  type: string;
  name?: string;
  entries: string[];
}

export interface SpellRawData {
  name: string;
  source?: string;
  level: number;
  school: string;
  time?: SpellTime[];
  range?: SpellRange;
  components?: SpellComponents;
  duration?: SpellDuration[];
  entries?: string[];
  entriesHigherLevel?: SpellHigherLevel[];
}

export interface SpellDetail {
  id: string;
  name: string;
  spellSourceId: string;
  source?: string;
  school?: string;
  rawData: SpellRawData;
}

// ── Client ────────────────────────────────────────────────────────────────────

export class SpellClient {
  private apiClient: HttpClient;

  constructor() {
    this.apiClient = HttpClient.GetInstance();
  }

  public async resolveSpell(name: string, source?: string): Promise<SpellDetail | null> {
    try {
      const params = new URLSearchParams({ name });
      if (source) params.set('source', source);
      const response = await this.apiClient.instance.get<SpellDetail>(
        `spell/spells/resolve?${params.toString()}`
      );
      return response.data;
    } catch {
      return null;
    }
  }
}

export const spellClient = new SpellClient();
