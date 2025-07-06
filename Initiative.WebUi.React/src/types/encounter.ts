import { CreatureJsonModel } from '../api/encounterClient';

export interface EditableCreature extends CreatureJsonModel {
  isEditing?: boolean;
}

export interface DragState {
  draggedCreature: number | null;
  dragOverIndex: number | null;
  dragPosition: 'top' | 'bottom' | null;
}

export interface EncounterState {
  isRunning: boolean;
  currentTurn: number;
  turnNumber: number;
}
