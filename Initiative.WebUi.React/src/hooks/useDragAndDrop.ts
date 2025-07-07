import { useCallback } from 'react';
import { arrayMove } from '@dnd-kit/sortable';
import { EditableCreature } from '../types';

export const useDragAndDrop = (
  creatures: EditableCreature[],
  onCreaturesReorder: (newCreatures: EditableCreature[]) => void,
  onSave: () => Promise<void>
) => {
  const handleDragEnd = useCallback(async (event: any) => {
    const { active, over } = event;

    if (active.id !== over?.id) {
      const oldIndex = parseInt(active.id.toString());
      const newIndex = parseInt(over.id.toString());

      if (oldIndex !== newIndex) {
        const newCreatures = arrayMove(creatures, oldIndex, newIndex);
        onCreaturesReorder(newCreatures);
        await onSave();
        
        // Return the new creatures array so the caller can use it for lobby updates
        return newCreatures;
      }
    }
    
    return creatures; // Return unchanged creatures if no reorder happened
  }, [creatures, onCreaturesReorder, onSave]);

  return {
    handleDragEnd
  };
};
