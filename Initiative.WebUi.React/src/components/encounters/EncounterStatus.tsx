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

  const meterColorClass = encounterDifficulty
    ? `encounter-difficulty-meter-${encounterDifficulty.difficulty.toLowerCase()}`
    : 'encounter-difficulty-meter-none';

  const progressPercent = encounterDifficulty && encounterDifficulty.thresholds.extreme > 0
    ? Math.min(100, Math.round((encounterDifficulty.totalMonsterXp / encounterDifficulty.thresholds.extreme) * 100))
    : 0;

  const overExtremeBy = encounterDifficulty
    ? Math.max(0, encounterDifficulty.totalMonsterXp - encounterDifficulty.thresholds.extreme)
    : 0;

  return (
    <>
      <button 
        className={`control-button ${viewersAllowed ? 'danger' : 'primary'}`}
        onClick={onToggleViewersAllowed}
      >
        {viewersAllowed ? 'Disallow Viewers' : 'Allow Viewers'}
      </button>

      <div className="encounter-status">
        <div className="encounter-difficulty">
          <div className="encounter-difficulty-header">
            <strong>Encounter Difficulty</strong>
            <button
              className="control-button secondary encounter-difficulty-toggle"
              onClick={onToggleShowDifficulty}
              title={showDifficulty ? 'Hide encounter difficulty meter' : 'Show encounter difficulty meter'}
            >
              {showDifficulty ? 'Hide' : 'Show'}
            </button>
          </div>

          {showDifficulty && (
            !encounterDifficulty ? (
              <span>Difficulty unavailable: choose a party to evaluate.</span>
            ) : (
              <>
                <div className="encounter-difficulty-meter">
                  <div
                    className={`encounter-difficulty-meter-fill ${meterColorClass}`}
                    style={{ width: `${progressPercent}%` }}
                  />
                </div>
                <div className="encounter-difficulty-meta">
                  <strong>{encounterDifficulty.difficulty}</strong>
                  {' · '}
                  {formatNumber(encounterDifficulty.totalMonsterXp)} / {formatNumber(encounterDifficulty.thresholds.extreme)} XP
                  {' · '}
                  {partyMemberCount} member{partyMemberCount === 1 ? '' : 's'}
                </div>
                {overExtremeBy > 0 && (
                  <div className="encounter-difficulty-note encounter-difficulty-note-danger">
                    Over Extreme by {formatNumber(overExtremeBy)} XP.
                  </div>
                )}
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
