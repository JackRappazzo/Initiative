import { Subject } from 'rxjs';
import {
  sendChatMessage,
  TALESPIRE_CHAT_MESSAGE_EVENT,
  TaleSpireBridgeEvent,
  TaleSpireChatMessageReceived,
  TaleSpireChatTarget,
  unwrapTaleSpireChatBridgeEvent,
} from './talespire';

type TaleSpireChatCustomEvent = CustomEvent<
  TaleSpireBridgeEvent<TaleSpireChatMessageReceived> | TaleSpireChatMessageReceived
>;

type TaleSpireChatHandler = (
  eventData: TaleSpireBridgeEvent<TaleSpireChatMessageReceived> | TaleSpireChatMessageReceived | string,
  payload?: TaleSpireChatMessageReceived,
) => void;

type InitiativeBridgeWindow = Window & {
  handleInitiativeChatMessage?: TaleSpireChatHandler;
};

export class TaleSpireChatClient {
  public readonly chatMessages$ = new Subject<TaleSpireChatMessageReceived>();

  private isConnected = false;

  private readonly handleChatBridgeEvent = (event: Event) => {
    const bridgeEvent = event as TaleSpireChatCustomEvent;
    const chatMessage = unwrapTaleSpireChatBridgeEvent(bridgeEvent.detail);
    if (!chatMessage) {
      return;
    }

    this.chatMessages$.next(chatMessage);
  };

  private ensureGlobalChatHandlerRegistered(): void {
    const bridgeWindow = window as InitiativeBridgeWindow;
    if (typeof bridgeWindow.handleInitiativeChatMessage === 'function') {
      return;
    }

    bridgeWindow.handleInitiativeChatMessage = (eventData, payload) => {
      const detail = typeof payload !== 'undefined'
        ? { kind: eventData, payload }
        : eventData;

      window.dispatchEvent(new CustomEvent(TALESPIRE_CHAT_MESSAGE_EVENT, { detail }));
    };
  }

  public connect(): void {
    if (this.isConnected) {
      return;
    }

    this.ensureGlobalChatHandlerRegistered();
    window.addEventListener(TALESPIRE_CHAT_MESSAGE_EVENT, this.handleChatBridgeEvent as EventListener);
    this.isConnected = true;
  }

  public disconnect(): void {
    if (!this.isConnected) {
      return;
    }

    window.removeEventListener(TALESPIRE_CHAT_MESSAGE_EVENT, this.handleChatBridgeEvent as EventListener);
    this.isConnected = false;
  }

  public async send(message: string, target: TaleSpireChatTarget = 'board'): Promise<void> {
    await sendChatMessage(message, target);
  }
}