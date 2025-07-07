import React, { useEffect, useState, useCallback, useMemo } from 'react';
import { useParams } from 'react-router-dom';
import { EncounterClient, FetchEncounterResponse } from '../../api/encounterClient';
import { EncounterState } from '../../types';
import { useCreatureManagement, useDragAndDrop, useLobbyConnection } from '../../hooks';
import { CreatureRow, EncounterHeader, EncounterStatus } from '../../components';
import { useUser } from '../../contexts/UserContext';

import './EditEncounter.css';

const EditEncounter: React.FC = () => {
  const { encounterId } = useParams<{ encounterId: string }>();
  const encounterClient = useMemo(() => new EncounterClient(), []);
  const { userInfo } = useUser();
  
  // Get room code from UserContext
  const roomCode = userInfo?.roomCode;

  console.log('[EditEncounter] Room code from UserContext:', roomCode);
  console.log('[EditEncounter] User info:', userInfo);

  // Use the lobby connection hook - handles all the complex logic
  const { lobbyClient } = useLobbyConnection({
    roomCode,
    autoConnect: true,
    autoJoinRoom: true
  });

  const [encounter, setEncounter] = useState<FetchEncounterResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [editingName, setEditingName] = useState(false);
  const [newName, setNewName] = useState('');
  const [encounterState, setEncounterState] = useState<EncounterState>({
    isRunning: false,
    currentTurn: 0,
    turnNumber: 1
  });

  const {
    creatures,
    error,
    setError,
    updateCreature: originalUpdateCreature,
    addCreature: originalAddCreature,
    removeCreature: originalRemoveCreature,
    sortByInitiative: originalSortByInitiative,
    setCreatureList,
    saveCreatures
  } = useCreatureManagement(encounterId, encounterClient);

  const {
    dragState,
    handleDragStart,
    handleDragEnd: originalHandleDragEnd,
    handleDragOver,
    handleDrop
  } = useDragAndDrop(creatures, setCreatureList, saveCreatures);

  // Method to send lobby state
  const sendLobbyState = useCallback(async (
    creatureList: typeof creatures, 
    currentTurn: number, 
    turnNumber: number,
    isRunning: boolean = encounterState.isRunning
  ) => {
    if (!lobbyClient) {
      console.log('[EditEncounter] Not sending lobby state - no lobby client');
      return;
    }

    try {
      const creatureNames = creatureList.map(creature => creature.name || 'Unnamed Creature');
      const lobbyMode = isRunning ? 'InProgress' : 'Waiting';
      
      console.log('[EditEncounter] Sending lobby state:', {
        creatures: creatureNames,
        currentCreatureIndex: currentTurn,
        currentTurn: turnNumber,
        mode: lobbyMode
      });

      await lobbyClient.setLobbyState(
        creatureNames,
        currentTurn,
        turnNumber,
        lobbyMode
      );
      
      console.log('[EditEncounter] Lobby state sent successfully');
    } catch (err) {
      console.error('[EditEncounter] Failed to send lobby state:', err);
    }
  }, [encounterState.isRunning, lobbyClient]);

  // Enhanced creature management functions that send lobby state updates
  const updateCreature = useCallback((index: number, creature: any) => {
    originalUpdateCreature(index, creature);
    
    // Send lobby state if creature name changed (affects display) and encounter is running
    const oldCreature = creatures[index];
    if (encounterState.isRunning && oldCreature && oldCreature.name !== creature.name) {
      const updatedCreatures = [...creatures];
      updatedCreatures[index] = creature;
      
      setTimeout(() => {
        sendLobbyState(updatedCreatures, encounterState.currentTurn, encounterState.turnNumber, encounterState.isRunning);
      }, 50);
    }
  }, [originalUpdateCreature, creatures, encounterState.isRunning, encounterState.currentTurn, encounterState.turnNumber, sendLobbyState]);

  // Enhanced creature management functions that send lobby state updates
  const addCreature = useCallback(() => {
    originalAddCreature();
    
    // Send lobby state after creature is added (if encounter is running)
    if (encounterState.isRunning) {
      // Use setTimeout to ensure the creature is added to state first
      setTimeout(() => {
        sendLobbyState([...creatures, { 
          name: 'New Creature', 
          hitPoints: 10, 
          maximumHitPoints: 10, 
          armorClass: 10, 
          initiative: 10, 
          initiativeModifier: 0, 
          isEditing: true 
        }], encounterState.currentTurn, encounterState.turnNumber, encounterState.isRunning);
      }, 50);
    }
  }, [originalAddCreature, encounterState.isRunning, encounterState.currentTurn, encounterState.turnNumber, creatures, sendLobbyState]);

  const removeCreature = useCallback(async (index: number) => {
    await originalRemoveCreature(index);
    
    // Send lobby state after creature is removed (if encounter is running)
    if (encounterState.isRunning) {
      const newCreatures = creatures.filter((_, i) => i !== index);
      let newCurrentTurn = encounterState.currentTurn;
      
      // Adjust current turn if needed
      if (encounterState.currentTurn >= newCreatures.length && newCreatures.length > 0) {
        newCurrentTurn = 0;
      } else if (encounterState.currentTurn > index) {
        newCurrentTurn = encounterState.currentTurn - 1;
      }
      
      // Update the encounter state with the new current turn
      if (newCurrentTurn !== encounterState.currentTurn) {
        setEncounterState(prev => ({
          ...prev,
          currentTurn: newCurrentTurn
        }));
      }
      
      sendLobbyState(newCreatures, newCurrentTurn, encounterState.turnNumber, encounterState.isRunning);
    }
  }, [originalRemoveCreature, encounterState.isRunning, encounterState.currentTurn, encounterState.turnNumber, creatures, sendLobbyState]);

  const sortByInitiative = useCallback(async () => {
    await originalSortByInitiative();
    
    // Send lobby state after sorting (if encounter is running)
    if (encounterState.isRunning) {
      // Get the sorted creatures
      const sortedCreatures = [...creatures].sort((a, b) => 
        (b.initiative + b.initiativeModifier) - (a.initiative + a.initiativeModifier)
      );
      
      sendLobbyState(sortedCreatures, encounterState.currentTurn, encounterState.turnNumber, encounterState.isRunning);
    }
  }, [originalSortByInitiative, encounterState.isRunning, encounterState.currentTurn, encounterState.turnNumber, creatures, sendLobbyState]);

  // Enhanced drag and drop that sends lobby state updates
  const handleDragEnd = useCallback(() => {
    originalHandleDragEnd();
    
    // Send lobby state after drag and drop reorder (if encounter is running)
    if (encounterState.isRunning) {
      // Use setTimeout to ensure the reorder is applied first
      setTimeout(() => {
        sendLobbyState(creatures, encounterState.currentTurn, encounterState.turnNumber, encounterState.isRunning);
      }, 50);
    }
  }, [originalHandleDragEnd, encounterState.isRunning, encounterState.currentTurn, encounterState.turnNumber, creatures, sendLobbyState]);

  const loadEncounter = useCallback(async () => {
    if (!encounterId) return;

    try {
      const data = await encounterClient.getEncounter(encounterId);
      setEncounter(data);
      setNewName(data.displayName);
      setCreatureList(data.creatures.map(c => ({ ...c, isEditing: false })));
      setError(null);
    } catch (err) {
      setError('Failed to load encounter');
      console.error('Error loading encounter:', err);
    } finally {
      setLoading(false);
    }
  }, [encounterId, encounterClient, setCreatureList, setError]);

  useEffect(() => {
    loadEncounter();
  }, [loadEncounter]);

  const handleNameEdit = async () => {
    if (!encounter || !encounterId || !newName.trim()) return;
    
    try {
      await encounterClient.renameEncounter(encounterId, newName.trim());
      setEncounter({ ...encounter, displayName: newName.trim() });
      setEditingName(false);
      setError(null);
    } catch (err) {
      setError('Failed to update encounter name');
      console.error('Error updating name:', err);
    }
  };

  const toggleEncounter = () => {
    setEncounterState(prev => ({
      ...prev,
      isRunning: !prev.isRunning,
      currentTurn: prev.isRunning ? 0 : prev.currentTurn,
      turnNumber: prev.isRunning ? 1 : prev.turnNumber
    }));
  };

  // Send lobby state when encounter starts/stops or when encounter state changes
  useEffect(() => {
    if (lobbyClient && creatures.length > 0) {
      console.log('[EditEncounter] Encounter state changed, sending lobby state. IsRunning:', encounterState.isRunning);
      sendLobbyState(creatures, encounterState.currentTurn, encounterState.turnNumber, encounterState.isRunning);
    }
  }, [encounterState.isRunning, encounterState.currentTurn, encounterState.turnNumber, creatures, lobbyClient, sendLobbyState]);

  const nextTurn = () => {
    if (creatures.length === 0) return;
    
    setEncounterState(prev => {
      let nextTurn = prev.currentTurn + 1;
      let nextTurnNumber = prev.turnNumber;
      
      if (nextTurn >= creatures.length) {
        nextTurn = 0;
        nextTurnNumber++;
      }
      
      const newState = {
        ...prev,
        currentTurn: nextTurn,
        turnNumber: nextTurnNumber
      };
      
      // Send lobby state after turn advance if encounter is running
      if (newState.isRunning) {
        sendLobbyState(creatures, nextTurn, nextTurnNumber, newState.isRunning);
      }
      
      return newState;
    });
  };

  if (loading) {
    return <div className="edit-encounter-container">Loading encounter...</div>;
  }

  if (!encounter) {
    return <div className="edit-encounter-container">Encounter not found</div>;
  }

  return (
    <div className="edit-encounter-container">
      <div className="edit-encounter-header">
        <EncounterHeader
          displayName={encounter.displayName}
          editingName={editingName}
          newName={newName}
          onNameChange={setNewName}
          onStartEdit={() => setEditingName(true)}
          onSaveEdit={handleNameEdit}
          onCancelEdit={() => {
            setEditingName(false);
            setNewName(encounter.displayName);
          }}
          onKeyDown={(e) => {
            if (e.key === 'Enter') {
              handleNameEdit();
            } else if (e.key === 'Escape') {
              setEditingName(false);
              setNewName(encounter.displayName);
            }
          }}
        />

        <EncounterStatus
          encounterState={encounterState}
          creatures={creatures}
          onToggleEncounter={toggleEncounter}
          onNextTurn={nextTurn}
        />
      </div>

      {error && (
        <div className="error-message" style={{ color: 'red', marginBottom: '1rem' }}>
          {error}
        </div>
      )}

      <div className="creature-list">
        <div className="creature-list-header">
          <button 
            className="control-button secondary sort-initiative-button"
            onClick={sortByInitiative}
            title="Sort by Initiative (Descending)"
          >
            Sort by Initiative
          </button>
        </div>
        <div className="creature-item creature-header">
          <div></div>
          <div>Name</div>
          <div>HP</div>
          <div>Max HP</div>
          <div>AC</div>
          <div>Init</div>
          <div>Mod</div>
          <div>Actions</div>
        </div>
        
        {creatures.map((creature, index) => (
          <CreatureRow
            key={index}
            creature={creature}
            index={index}
            isCurrentTurn={index === encounterState.currentTurn && encounterState.isRunning}
            isDragging={dragState.draggedCreature === index}
            isDragOver={dragState.dragOverIndex === index}
            dragPosition={dragState.dragPosition}
            onCreatureChange={updateCreature}
            onCreatureRemove={removeCreature}
            onDragStart={handleDragStart}
            onDragEnd={handleDragEnd}
            onDragOver={handleDragOver}
            onDrop={handleDrop}
          />
        ))}
      </div>

      <button className="add-creature-button" onClick={addCreature}>
        Add Creature
      </button>
    </div>
  );
};

export default EditEncounter;
