import {
  formatInviteCommand,
  isValidRoomCode,
  parseTaleSpireChatCommand,
} from './talespireChatCommands';

describe('talespireChatCommands', () => {
  test('parses invite commands with mixed casing and whitespace', () => {
    expect(parseTaleSpireChatCommand('  @InV AbC123  ')).toEqual({
      type: 'invite',
      roomCode: 'AbC123',
    });
  });

  test('parses join commands', () => {
    expect(parseTaleSpireChatCommand(' @join ')).toEqual({ type: 'join' });
  });

  test('rejects malformed invite commands', () => {
    expect(parseTaleSpireChatCommand('@inv ABC12')).toBeNull();
    expect(parseTaleSpireChatCommand('@invite ABC123')).toBeNull();
    expect(parseTaleSpireChatCommand('@inv ABC123 extra')).toBeNull();
  });

  test('validates room codes and formats invite commands', () => {
    expect(isValidRoomCode('A1b2C3')).toBe(true);
    expect(isValidRoomCode('A1b2C')).toBe(false);
    expect(formatInviteCommand('AbC123')).toBe('@inv AbC123');
  });
});