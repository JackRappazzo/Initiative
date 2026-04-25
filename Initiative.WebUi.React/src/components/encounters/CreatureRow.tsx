import React, { useState, useRef, useCallback } from 'react';
import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { NumericInput } from '../ui';
import { EditableCreature } from '../../types';
import { BestiaryClient, FiveEToolsRawData } from '../../api/bestiaryClient';
import CreatureStatBlock from '../bestiaries/CreatureStatBlock';
import { StatusTypeahead } from './StatusTypeahead';
import { creatureToFantasyStatblockYaml } from '../../utils/fantasyStatblockYaml';

type EditingField = 'initiative' | 'currentHP' | 'maxHP' | 'displayName' | 'ac' | null;

const bestiaryClient = new BestiaryClient();
const HEALTH_STATUS_SET = new Set(['healthy', 'hurt', 'bloodied']);
const D20_ICON = '\u{1F3B2}';
const SHEET_ICON = '\u{1F4CB}';
const HIDE_ICON = '\u{1F441}\uFE0F';
const SHOW_ICON = '\u{1F513}';

interface CreatureRowProps {
  creature: EditableCreature;
  index: number;
  isCurrentTurn: boolean;
  onCreatureChange: (index: number, creature: EditableCreature) => void;
  onCreatureRemove: (index: number) => void;
}

export const CreatureRow: React.FC<CreatureRowProps> = ({
  creature,
  index,
  isCurrentTurn,
  onCreatureChange,
  onCreatureRemove,
}) => {
  const [editingField, setEditingField] = useState<EditingField>(null);
  const nameInputRef = useRef<HTMLInputElement>(null);
  const [statBlockData, setStatBlockData] = useState<FiveEToolsRawData | null>(null);
  const [statBlockLoading, setStatBlockLoading] = useState(false);
  const [copyState, setCopyState] = useState<'copied' | 'error' | null>(null);
  const [showStatusTypeahead, setShowStatusTypeahead] = useState(false);

  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: index.toString() });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  };

  const handleFieldChange = useCallback((field: keyof EditableCreature, value: any) => {
    onCreatureChange(index, { ...creature, [field]: value });
  }, [onCreatureChange, index, creature]);

  const startEditing = useCallback((field: EditingField) => {
    setEditingField(field);
    if (field === 'displayName') {
      setTimeout(() => nameInputRef.current?.select(), 0);
    }
  }, []);

  const stopEditing = useCallback(() => {
    setEditingField(null);
  }, []);

  const openStatBlock = useCallback(async () => {
    if (!creature.creatureId) return;

    setStatBlockLoading(true);
    try {
      const detail = await bestiaryClient.getCreatureById(creature.creatureId);
      setStatBlockData(detail.rawData);
    } finally {
      setStatBlockLoading(false);
    }
  }, [creature.creatureId]);

  const copyYamlToClipboard = useCallback(async () => {
    if (!statBlockData) return;

    try {
      const yamlText = creatureToFantasyStatblockYaml(statBlockData);
      await navigator.clipboard.writeText(yamlText);
      setCopyState('copied');
      window.setTimeout(() => setCopyState(null), 1800);
    } catch {
      setCopyState('error');
      window.setTimeout(() => setCopyState(null), 2200);
    }
  }, [statBlockData]);

  const rollInitiative = useCallback(() => {
    const roll = Math.floor(Math.random() * 20) + 1;
    const modifier = creature.initiativeModifier ?? 0;
    handleFieldChange('initiative', roll + modifier);
  }, [creature.initiativeModifier, handleFieldChange]);

  const displayValue = (val: number | undefined, placeholder: string) =>
    val !== undefined && val !== null ? val.toString() : placeholder;

  const addStatus = useCallback(() => {
    setShowStatusTypeahead(true);
  }, []);

  const handleStatusSelected = useCallback((status: string) => {
    const trimmed = status.trim();
    if (!trimmed || HEALTH_STATUS_SET.has(trimmed.toLowerCase())) {
      return;
    }

    const existingStatuses = creature.statuses ?? [];
    if (existingStatuses.some((s) => s.toLowerCase() === trimmed.toLowerCase())) {
      return;
    }

    handleFieldChange('statuses', [...existingStatuses, trimmed]);
    setShowStatusTypeahead(false);
  }, [creature.statuses, handleFieldChange]);

  const removeStatus = useCallback((statusIndex: number) => {
    const existingStatuses = creature.statuses ?? [];
    handleFieldChange('statuses', existingStatuses.filter((_, idx) => idx !== statusIndex));
  }, [creature.statuses, handleFieldChange]);

  const visibleStatuses = (creature.statuses ?? [])
    .map((status, statusIndex) => ({ status, statusIndex }))
    .filter(({ status }) => !HEALTH_STATUS_SET.has(status.trim().toLowerCase()));

  return (
    <>
      <div
        ref={setNodeRef}
        style={style}
        className={`creature-row-wrap ${isDragging ? 'dragging' : ''}${creature.isHidden ? ' creature-row-wrap-hidden' : ''}`}
        {...attributes}
      >
        <div
          className={`creature-item${isCurrentTurn ? ' creature-item-current-turn' : ''}${creature.isHidden ? ' creature-item-hidden' : ''}`}
        >
          <div
            className="drag-handle"
            {...listeners}
          >
            ::
          </div>

          <div className="creature-init-cell">
            <button
              className="die-button"
              onClick={rollInitiative}
              title={`Roll 1d20${(creature.initiativeModifier ?? 0) >= 0 ? '+' : ''}${creature.initiativeModifier ?? 0}`}
              aria-label="Roll initiative"
            >
              {D20_ICON}
            </button>
            {editingField === 'initiative' ? (
              <NumericInput
                value={creature.initiative}
                onChange={(value) => handleFieldChange('initiative', value)}
                onBlur={stopEditing}
                ariaLabel="Initiative"
                placeholder="-"
                className="creature-field-input creature-init-input"
              />
            ) : (
              <span
                className="creature-field-display creature-init-value"
                onClick={() => startEditing('initiative')}
                title="Click to edit initiative"
              >
                {displayValue(creature.initiative, '-')}
              </span>
            )}
          </div>

          <div className="creature-hp-cell">
            {creature.isPlayer ? (
              <span className="creature-field-display creature-player-dash">--</span>
            ) : (
              <>
                {editingField === 'currentHP' ? (
                  <NumericInput
                    value={creature.currentHP}
                    onChange={(value) => handleFieldChange('currentHP', value)}
                    onBlur={stopEditing}
                    ariaLabel="Hit Points"
                    placeholder="-"
                    className="creature-field-input creature-hp-input"
                  />
                ) : (
                  <span
                    className="creature-field-display creature-hp-part"
                    onClick={() => startEditing('currentHP')}
                    title="Click to edit current HP"
                  >
                    {displayValue(creature.currentHP, '-')}
                  </span>
                )}
                <span className="creature-hp-sep">/</span>
                {editingField === 'maxHP' ? (
                  <NumericInput
                    value={creature.maxHP}
                    onChange={(value) => handleFieldChange('maxHP', value)}
                    onBlur={stopEditing}
                    ariaLabel="Maximum Hit Points"
                    placeholder="-"
                    className="creature-field-input creature-hp-input"
                  />
                ) : (
                  <span
                    className="creature-field-display creature-hp-part"
                    onClick={() => startEditing('maxHP')}
                    title="Click to edit max HP"
                  >
                    {displayValue(creature.maxHP, '-')}
                  </span>
                )}
              </>
            )}
          </div>

          <div className="creature-name-cell">
            {editingField === 'displayName' ? (
              <input
                ref={nameInputRef}
                type="text"
                value={creature.displayName}
                onChange={(e) => handleFieldChange('displayName', e.target.value)}
                onBlur={stopEditing}
                className="creature-field-input creature-name-input"
                autoFocus
              />
            ) : (
              <span
                className={`creature-field-display creature-name-display${creature.isPlayer ? ' creature-name-player' : ''}`}
                onClick={() => startEditing('displayName')}
                title="Click to edit name"
              >
                {creature.displayName || '-'}
              </span>
            )}
            <div className="creature-status-inline">
              {visibleStatuses.map(({ status, statusIndex }) => (
                <span key={`${status}-${statusIndex}`} className="creature-status-pill creature-status-pill-inline">
                  {status}
                  <button
                    type="button"
                    className="creature-status-remove"
                    onClick={() => removeStatus(statusIndex)}
                    title={`Remove ${status}`}
                  >
                    X
                  </button>
                </span>
              ))}
              <button
                type="button"
                className="creature-status-add-inline"
                onClick={addStatus}
                title="Add status"
              >
                +
              </button>
            </div>
          </div>

          {creature.isPlayer ? (
            <span className="creature-field-display creature-player-dash">--</span>
          ) : editingField === 'ac' ? (
            <NumericInput
              value={creature.ac}
              min={0}
              onChange={(value) => handleFieldChange('ac', value)}
              onBlur={stopEditing}
              ariaLabel="Armor Class"
              placeholder="-"
              className="creature-field-input"
            />
          ) : (
            <span
              className="creature-field-display"
              onClick={() => startEditing('ac')}
              title="Click to edit AC"
            >
              {displayValue(creature.ac, '-')}
            </span>
          )}

          <div className="creature-controls">
            {!creature.isPlayer && creature.creatureId && (
              <button
                className="control-button secondary"
                onClick={openStatBlock}
                disabled={statBlockLoading}
                title="View stat block"
                aria-label="View stat block"
              >
                {statBlockLoading ? '...' : SHEET_ICON}
              </button>
            )}
            {!creature.isPlayer && (
              <button
                className={`control-button secondary ${creature.isHidden ? 'hidden' : ''}`}
                onClick={() => handleFieldChange('isHidden', !creature.isHidden)}
                title={creature.isHidden ? 'Show in lobby' : 'Hide from lobby'}
                aria-label={creature.isHidden ? 'Show in lobby' : 'Hide from lobby'}
              >
                {creature.isHidden ? SHOW_ICON : HIDE_ICON}
              </button>
            )}
            <button
              className="control-button danger"
              onClick={() => onCreatureRemove(index)}
            >
              X
            </button>
          </div>
        </div>
      </div>

      {statBlockData && (
        <div className="stat-block-overlay" onClick={() => setStatBlockData(null)}>
          <div className="stat-block-modal" onClick={(e) => e.stopPropagation()}>
            <div className="stat-block-controls">
              <button className="stat-block-copy" onClick={copyYamlToClipboard} aria-label="Copy Fantasy YAML">YAML</button>
              <button className="stat-block-close" onClick={() => setStatBlockData(null)} aria-label="Close">X</button>
            </div>
            {copyState && (
              <div className={`stat-block-copy-status ${copyState === 'error' ? 'is-error' : 'is-success'}`} role="status">
                {copyState === 'copied' ? 'YAML copied' : 'Copy failed'}
              </div>
            )}
            <CreatureStatBlock data={statBlockData} />
          </div>
        </div>
      )}

      {showStatusTypeahead && (
        <StatusTypeahead
          existingStatuses={(creature.statuses ?? []).filter((status) => !HEALTH_STATUS_SET.has(status.trim().toLowerCase()))}
          onStatusSelected={handleStatusSelected}
          onDismiss={() => setShowStatusTypeahead(false)}
        />
      )}
    </>
  );
};
