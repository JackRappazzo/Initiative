export interface TaleSpireInviteCommand {
  type: 'invite';
  roomCode: string;
}

export interface TaleSpireJoinCommand {
  type: 'join';
}

export type TaleSpireChatCommand = TaleSpireInviteCommand | TaleSpireJoinCommand;

const roomCodePattern = /^[A-Za-z0-9]{6}$/;

export function isValidRoomCode(roomCode: string): boolean {
  return roomCodePattern.test(roomCode);
}

export function parseTaleSpireChatCommand(message: string): TaleSpireChatCommand | null {
  const trimmedMessage = message.trim();

  if (/^@join$/i.test(trimmedMessage)) {
    return { type: 'join' };
  }

  const inviteMatch = trimmedMessage.match(/^@inv\s+([A-Za-z0-9]{6})$/i);
  if (!inviteMatch) {
    return null;
  }

  const roomCode = inviteMatch[1];
  if (!isValidRoomCode(roomCode)) {
    return null;
  }

  return {
    type: 'invite',
    roomCode,
  };
}

export function formatInviteCommand(roomCode: string): string {
  return `@inv ${roomCode}`;
}