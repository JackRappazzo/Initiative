import React, { useCallback, useMemo } from 'react';
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
} from '@dnd-kit/core';
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
} from '@dnd-kit/sortable';
import { CreatureRow } from './CreatureRow';
import { EditableCreature } from '../../types';

interface EditableCreatureListProps {
  creatures: EditableCreature[];
  highlightedCreatureIndex?: number;
  onCreaturesChange: (creatures: EditableCreature[]) => void;
  onCreatureUpdate: (index: number, creature: EditableCreature) => void;
  onCreatureRemove: (index: number) => void;
  onRollAllInitiative: () => void;
}

export const EditableCreatureList: React.FC<EditableCreatureListProps> = ({
  creatures,
  highlightedCreatureIndex,
  onCreaturesChange,
  onCreatureUpdate,
  onCreatureRemove,
  onRollAllInitiative,
}) => {
  // Set up sensors for @dnd-kit
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  // Create stable IDs for the sortable items
  const itemIds = useMemo(() => 
    creatures.map((_, index) => index.toString()), 
    [creatures]
  );

  const handleDragEnd = useCallback((event: any) => {
    const { active, over } = event;

    if (active.id !== over?.id) {
      const oldIndex = parseInt(active.id.toString());
      const newIndex = parseInt(over.id.toString());

      if (oldIndex !== newIndex) {
        const newCreatures = arrayMove(creatures, oldIndex, newIndex);

        // Adjust the moved creature's initiative to fit between its new neighbours
        const above = newCreatures[newIndex - 1];
        const below = newCreatures[newIndex + 1];

        let newInitiative: number | undefined;
        if (above !== undefined && below !== undefined) {
          newInitiative = Math.floor((above.initiative + below.initiative) / 2);
        } else if (above !== undefined) {
          newInitiative = above.initiative;
        } else if (below !== undefined) {
          newInitiative = below.initiative;
        }

        if (newInitiative !== undefined) {
          newCreatures[newIndex] = { ...newCreatures[newIndex], initiative: newInitiative };
        }

        onCreaturesChange(newCreatures);
      }
    }
  }, [creatures, onCreaturesChange]);

  return (
    <>
      {/* Header Row */}
      <div className="creature-item creature-header">
        <div></div>
        <div className="creature-header-init">
          Init
          <button
            className="die-button die-button-header"
            onClick={onRollAllInitiative}
            title="Roll initiative for all non-player creatures"
          >
            🎲
          </button>
        </div>
        <div>HP</div>
        <div>Name</div>
        <div>AC</div>
        <div>Actions</div>
      </div>

      {/* Draggable Creature List */}
      <DndContext 
        sensors={sensors}
        collisionDetection={closestCenter}
        onDragEnd={handleDragEnd}
      >
        <SortableContext 
          items={itemIds}
          strategy={verticalListSortingStrategy}
        >
          {creatures.map((creature, index) => (
            <CreatureRow
              key={index}
              creature={creature}
              index={index}
              isCurrentTurn={index === highlightedCreatureIndex}
              onCreatureChange={onCreatureUpdate}
              onCreatureRemove={onCreatureRemove}
            />
          ))}
        </SortableContext>
      </DndContext>
    </>
  );
};
