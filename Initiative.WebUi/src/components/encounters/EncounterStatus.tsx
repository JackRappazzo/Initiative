import React from 'react';
import { EncounterState, EditableCreature } from '../../types';

interface EncounterStatusProps {
  encounterState: EncounterState;
  creatures: EditableCreature[];
  onToggleEncounter: () => void;
  onNextTurn: () => void;
}

export const EncounterStatus: React.FC<EncounterStatusProps> = ({
  encounterState,
  creatures,
  onToggleEncounter,
  onNextTurn
}) => {
  const { isRunning, currentTurn, turnNumber } = encounterState;

  return (
    <>
      <button 
        className={`control-button ${isRunning ? 'danger' : 'primary'}`}
        onClick={onToggleEncounter}
      >
        {isRunning ? 'End Encounter' : 'Start Encounter'}
      </button>

      {isRunning && (
        <div className="encounter-status">
          <span>Turn {turnNumber}</span>
          <span>Active: {creatures[currentTurn]?.name || 'None'}</span>
          <button 
            className="control-button primary"
            onClick={onNextTurn}
          >
            Next Turn
          </button>
        </div>
      )}
    </>
  );
};
