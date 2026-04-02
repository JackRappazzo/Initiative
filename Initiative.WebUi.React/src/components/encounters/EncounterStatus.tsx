import React from 'react';
import { EncounterState, EditableCreature } from '../../types';

interface EncounterStatusProps {
  encounterState: EncounterState;
  creatures: EditableCreature[];
  onToggleViewersAllowed: () => void;
  onNextTurn: () => void;
  onPrevTurn: () => void;
}

export const EncounterStatus: React.FC<EncounterStatusProps> = ({
  encounterState,
  creatures,
  onToggleViewersAllowed,
  onNextTurn,
  onPrevTurn
}) => {
  const { viewersAllowed, currentTurn, turnNumber } = encounterState;

  return (
    <>
      <button 
        className={`control-button ${viewersAllowed ? 'danger' : 'primary'}`}
        onClick={onToggleViewersAllowed}
      >
        {viewersAllowed ? 'Disallow Viewers' : 'Allow Viewers'}
      </button>

      <div className="encounter-status">
        <div className="encounter-active-creature">
          {creatures[currentTurn]?.displayName || '\u00A0'}
        </div>
        <div className="encounter-turn-nav">
          <button className="control-button secondary" onClick={onPrevTurn} title="Previous turn">◀</button>
          <span className="encounter-turn-number">Turn {turnNumber}</span>
          <button className="control-button primary" onClick={onNextTurn} title="Next turn">▶</button>
        </div>
      </div>
    </>
  );
};
