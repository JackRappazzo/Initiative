import React, { useEffect, useState, useCallback, useMemo } from 'react';
import { useParams } from 'react-router-dom';
import { EncounterClient, FetchEncounterResponse } from '../../api/encounterClient';
import { EncounterState } from '../../types';
import { useCreatureManagement, useDragAndDrop } from '../../hooks';
import { CreatureRow, EncounterHeader, EncounterStatus } from '../../components';

import './EditEncounter.css';

const EditEncounter: React.FC = () => {
  const { encounterId } = useParams<{ encounterId: string }>();
  const encounterClient = useMemo(() => new EncounterClient(), []);
  
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
    updateCreature,
    addCreature,
    removeCreature,
    sortByInitiative,
    setCreatureList,
    saveCreatures
  } = useCreatureManagement(encounterId, encounterClient);

  const {
    dragState,
    handleDragStart,
    handleDragEnd,
    handleDragOver,
    handleDrop
  } = useDragAndDrop(creatures, setCreatureList, saveCreatures);

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

  const nextTurn = () => {
    if (creatures.length === 0) return;
    
    setEncounterState(prev => {
      let nextTurn = prev.currentTurn + 1;
      let nextTurnNumber = prev.turnNumber;
      
      if (nextTurn >= creatures.length) {
        nextTurn = 0;
        nextTurnNumber++;
      }
      
      return {
        ...prev,
        currentTurn: nextTurn,
        turnNumber: nextTurnNumber
      };
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
