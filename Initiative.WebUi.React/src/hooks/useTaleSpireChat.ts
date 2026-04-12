import { useEffect, useMemo } from 'react';
import { TaleSpireChatClient } from '../utils/talespireChatClient';

let globalTaleSpireChatClient: TaleSpireChatClient | null = null;
let globalTaleSpireChatClientReferenceCount = 0;

export const useTaleSpireChat = (): TaleSpireChatClient => {
  const chatClient = useMemo(() => {
    if (!globalTaleSpireChatClient) {
      globalTaleSpireChatClient = new TaleSpireChatClient();
    }

    globalTaleSpireChatClientReferenceCount += 1;
    return globalTaleSpireChatClient;
  }, []);

  useEffect(() => {
    chatClient.connect();

    return () => {
      globalTaleSpireChatClientReferenceCount -= 1;

      if (globalTaleSpireChatClientReferenceCount <= 0) {
        chatClient.disconnect();
        globalTaleSpireChatClient = null;
        globalTaleSpireChatClientReferenceCount = 0;
      }
    };
  }, [chatClient]);

  return chatClient;
};