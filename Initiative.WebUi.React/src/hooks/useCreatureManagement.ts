import { useState, useCallback } from 'react';
import { EditableCreature } from '../types';
import { EncounterClient } from '../api/encounterClient';

export const useCreatureManagement = (encounterId: string | undefined, encounterClient: EncounterClient) => {
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

  const addCreature = useCallback(async () => {
    const newCreature: EditableCreature = {
      name: 'New Creature',
      hitPoints: 10,
      maximumHitPoints: 10,
      armorClass: 10,
      initiative: 10,
      initiativeModifier: 0,
      isEditing: true
    };
    
    const newCreatures = [...creatures, newCreature];
    setCreatures(newCreatures);
    await updateCreatureAPI(newCreatures);
  }, [creatures, updateCreatureAPI]);

  const removeCreature = useCallback(async (index: number) => {
    const newCreatures = creatures.filter((_, i) => i !== index);
    setCreatures(newCreatures);
    await updateCreatureAPI(newCreatures);
  }, [creatures, updateCreatureAPI]);

  const sortByInitiative = useCallback(async () => {
    const sortedCreatures = [...creatures].sort((a, b) => 
      (b.initiative + b.initiativeModifier) - (a.initiative + a.initiativeModifier)
    );
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
    addCreature,
    removeCreature,
    sortByInitiative,
    setCreatureList,
    saveCreatures
  };
};
