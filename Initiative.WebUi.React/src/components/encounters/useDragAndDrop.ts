import { useState, useCallback } from 'react';
import { DragState, EditableCreature } from '../../types';

export const useDragAndDrop = (
  creatures: EditableCreature[],
  onCreaturesReorder: (newCreatures: EditableCreature[]) => void,
  onSave: () => Promise<void>
) => {
  const [dragState, setDragState] = useState<DragState>({
    draggedCreature: null,
    dragOverIndex: null,
    dragPosition: null
  });

  const handleDragStart = useCallback((index: number) => {
    setDragState(prev => ({ ...prev, draggedCreature: index }));
  }, []);

  const handleDragEnd = useCallback(() => {
    setDragState({
      draggedCreature: null,
      dragOverIndex: null,
      dragPosition: null
    });
  }, []);

  const handleDragOver = useCallback((e: React.DragEvent, index: number) => {
    e.preventDefault();
    if (dragState.draggedCreature === null || dragState.draggedCreature === index) return;

    const rect = (e.currentTarget as HTMLElement).getBoundingClientRect();
    const midpoint = rect.top + rect.height / 2;
    const position = e.clientY < midpoint ? 'top' : 'bottom';
    
    setDragState(prev => ({
      ...prev,
      dragOverIndex: index,
      dragPosition: position
    }));

    // Update the order in real-time
    const dropIndex = position === 'bottom' ? index + 1 : index;
    if (dropIndex !== dragState.draggedCreature && dropIndex !== dragState.draggedCreature + 1) {
      const newCreatures = [...creatures];
      const [draggedItem] = newCreatures.splice(dragState.draggedCreature, 1);
      const adjustedDropIndex = dropIndex > dragState.draggedCreature ? dropIndex - 1 : dropIndex;
      newCreatures.splice(adjustedDropIndex, 0, draggedItem);
      onCreaturesReorder(newCreatures);
      setDragState(prev => ({ ...prev, draggedCreature: adjustedDropIndex }));
    }
  }, [dragState.draggedCreature, creatures, onCreaturesReorder]);

  const handleDrop = useCallback(async () => {
    setDragState({
      draggedCreature: null,
      dragOverIndex: null,
      dragPosition: null
    });
    await onSave();
  }, [onSave]);

  return {
    dragState,
    handleDragStart,
    handleDragEnd,
    handleDragOver,
    handleDrop
  };
};
