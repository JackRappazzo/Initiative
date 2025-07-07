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
}

export const EditableCreatureList: React.FC<EditableCreatureListProps> = ({
  creatures,
  highlightedCreatureIndex,
  onCreaturesChange,
  onCreatureUpdate,
  onCreatureRemove,
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
        onCreaturesChange(newCreatures);
      }
    }
  }, [creatures, onCreaturesChange]);

  return (
    <>
      {/* Header Row */}
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
