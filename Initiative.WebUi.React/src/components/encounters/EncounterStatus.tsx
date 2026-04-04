import React from 'react';
import { EncounterState, EditableCreature } from '../../types';
import { EncounterDifficultySummary } from '../../utils/encounterDifficulty';

interface EncounterStatusProps {
  encounterState: EncounterState;
  creatures: EditableCreature[];
  onToggleViewersAllowed: () => void;
  onNextTurn: () => void;
  onPrevTurn: () => void;
  showDifficulty: boolean;
  onToggleShowDifficulty: () => void;
  encounterDifficulty: EncounterDifficultySummary | null;
  partyMemberCount: number;
  unknownMonsterCount: number;
}

export const EncounterStatus: React.FC<EncounterStatusProps> = ({
  encounterState,
  creatures,
  onToggleViewersAllowed,
  onNextTurn,
  onPrevTurn,
  showDifficulty,
  onToggleShowDifficulty,
  encounterDifficulty,
  partyMemberCount,
  unknownMonsterCount,
}) => {
  const { viewersAllowed, currentTurn, turnNumber } = encounterState;

  const formatNumber = (value: number): string => value.toLocaleString();

  const difficultyClass = encounterDifficulty
    ? `encounter-difficulty-${encounterDifficulty.difficulty.toLowerCase()}`
    : 'encounter-difficulty-none';

  return (
    <>
      <button 
        className={`control-button ${viewersAllowed ? 'danger' : 'primary'}`}
        onClick={onToggleViewersAllowed}
      >
        {viewersAllowed ? 'Disallow Viewers' : 'Allow Viewers'}
      </button>

      <div className="encounter-status">
        <div className={`encounter-difficulty ${difficultyClass}`}>
          <div className="encounter-difficulty-header">
            <strong>Encounter Difficulty</strong>
            <button
              className="control-button secondary encounter-difficulty-toggle"
              onClick={onToggleShowDifficulty}
              title={showDifficulty ? 'Hide encounter difficulty details' : 'Show encounter difficulty details'}
            >
              {showDifficulty ? 'Hide' : 'Show'}
            </button>
          </div>

          {showDifficulty && (
            !encounterDifficulty ? (
              <span>Difficulty unavailable: choose a party to evaluate.</span>
            ) : (
              <>
                <div className="encounter-difficulty-row">
                  <strong>Difficulty:</strong> {encounterDifficulty.difficulty}
                </div>
                <div className="encounter-difficulty-row">
                  <strong>Monster XP:</strong> {formatNumber(encounterDifficulty.totalMonsterXp)}
                </div>
                <div className="encounter-difficulty-row">
                  <strong>Party:</strong> {partyMemberCount} member{partyMemberCount === 1 ? '' : 's'}
                </div>
                <div className="encounter-difficulty-thresholds">
                  L {formatNumber(encounterDifficulty.thresholds.low)}
                  {' | '}M {formatNumber(encounterDifficulty.thresholds.moderate)}
                  {' | '}H {formatNumber(encounterDifficulty.thresholds.high)}
                  {' | '}X {formatNumber(encounterDifficulty.thresholds.extreme)}
                </div>
                {unknownMonsterCount > 0 && (
                  <div className="encounter-difficulty-note">
                    {unknownMonsterCount} monster{unknownMonsterCount === 1 ? '' : 's'} with unknown CR were treated as 0 XP.
                  </div>
                )}
              </>
            )
          )}
        </div>

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
