import React from 'react';
import { EncounterState, EditableCreature } from '../../types';

interface EncounterStatusProps {
  encounterState: EncounterState;
  creatures: EditableCreature[];
  onToggleEncounter: () => void;
  onNextTurn: () => void;
  onPrevTurn: () => void;
}

export const EncounterStatus: React.FC<EncounterStatusProps> = ({
  encounterState,
  creatures,
  onToggleEncounter,
  onNextTurn,
  onPrevTurn
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
          <span className="encounter-turn-number">Turn {turnNumber}</span>
          <div className="encounter-turn-nav">
            <button className="control-button secondary" onClick={onPrevTurn} title="Previous turn">◀</button>
            <span className="encounter-active-creature">{creatures[currentTurn]?.displayName || 'None'}</span>
            <button className="control-button primary" onClick={onNextTurn} title="Next turn">▶</button>
          </div>
        </div>
      )}
    </>
  );
};
