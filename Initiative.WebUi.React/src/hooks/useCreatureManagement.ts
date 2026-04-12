import { useState, useCallback } from 'react';
import { EditableCreature } from '../types';
import { EncounterClient } from '../api/encounterClient';
import { BestiaryClient, CreatureListItem, FiveEToolsAc } from '../api/bestiaryClient';

const sortCreaturesByInitiative = (creatureList: EditableCreature[]) =>
  creatureList
    .map((creature, index) => ({ creature, index }))
    .sort((left, right) => {
      const initiativeDelta = right.creature.initiative - left.creature.initiative;
      return initiativeDelta !== 0 ? initiativeDelta : left.index - right.index;
    })
    .map((entry) => entry.creature);

export const useCreatureManagement = (encounterId: string | undefined, encounterClient: EncounterClient, bestiaryClient?: BestiaryClient) => {
  const [creatures, setCreatures] = useState<EditableCreature[]>([]);
  const [error, setError] = useState<string | null>(null);

  const updateCreatureAPI = useCallback(async (newCreatures: EditableCreature[]) => {
    if (!encounterId) return;

    try {
      await encounterClient.setCreatures(encounterId, newCreatures);
      setError(null);
    } catch (err) {
      setError('Failed to update creatures');
      console.error('Error updating creatures:', err);
    }
  }, [encounterId, encounterClient]);

  const updateCreature = useCallback((index: number, creature: EditableCreature) => {
    const newCreatures = [...creatures];
    newCreatures[index] = creature;
    const sortedCreatures = sortCreaturesByInitiative(newCreatures);
    setCreatures(sortedCreatures);
    // Auto-save after a short delay to avoid too many API calls
    setTimeout(() => updateCreatureAPI(sortedCreatures), 300);
  }, [creatures, updateCreatureAPI]);

  const updateCreatureAndSave = useCallback(async (index: number, creature: EditableCreature) => {
    const newCreatures = [...creatures];
    newCreatures[index] = creature;
    const sortedCreatures = sortCreaturesByInitiative(newCreatures);
    setCreatures(sortedCreatures);
    await updateCreatureAPI(sortedCreatures);
  }, [creatures, updateCreatureAPI]);

  const addCreatureFromBestiary = useCallback(async (source: CreatureListItem) => {
    const existingCount = creatures.filter(c => c.creatureName === source.name).length;
    const displayName = `${source.name} ${existingCount + 1}`;

    let maxHP = 0;
    let currentHP = 0;
    let ac = 0;
    let initiativeModifier = 0;

    if (bestiaryClient) {
      try {
        const detail = await bestiaryClient.getCreatureById(source.id);
        const raw = detail.rawData;

        // Parse HP
        maxHP = raw.hp?.average ?? 0;
        currentHP = maxHP;

        // Parse AC: entries can be a bare number or an object { ac, from, ... }
        if (raw.ac && raw.ac.length > 0) {
          const first = raw.ac[0];
          ac = typeof first === 'number' ? first : (first as FiveEToolsAc).ac ?? 0;
        }

        // Initiative modifier = DEX modifier = floor((dex - 10) / 2)
        if (raw.dex !== undefined) {
          initiativeModifier = Math.floor((raw.dex - 10) / 2);
        }
      } catch {
        // fall through with defaults
      }
    }

    const newCreature: EditableCreature = {
      isPlayer: false,
      displayName,
      creatureName: source.name,
      creatureId: source.id,
      initiative: 0,
      initiativeModifier,
      maxHP,
      currentHP,
      ac,
      isEditing: false
    };

    const newCreatures = sortCreaturesByInitiative([...creatures, newCreature]);
    setCreatures(newCreatures);
    await updateCreatureAPI(newCreatures);
  }, [creatures, updateCreatureAPI, bestiaryClient]);

  const removeCreature = useCallback(async (index: number) => {
    const newCreatures = sortCreaturesByInitiative(creatures.filter((_, i) => i !== index));
    setCreatures(newCreatures);
    await updateCreatureAPI(newCreatures);
  }, [creatures, updateCreatureAPI]);

  const setCreatureList = useCallback((newCreatures: EditableCreature[]) => {
    setCreatures(sortCreaturesByInitiative(newCreatures));
  }, []);

  const saveCreatures = useCallback(async () => {
    const sortedCreatures = sortCreaturesByInitiative(creatures);
    setCreatures(sortedCreatures);
    await updateCreatureAPI(sortedCreatures);
  }, [creatures, updateCreatureAPI]);

  return {
    creatures,
    error,
    setError,
    updateCreature,
    updateCreatureAndSave,
    addCreatureFromBestiary,
    removeCreature,
    setCreatureList,
    saveCreatures
  };
};

