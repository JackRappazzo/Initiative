// TaleSpire Symbiote API – minimal type declarations for the injected `TS` global.
// The full API is documented at https://symbiote-docs.talespire.com/api_doc_v0_1.md.html

interface TSRollDescriptor {
  name: string;
  roll: string;
}

export interface TaleSpireChatMessageReceived {
  senderPlayerId: string;
  sentAs: 'creature' | 'player' | 'unknown';
  sentAsId: string;
  sentAsName: string;
  sentFromBoardId: string;
  body: string;
}

export interface TaleSpireBridgeEvent<TPayload> {
  kind?: string;
  payload?: TPayload;
}

export type TaleSpireChatTarget = 'board' | 'campaign' | 'gms' | string;

export const TALESPIRE_CHAT_MESSAGE_EVENT = 'initiative:talespire-chat-message';

interface TSStateDiceAPI {
  isValidRollString(rollStr: string): Promise<boolean>;
  makeRollDescriptors(rollString: string): Promise<TSRollDescriptor[]>;
  putDiceInTray(rollDescriptors: TSRollDescriptor[], quietResults: boolean): Promise<string>;
}

interface TSChatAPI {
  send(message: string, target: TaleSpireChatTarget): Promise<void>;
}

interface TSApi {
  dice: TSStateDiceAPI;
  chat: TSChatAPI;
}

declare global {
  interface Window {
    TS?: TSApi;
  }
}

/** Returns true when the app is running inside a TaleSpire Symbiote WebView. */
export function isTaleSpire(): boolean {
  return typeof window !== 'undefined' && typeof window.TS !== 'undefined';
}

export function isTaleSpireChatMessageReceived(value: unknown): value is TaleSpireChatMessageReceived {
  if (!value || typeof value !== 'object') {
    return false;
  }

  const candidate = value as TaleSpireChatMessageReceived;
  return typeof candidate.senderPlayerId === 'string'
    && typeof candidate.sentAs === 'string'
    && typeof candidate.sentAsId === 'string'
    && typeof candidate.sentAsName === 'string'
    && typeof candidate.sentFromBoardId === 'string'
    && typeof candidate.body === 'string';
}

export function unwrapTaleSpireChatBridgeEvent(
  eventData: TaleSpireBridgeEvent<TaleSpireChatMessageReceived> | TaleSpireChatMessageReceived,
): TaleSpireChatMessageReceived | null {
  if (isTaleSpireChatMessageReceived(eventData)) {
    return eventData;
  }

  if (eventData && typeof eventData === 'object' && isTaleSpireChatMessageReceived(eventData.payload)) {
    return eventData.payload;
  }

  return null;
}

/**
 * TaleSpire's roll parser is strict about whitespace around operators.
 * Normalize display text like "1d10 + 5" into "1d10+5" before parsing.
 */
function normalizeRollExpression(expression: string): string {
  return expression.replace(/\s+/g, '').trim();
}

/**
 * Puts a dice roll into the TaleSpire dice tray.
 * @param expression A dice expression e.g. "2d6+3"
 * @param label      The name shown alongside the roll result in TaleSpire
 */
export async function rollInTray(expression: string, label: string): Promise<void> {
  if (!window.TS) return;
  const normalized = normalizeRollExpression(expression);
  const descriptors = await window.TS.dice.makeRollDescriptors(normalized);
  // makeRollDescriptors splits on '/' — assign the label to each group
  const named = descriptors.map((d) => ({ ...d, name: label }));
  await window.TS.dice.putDiceInTray(named, false);
}

export async function sendChatMessage(message: string, target: TaleSpireChatTarget = 'board'): Promise<void> {
  if (!window.TS?.chat) {
    return;
  }

  await window.TS.chat.send(message, target);
}
