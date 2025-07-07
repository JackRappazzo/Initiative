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
      <input
        type="text"
        value={creature.name}
        onChange={(e) => handleFieldChange('name', e.target.value)}
      />
      <NumericInput
        value={creature.hitPoints}
        onChange={(value) => handleFieldChange('hitPoints', value)}
        ariaLabel="Hit Points"
        placeholder="HP"
      />
      <NumericInput
        value={creature.maximumHitPoints}
        onChange={(value) => handleFieldChange('maximumHitPoints', value)}
        ariaLabel="Maximum Hit Points"
        placeholder="Max HP"
      />
      <NumericInput
        value={creature.armorClass}
        min={0}
        onChange={(value) => handleFieldChange('armorClass', value)}
        ariaLabel="Armor Class"
        placeholder="AC"
      />
      <NumericInput
        value={creature.initiative}
        onChange={(value) => handleFieldChange('initiative', value)}
        ariaLabel="Initiative"
        placeholder="Init"
      />
      <NumericInput
        value={creature.initiativeModifier}
        onChange={(value) => handleFieldChange('initiativeModifier', value)}
        ariaLabel="Initiative Modifier"
        placeholder="Mod"
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
