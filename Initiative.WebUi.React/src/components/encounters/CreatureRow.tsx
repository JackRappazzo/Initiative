import React, { useState, useRef, useCallback } from 'react';
import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { NumericInput } from '../ui';
import { EditableCreature } from '../../types';

type EditingField = 'initiative' | 'currentHP' | 'maxHP' | 'displayName' | 'ac' | null;

interface CreatureRowProps {
  creature: EditableCreature;
  index: number;
  isCurrentTurn: boolean;
  onCreatureChange: (index: number, creature: EditableCreature) => void;
  onCreatureRemove: (index: number) => void;
}

export const CreatureRow: React.FC<CreatureRowProps> = ({
  creature,
  index,
  isCurrentTurn,
  onCreatureChange,
  onCreatureRemove,
}) => {
  const [editingField, setEditingField] = useState<EditingField>(null);
  const nameInputRef = useRef<HTMLInputElement>(null);

  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: index.toString() });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  };

  const handleFieldChange = (field: keyof EditableCreature, value: any) => {
    onCreatureChange(index, { ...creature, [field]: value });
  };

  const startEditing = useCallback((field: EditingField) => {
    setEditingField(field);
    if (field === 'displayName') {
      // Focus the name input after render
      setTimeout(() => nameInputRef.current?.select(), 0);
    }
  }, []);

  const stopEditing = useCallback(() => {
    setEditingField(null);
  }, []);

  const displayValue = (val: number | undefined, placeholder: string) =>
    val !== undefined && val !== null ? val.toString() : placeholder;

  return (
    <div 
      ref={setNodeRef}
      style={{
        ...style,
        ...(isCurrentTurn ? { background: '#e9ecef' } : {})
      }}
      className={`creature-item ${isDragging ? 'dragging' : ''}`}
      {...attributes}
    >
      <div
        className="drag-handle"
        {...listeners}
      >
        ⋮⋮
      </div>

      {/* Initiative */}
      {editingField === 'initiative' ? (
        <NumericInput
          value={creature.initiative}
          onChange={(value) => handleFieldChange('initiative', value)}
          onBlur={stopEditing}
          ariaLabel="Initiative"
          placeholder="–"
          className="creature-field-input"
        />
      ) : (
        <span
          className="creature-field-display"
          onClick={() => startEditing('initiative')}
          title="Click to edit initiative"
        >
          {displayValue(creature.initiative, '–')}
        </span>
      )}

      {/* HP: current / max */}
      <div className="creature-hp-cell">
        {editingField === 'currentHP' ? (
          <NumericInput
            value={creature.currentHP}
            onChange={(value) => handleFieldChange('currentHP', value)}
            onBlur={stopEditing}
            ariaLabel="Hit Points"
            placeholder="–"
            className="creature-field-input creature-hp-input"
          />
        ) : (
          <span
            className="creature-field-display creature-hp-part"
            onClick={() => startEditing('currentHP')}
            title="Click to edit current HP"
          >
            {displayValue(creature.currentHP, '–')}
          </span>
        )}
        <span className="creature-hp-sep">/</span>
        {editingField === 'maxHP' ? (
          <NumericInput
            value={creature.maxHP}
            onChange={(value) => handleFieldChange('maxHP', value)}
            onBlur={stopEditing}
            ariaLabel="Maximum Hit Points"
            placeholder="–"
            className="creature-field-input creature-hp-input"
          />
        ) : (
          <span
            className="creature-field-display creature-hp-part"
            onClick={() => startEditing('maxHP')}
            title="Click to edit max HP"
          >
            {displayValue(creature.maxHP, '–')}
          </span>
        )}
      </div>

      {/* Display Name */}
      {editingField === 'displayName' ? (
        <input
          ref={nameInputRef}
          type="text"
          value={creature.displayName}
          onChange={(e) => handleFieldChange('displayName', e.target.value)}
          onBlur={stopEditing}
          className="creature-field-input creature-name-input"
          autoFocus
        />
      ) : (
        <span
          className={`creature-field-display creature-name-display${creature.isPlayer ? ' creature-name-player' : ''}`}
          onClick={() => startEditing('displayName')}
          title="Click to edit name"
        >
          {creature.displayName || '–'}
        </span>
      )}

      {/* AC */}
      {editingField === 'ac' ? (
        <NumericInput
          value={creature.ac}
          min={0}
          onChange={(value) => handleFieldChange('ac', value)}
          onBlur={stopEditing}
          ariaLabel="Armor Class"
          placeholder="–"
          className="creature-field-input"
        />
      ) : (
        <span
          className="creature-field-display"
          onClick={() => startEditing('ac')}
          title="Click to edit AC"
        >
          {displayValue(creature.ac, '–')}
        </span>
      )}

      <div className="creature-controls">
        <button
          className="control-button danger"
          onClick={() => onCreatureRemove(index)}
        >
          ✕
        </button>
      </div>
    </div>
  );
};
