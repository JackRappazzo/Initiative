import { useState, useCallback } from 'react';
import { EditableCreature } from '../types';
import { EncounterClient } from '../api/encounterClient';
import { BestiaryClient, CreatureListItem, FiveEToolsAc } from '../api/bestiaryClient';

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
    setCreatures(newCreatures);
    // Auto-save after a short delay to avoid too many API calls
    setTimeout(() => updateCreatureAPI(newCreatures), 300);
  }, [creatures, updateCreatureAPI]);

  const updateCreatureAndSave = useCallback(async (index: number, creature: EditableCreature) => {
    const newCreatures = [...creatures];
    newCreatures[index] = creature;
    setCreatures(newCreatures);
    await updateCreatureAPI(newCreatures);
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

    const newCreatures = [...creatures, newCreature];
    setCreatures(newCreatures);
    await updateCreatureAPI(newCreatures);
  }, [creatures, updateCreatureAPI, bestiaryClient]);

  const removeCreature = useCallback(async (index: number) => {
    const newCreatures = creatures.filter((_, i) => i !== index);
    setCreatures(newCreatures);
    await updateCreatureAPI(newCreatures);
  }, [creatures, updateCreatureAPI]);

  const sortByInitiative = useCallback(async () => {
    const sortedCreatures = [...creatures].sort((a, b) => b.initiative - a.initiative);
    setCreatures(sortedCreatures);
    await updateCreatureAPI(sortedCreatures);
  }, [creatures, updateCreatureAPI]);

  const setCreatureList = useCallback((newCreatures: EditableCreature[]) => {
    setCreatures(newCreatures);
  }, []);

  const saveCreatures = useCallback(async () => {
    await updateCreatureAPI(creatures);
  }, [creatures, updateCreatureAPI]);

  return {
    creatures,
    error,
    setError,
    updateCreature,
    updateCreatureAndSave,
    addCreatureFromBestiary,
    removeCreature,
    sortByInitiative,
    setCreatureList,
    saveCreatures
  };
};

