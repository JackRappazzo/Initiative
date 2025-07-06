import React from 'react';
import { NumericInput } from '../ui';
import { EditableCreature } from '../../types';

interface CreatureRowProps {
  creature: EditableCreature;
  index: number;
  isCurrentTurn: boolean;
  isDragging: boolean;
  isDragOver: boolean;
  dragPosition: 'top' | 'bottom' | null;
  onCreatureChange: (index: number, creature: EditableCreature) => void;
  onCreatureRemove: (index: number) => void;
  onDragStart: (index: number) => void;
  onDragEnd: () => void;
  onDragOver: (e: React.DragEvent, index: number) => void;
  onDrop: () => void;
}

export const CreatureRow: React.FC<CreatureRowProps> = ({
  creature,
  index,
  isCurrentTurn,
  isDragging,
  isDragOver,
  dragPosition,
  onCreatureChange,
  onCreatureRemove,
  onDragStart,
  onDragEnd,
  onDragOver,
  onDrop
}) => {
  const handleFieldChange = (field: keyof EditableCreature, value: any) => {
    onCreatureChange(index, { ...creature, [field]: value });
  };

  return (
    <div 
      className={`creature-item ${
        isDragging ? 'dragging' : ''
      } ${
        isDragOver && dragPosition === 'top' ? 'drag-over-top' : ''
      } ${
        isDragOver && dragPosition === 'bottom' ? 'drag-over-bottom' : ''
      }`}
      style={isCurrentTurn ? { background: '#e9ecef' } : undefined}
      onDragOver={(e) => onDragOver(e, index)}
      onDrop={onDrop}
    >
      <div
        className="drag-handle"
        draggable
        onDragStart={() => onDragStart(index)}
        onDragEnd={onDragEnd}
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
