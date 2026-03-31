import React from 'react';
import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { NumericInput } from '../ui';
import { EditableCreature } from '../../types';

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
      <NumericInput
        value={creature.initiative}
        onChange={(value) => handleFieldChange('initiative', value)}
        ariaLabel="Initiative"
        placeholder="Init"
      />
      <NumericInput
        value={creature.currentHP}
        onChange={(value) => handleFieldChange('currentHP', value)}
        ariaLabel="Hit Points"
        placeholder="HP"
      />
      <NumericInput
        value={creature.maxHP}
        onChange={(value) => handleFieldChange('maxHP', value)}
        ariaLabel="Maximum Hit Points"
        placeholder="Max HP"
      />
      <input
        type="text"
        value={creature.displayName}
        onChange={(e) => handleFieldChange('displayName', e.target.value)}
      />
      <NumericInput
        value={creature.ac}
        min={0}
        onChange={(value) => handleFieldChange('ac', value)}
        ariaLabel="Armor Class"
        placeholder="AC"
      />
      <input
        type="checkbox"
        checked={creature.isPlayer}
        onChange={(e) => handleFieldChange('isPlayer', e.target.checked)}
        aria-label="Is Player"
        title="Player Character"
      />
      <div className="creature-controls">
        <button
          className="control-button danger"
          onClick={() => onCreatureRemove(index)}
        >
          Remove
        </button>
      </div>
    </div>
  );
};
