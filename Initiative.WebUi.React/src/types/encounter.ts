import { EncounterCreatureJsonModel } from '../api/encounterClient';

export interface EditableCreature extends EncounterCreatureJsonModel {
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
