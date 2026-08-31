import { useEffect, useMemo, useRef, useState } from 'react';
import { DndBeyondClient, DndBeyondRateLimitedError } from '../api/dndBeyondClient';
import { EditableCreature } from '../types';
import { useDndBeyondSession } from '../contexts/DndBeyondContext';

export interface DndBeyondHpSnapshot {
  currentHP: number;
  maxHP: number;
  temporaryHP: number;
}

const MAX_BACKOFF_MS = 5 * 60 * 1000;

export const useDndBeyondHealthSync = (
  creatures: EditableCreature[],
  intervalMs: number = 30000
): { hpById: Record<string, DndBeyondHpSnapshot | undefined> } => {
  const { token } = useDndBeyondSession();
  const [hpById, setHpById] = useState<Record<string, DndBeyondHpSnapshot | undefined>>({});
  const clientRef = useRef<DndBeyondClient | null>(null);
  if (clientRef.current === null) {
    clientRef.current = new DndBeyondClient();
  }

  const linked = useMemo(
    () =>
      creatures.filter(
        (creature) => creature.isPlayer && !!creature.dndBeyondCharacterId && !!creature.isHpLinkedToDndBeyond
      ),
    [creatures]
  );

  const linkedRef = useRef(linked);
  linkedRef.current = linked;

  const linkedKey = useMemo(
    () => linked.map((creature) => creature.dndBeyondCharacterId).sort().join('|'),
    [linked]
  );

  useEffect(() => {
    if (!token || linkedKey === '') return;

    let cancelled = false;
    let timerId: number;

    const schedule = (delayMs: number) => {
      timerId = window.setTimeout(poll, delayMs);
    };

    const poll = async () => {
      if (cancelled) return;

      let delayMs = intervalMs;

      for (const creature of linkedRef.current) {
        const characterId = creature.dndBeyondCharacterId!;
        try {
          const detail = await clientRef.current!.getCharacter(characterId);
          if (cancelled) return;
          setHpById((prev) => ({
            ...prev,
            [characterId]: {
              currentHP: detail.currentHP,
              maxHP: detail.maxHP,
              temporaryHP: detail.temporaryHP,
            },
          }));
        } catch (error) {
          if (error instanceof DndBeyondRateLimitedError) {
            delayMs = Math.max(intervalMs, Math.min(error.retryAfterSeconds * 1000, MAX_BACKOFF_MS));
            break;
          }
          // Ignore individual failures and keep the last known value.
        }
      }

      if (!cancelled) {
        schedule(delayMs);
      }
    };

    poll();
    return () => {
      cancelled = true;
      window.clearTimeout(timerId);
    };
  }, [token, linkedKey, intervalMs]);

  return { hpById };
};
