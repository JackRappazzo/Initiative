import React, { useEffect, useState, useCallback, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { EncounterClient, CreatureJsonModel, FetchEncounterResponse } from '../../../api/encounterClient';
import { NumericInput } from '../../../components/NumericInput';

import './EditEncounter.css';

interface EditableCreature extends CreatureJsonModel {
  isEditing?: boolean;
}

const EditEncounter: React.FC = () => {
  const { encounterId } = useParams<{ encounterId: string }>();
  const navigate = useNavigate();
  const encounterClient = useMemo(() => new EncounterClient(), []);
  
  const [encounter, setEncounter] = useState<FetchEncounterResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [editingName, setEditingName] = useState(false);
  const [newName, setNewName] = useState('');
  const [creatures, setCreatures] = useState<EditableCreature[]>([]);
  const [isRunning, setIsRunning] = useState(false);
  const [currentTurn, setCurrentTurn] = useState(0);
  const [turnNumber, setTurnNumber] = useState(1);
  const [draggedCreature, setDraggedCreature] = useState<number | null>(null);
  const [dragOverIndex, setDragOverIndex] = useState<number | null>(null);
  const [dragPosition, setDragPosition] = useState<'top' | 'bottom' | null>(null);

  const loadEncounter = useCallback(async () => {
    if (!encounterId) 
      return;

    try {
      const data = await encounterClient.getEncounter(encounterId);
      setEncounter(data);
      setNewName(data.displayName);
      setCreatures(data.creatures.map(c => ({ ...c, isEditing: false })));
      setError(null);
    } catch (err) {
      setError('Failed to load encounter');
      console.error('Error loading encounter:', err);
    } finally {
      setLoading(false);
    }
  }, [encounterId, encounterClient]);

  useEffect(() => {
    loadEncounter();
  }, [loadEncounter]);

  const handleNameEdit = async () => {
    if (!encounter || !encounterId || !newName.trim()) 
      return;
    
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

  const handleCreatureUpdate = async () => {
    if (!encounterId) 
      return;

    try {
      await encounterClient.setCreatures(encounterId, creatures);
      setError(null);
    } catch (err) {
      setError('Failed to update creatures');
      console.error('Error updating creatures:', err);
    }
  };

  const handleDragStart = (index: number) => {
    setDraggedCreature(index);
  };

  const handleDragEnd = () => {
    setDraggedCreature(null);
    setDragOverIndex(null);
    setDragPosition(null);
  };

  const handleDragOver = (e: React.DragEvent, index: number) => {
    e.preventDefault();
    if (draggedCreature === null || draggedCreature === index) return;

    const rect = (e.currentTarget as HTMLElement).getBoundingClientRect();
    const midpoint = rect.top + rect.height / 2;
    const position = e.clientY < midpoint ? 'top' : 'bottom';
    
    setDragOverIndex(index);
    setDragPosition(position);

    // Update the order in real-time
    const dropIndex = position === 'bottom' ? index + 1 : index;
    if (dropIndex !== draggedCreature && dropIndex !== draggedCreature + 1) {
      const newCreatures = [...creatures];
      const [draggedItem] = newCreatures.splice(draggedCreature, 1);
      const adjustedDropIndex = dropIndex > draggedCreature ? dropIndex - 1 : dropIndex;
      newCreatures.splice(adjustedDropIndex, 0, draggedItem);
      setCreatures(newCreatures);
      setDraggedCreature(adjustedDropIndex);
    }
  };

  const handleDrop = async () => {
    setDraggedCreature(null);
    setDragOverIndex(null);
    setDragPosition(null);
    await handleCreatureUpdate();
  };

  const addCreature = () => {
    const newCreature: EditableCreature = {
      name: 'New Creature',
      hitPoints: 10,
      maximumHitPoints: 10,
      armorClass: 10,
      initiative: 10,
      initiativeModifier: 0,
      isEditing: true
    };
    
    setCreatures([...creatures, newCreature]);
  };

  const removeCreature = (index: number) => {
    const newCreatures = creatures.filter((_, i) => i !== index);
    setCreatures(newCreatures);
    handleCreatureUpdate();
  };

  const toggleEncounter = () => {
    if (isRunning) {
      setIsRunning(false);
    } else {
      setIsRunning(true);
      setCurrentTurn(0);
      setTurnNumber(1);
    }
  };

  const nextTurn = () => {
    if (creatures.length === 0) 
      return;
    
    let nextTurn = currentTurn + 1;
    let nextTurnNumber = turnNumber;
    
    if (nextTurn >= creatures.length) {
      nextTurn = 0;
      nextTurnNumber++;
    }
    
    setCurrentTurn(nextTurn);
    setTurnNumber(nextTurnNumber);
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
        <div className="encounter-name">
          {editingName ? (
            <>
              <input
                type="text"
                value={newName}
                onChange={(e) => setNewName(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    handleNameEdit();
                  } else if (e.key === 'Escape') {
                    setEditingName(false);
                    setNewName(encounter.displayName);
                  }
                }}
                autoFocus
              />
              <button className="control-button primary" onClick={handleNameEdit}>Save</button>
              <button className="control-button secondary" onClick={() => setEditingName(false)}>Cancel</button>
            </>
          ) : (
            <>
              <h1>{encounter.displayName}</h1>
              <button className="control-button secondary" onClick={() => setEditingName(true)}>
                Edit Name
              </button>
            </>
          )}
        </div>

        <button 
          className={`control-button ${isRunning ? 'danger' : 'primary'}`}
          onClick={toggleEncounter}
        >
          {isRunning ? 'End Encounter' : 'Start Encounter'}
        </button>
      </div>

      {error && (
        <div className="error-message" style={{ color: 'red', marginBottom: '1rem' }}>
          {error}
        </div>
      )}

      <div className="creature-list">
        <div className="creature-item creature-header">
          <div>Name</div>
          <div>HP</div>
          <div>Max HP</div>
          <div>AC</div>
          <div>Init</div>
          <div>Mod</div>
          <div>Actions</div>
        </div>
        
        {creatures.map((creature, index) => (
          <div 
            key={index} 
            className={`creature-item ${
              draggedCreature === index ? 'dragging' : ''
            } ${
              dragOverIndex === index && dragPosition === 'top' ? 'drag-over-top' : ''
            } ${
              dragOverIndex === index && dragPosition === 'bottom' ? 'drag-over-bottom' : ''
            }`}
            style={{
              ...index === currentTurn && isRunning ? { background: '#e9ecef' } : {},
              cursor: 'move'
            }}
            draggable
            onDragStart={() => handleDragStart(index)}
            onDragOver={(e) => handleDragOver(e, index)}
            onDrop={handleDrop}
            onDragEnd={handleDragEnd}
          >
            <input
              type="text"
              value={creature.name}
              onChange={(e) => {
                const newCreatures = [...creatures];
                newCreatures[index] = { ...creature, name: e.target.value };
                setCreatures(newCreatures);
              }}
              onBlur={handleCreatureUpdate}
            />
            <NumericInput
              value={creature.hitPoints}
              onChange={(newHP) => {
                const newCreatures = [...creatures];
                newCreatures[index] = { ...creature, hitPoints: newHP };
                setCreatures(newCreatures);
              }}
              onBlur={handleCreatureUpdate}
              ariaLabel="Hit Points"
              placeholder="HP"
            />
            <NumericInput
              value={creature.maximumHitPoints}
              onChange={(newMaxHP) => {
                const newCreatures = [...creatures];
                newCreatures[index] = { 
                  ...creature, 
                  maximumHitPoints: newMaxHP
                };
                setCreatures(newCreatures);
              }}
              onBlur={handleCreatureUpdate}
              ariaLabel="Maximum Hit Points"
              placeholder="Max HP"
            />
            <NumericInput
              value={creature.armorClass}
              min={0}
              onChange={(newAC) => {
                const newCreatures = [...creatures];
                newCreatures[index] = { ...creature, armorClass: newAC };
                setCreatures(newCreatures);
              }}
              onBlur={handleCreatureUpdate}
              ariaLabel="Armor Class"
              placeholder="AC"
            />
            <NumericInput
              value={creature.initiative}
              onChange={(newInitiative) => {
                const newCreatures = [...creatures];
                newCreatures[index] = { ...creature, initiative: newInitiative };
                setCreatures(newCreatures);
              }}
              onBlur={handleCreatureUpdate}
              ariaLabel="Initiative"
              placeholder="Init"
            />
            <NumericInput
              value={creature.initiativeModifier}
              onChange={(newMod) => {
                const newCreatures = [...creatures];
                newCreatures[index] = { ...creature, initiativeModifier: newMod };
                setCreatures(newCreatures);
              }}
              onBlur={handleCreatureUpdate}
              ariaLabel="Initiative Modifier"
              placeholder="Mod"
            />
            <div className="creature-controls">
              <button
                className="control-button danger"
                onClick={() => removeCreature(index)}
              >
                Remove
              </button>
            </div>
          </div>
        ))}
      </div>

      <button className="add-creature-button" onClick={addCreature}>
        Add Creature
      </button>

      {isRunning && (
        <div className="encounter-status">
          <span>Turn {turnNumber}</span>
          <span>Active: {creatures[currentTurn]?.name || 'None'}</span>
          <button 
            className="control-button primary"
            onClick={nextTurn}
          >
            Next Turn
          </button>
        </div>
      )}
    </div>
  );
};

export default EditEncounter;