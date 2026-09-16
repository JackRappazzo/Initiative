import React, { useCallback, useMemo, useState } from 'react';
import { DndBeyondCharacterDetail, DndBeyondSpellSlot } from '../../api/dndBeyondClient';
import { isTaleSpire, rollInTray } from '../../utils/talespire';
import {
  RollResult,
  rollExpression,
  AbilityScores,
  PropertyLine,
  SpellLink,
  capitalize,
} from '../bestiaries/statBlockShared';
import '../bestiaries/CreatureStatBlock.css';

interface Props {
  detail: DndBeyondCharacterDetail;
}

const LEVEL_SUFFIX: Record<number, string> = { 1: 'st', 2: 'nd', 3: 'rd' };

function formatLevelLabel(level: number): string {
  if (level === 0) return 'Cantrips';
  return `${level}${LEVEL_SUFFIX[level] ?? 'th'} level`;
}

function formatSlotTotal(slot: DndBeyondSpellSlot): number {
  return slot.used + slot.available;
}

function formatSigned(value: number): string {
  return value >= 0 ? `+${value}` : `${value}`;
}

const PlayerStatBlock: React.FC<Props> = ({ detail }) => {
  const [lastRoll, setLastRoll] = useState<RollResult | null>(null);

  const onRoll = useCallback((result: RollResult) => {
    if (isTaleSpire()) {
      rollInTray(result.expression, result.expression);
    } else {
      setLastRoll(result);
    }
  }, []);

  const rollDetail = lastRoll && lastRoll.rolls.length > 1
    ? ` [${lastRoll.rolls.join(', ')}]`
    : '';

  const spellsByLevel = useMemo(() => {
    const groups = new Map<number, string[]>();
    for (const spell of detail.preparedSpells ?? []) {
      const list = groups.get(spell.level) ?? [];
      list.push(spell.name);
      groups.set(spell.level, list);
    }
    return Array.from(groups.entries()).sort(([a], [b]) => a - b);
  }, [detail.preparedSpells]);

  const spellSlots = useMemo(
    () => (detail.spellSlots ?? []).filter((slot) => formatSlotTotal(slot) > 0),
    [detail.spellSlots]
  );

  const pactSlots = useMemo(
    () => (detail.pactSlots ?? []).filter((slot) => formatSlotTotal(slot) > 0),
    [detail.pactSlots]
  );

  const className = detail.className ?? '';
  const meta = [detail.race, className && detail.level > 0 ? `${className} ${detail.level}` : className]
    .filter(Boolean)
    .join(', ');

  const hpText = detail.temporaryHP > 0
    ? `(${detail.temporaryHP}) ${detail.currentHP} / ${detail.maxHP}`
    : `${detail.currentHP} / ${detail.maxHP}`;

  return (
    <div className="stat-block">
      {lastRoll && (
        <div className="stat-block__roll-banner">
          <span className="stat-block__roll-expression">{lastRoll.expression}</span>
          <span className="stat-block__roll-total">{lastRoll.total}</span>
          {rollDetail && <span className="stat-block__roll-detail">{rollDetail}</span>}
          <button
            className="stat-block__roll-dismiss"
            aria-label="Dismiss"
            onClick={() => setLastRoll(null)}
          >×</button>
        </div>
      )}

      <div className="stat-block__header">
        <h2 className="stat-block__name">{detail.name}</h2>
        {meta && <p className="stat-block__meta">{meta}</p>}
      </div>

      <div className="stat-block__divider stat-block__divider--thick" />

      <PropertyLine label="Armor Class" value={detail.armorClass} />
      <PropertyLine label="Hit Points" value={hpText} />
      <PropertyLine label="Speed" value={`${detail.speed} ft.`} />

      <div className="stat-block__divider stat-block__divider--thick" />

      <AbilityScores
        data={{
          str: detail.strength,
          dex: detail.dexterity,
          con: detail.constitution,
          int: detail.intelligence,
          wis: detail.wisdom,
          cha: detail.charisma,
        }}
        onRoll={onRoll}
      />

      {(detail.focusPoints || pactSlots.length > 0 || spellSlots.length > 0 || spellsByLevel.length > 0) && (
        <div className="stat-block__divider stat-block__divider--thick" />
      )}

      {detail.focusPoints && (
        <PropertyLine
          label="Focus Points"
          value={`${detail.focusPoints.current} / ${detail.focusPoints.max}`}
        />
      )}

      {pactSlots.length > 0 && (
        <PropertyLine
          label="Pact Magic"
          value={pactSlots
            .map((slot) => `${formatLevelLabel(slot.level)} (${formatSlotTotal(slot) - slot.available}/${formatSlotTotal(slot)})`)
            .join(', ')}
        />
      )}

      {spellSlots.length > 0 && (
        <PropertyLine
          label="Spell Slots"
          value={spellSlots
            .map((slot) => `${formatLevelLabel(slot.level)} (${formatSlotTotal(slot) - slot.available}/${formatSlotTotal(slot)})`)
            .join(', ')}
        />
      )}

      {spellsByLevel.map(([level, spells]) => (
        <div key={level} className="stat-block__feature">
          <em>{formatLevelLabel(level)}: </em>
          {spells.map((name, i) => (
            <React.Fragment key={name}>
              {i > 0 && ', '}
              <SpellLink raw={capitalize(name)} />
            </React.Fragment>
          ))}.
        </div>
      ))}

      {(detail.attacks ?? []).length > 0 && (
        <>
          <div className="stat-block__divider" />
          <h4 className="stat-block__section-title">Actions</h4>
          {detail.attacks.map((attack, i) => {
            const toHitExpr = `1d20${formatSigned(attack.toHitBonus)}`;
            const damageExpr = attack.damageBonus !== 0
              ? `${attack.damageDice}${formatSigned(attack.damageBonus)}`
              : attack.damageDice;
            const rangeText = attack.isRanged
              ? (attack.longRange > 0 ? `range ${attack.range}/${attack.longRange} ft.` : `range ${attack.range} ft.`)
              : `reach ${attack.range} ft.`;
            const damageType = attack.damageType ? `${attack.damageType.toLowerCase()} ` : '';
            return (
              <div key={`${attack.name}-${i}`} className="stat-block__feature">
                <strong>{attack.name}. </strong>
                <em>{attack.isRanged ? 'Ranged' : 'Melee'} Weapon Attack: </em>
                <button
                  className="stat-block__dice-chip"
                  title={`Roll ${toHitExpr}`}
                  onClick={() => onRoll(rollExpression(toHitExpr))}
                >
                  {formatSigned(attack.toHitBonus)}
                </button>{' '}
                to hit, {rangeText}. <em>Hit: </em>
                <button
                  className="stat-block__dice-chip"
                  title={`Roll ${damageExpr}`}
                  onClick={() => onRoll(rollExpression(damageExpr))}
                >
                  {damageExpr}
                </button>{' '}
                {damageType}damage.
              </div>
            );
          })}
        </>
      )}
    </div>
  );
};

export default PlayerStatBlock;
