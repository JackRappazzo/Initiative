import React, { useState, useCallback } from 'react';
import { FiveEToolsRawData, FiveEToolsEntry } from '../../api/bestiaryClient';
import { isTaleSpire, rollInTray } from '../../utils/talespire';
import {
  RollResult,
  OnRoll,
  cleanTags,
  capitalize,
  renderDiceNodes,
  renderEntriesAsNodes,
  AbilityScores,
  PropertyLine,
  SpellLink,
  ConditionLink,
} from './statBlockShared';
import './CreatureStatBlock.css';

interface Props {
  data: FiveEToolsRawData;
}

// ── Lookup tables ─────────────────────────────────────────────────────────────

const SIZE_MAP: Record<string, string> = {
  T: 'Tiny', S: 'Small', M: 'Medium', L: 'Large', H: 'Huge', G: 'Gargantuan',
};

const ALIGNMENT_MAP: Record<string, string> = {
  L: 'Lawful', N: 'Neutral', C: 'Chaotic',
  G: 'Good', E: 'Evil',
  U: 'Unaligned', A: 'Any alignment',
};

const CR_XP: Record<string, string> = {
  '0': '10', '1/8': '25', '1/4': '50', '1/2': '100',
  '1': '200', '2': '450', '3': '700', '4': '1,100', '5': '1,800',
  '6': '2,300', '7': '2,900', '8': '3,900', '9': '5,000', '10': '5,900',
  '11': '7,200', '12': '8,400', '13': '10,000', '14': '11,500', '15': '13,000',
  '16': '15,000', '17': '18,000', '18': '20,000', '19': '22,000', '20': '25,000',
  '21': '33,000', '22': '41,000', '23': '50,000', '24': '62,000', '25': '75,000',
  '26': '90,000', '27': '105,000', '28': '120,000', '29': '135,000', '30': '155,000',
};

type SpellcastingEntry = NonNullable<FiveEToolsRawData['spellcasting']>[number];

/**
 * Renders a header entry string, turning {@spell} tags into interactive SpellLink
 * buttons and the rest into standard dice/markdown nodes.
 */
function renderHeaderEntry(raw: string, onRoll: OnRoll, keyPrefix: string): React.ReactNode {
  // Split on {@spell ...} tags, keeping them as captured groups
  const parts = raw.split(/(\{@spell [^}]+\})/g);
  return parts.map((part, i) => {
    if (part.startsWith('{@spell ')) {
      return <SpellLink key={`${keyPrefix}-${i}`} raw={part} />;
    }
    return (
      <React.Fragment key={`${keyPrefix}-${i}`}>
        {renderDiceNodes(part, onRoll, `${keyPrefix}-${i}`)}
      </React.Fragment>
    );
  });
}

function SpellcastingBlock({ entry, onRoll }: { entry: SpellcastingEntry; onRoll: OnRoll }) {
  const hideDaily = entry.hidden?.includes('daily');
  const headerEntries = entry.headerEntries ?? [];
  const hasSpellList =
    (entry.will?.length ?? 0) > 0 ||
    (entry.spells && Object.keys(entry.spells).length > 0) ||
    (!hideDaily && entry.daily && Object.keys(entry.daily).length > 0);
  const firstSpellHeaderIndex = headerEntries.findIndex(h => h.includes('{@spell '));
  const hasInlineFreeformSpellList = !hasSpellList && firstSpellHeaderIndex > 0;

  return (
    <div className="stat-block__feature">
      <strong className="stat-block__feature-name">{cleanTags(entry.name)}. </strong>
      {hasInlineFreeformSpellList ? (
        <>
          {headerEntries.slice(0, firstSpellHeaderIndex).map((h, i) => (
            <span key={i}>{renderHeaderEntry(h, onRoll, `sc-header-${i}`)} </span>
          ))}
          <span className="stat-block__spell-list-break" aria-hidden="true" />
          {headerEntries.slice(firstSpellHeaderIndex).map((h, i) => (
            <span key={`${firstSpellHeaderIndex + i}`} className="stat-block__spell-freeform-line">
              {renderHeaderEntry(h, onRoll, `sc-header-${firstSpellHeaderIndex + i}`)}
            </span>
          ))}
        </>
      ) : (
        <>
          {headerEntries.map((h, i) => (
            <span key={i}>{renderHeaderEntry(h, onRoll, `sc-header-${i}`)} </span>
          ))}
          {headerEntries.length && hasSpellList ? <span className="stat-block__spell-list-break" aria-hidden="true" /> : null}
        </>
      )}
      {entry.will && entry.will.length > 0 && (
        <span>
          <em>At will: </em>
          {entry.will.map((raw, i) => (
            <React.Fragment key={i}>
              {i > 0 && ', '}
              <SpellLink raw={raw} />
            </React.Fragment>
          ))}.{' '}
        </span>
      )}
      {entry.spells && Object.entries(entry.spells)
        .sort(([a], [b]) => Number(a) - Number(b))
        .map(([level, { slots, spells }]) => {
          const lvl = Number(level);
          const suffixes: Record<number, string> = { 1: 'st', 2: 'nd', 3: 'rd' };
          const suffix = suffixes[lvl] ?? 'th';
          const label = `${lvl}${suffix} level${slots !== undefined ? ` (${slots} slot${slots !== 1 ? 's' : ''})` : ''}`;
          return (
            <span key={level}>
              <em>{label}: </em>
              {spells.map((raw, i) => (
                <React.Fragment key={i}>
                  {i > 0 && ', '}
                  <SpellLink raw={raw} />
                </React.Fragment>
              ))}.{' '}
            </span>
          );
        })}
      {!hideDaily && entry.daily && Object.entries(entry.daily).map(([key, spells]) => {
        const count = key.replace('e', '');
        const label = key.endsWith('e') ? `${count}/day each` : `${count}/day`;
        return (
          <span key={key}>
            <em>{capitalize(label)}: </em>
            {spells.map((raw, i) => (
              <React.Fragment key={i}>
                {i > 0 && ', '}
                <SpellLink raw={raw} />
              </React.Fragment>
            ))}.{' '}
          </span>
        );
      })}
    </div>
  );
}

function Section({
  title,
  entries,
  onRoll,
  spellcastingEntries,
}: {
  title: string;
  entries: FiveEToolsEntry[];
  onRoll: OnRoll;
  spellcastingEntries?: SpellcastingEntry[];
}) {
  if (!entries.length && !spellcastingEntries?.length) return null;
  return (
    <>
      <div className="stat-block__divider" />
      <h4 className="stat-block__section-title">{title}</h4>
      {spellcastingEntries?.map((sc, i) => (
        <SpellcastingBlock key={`sc-${i}`} entry={sc} onRoll={onRoll} />
      ))}
      {entries.map((entry, i) => (
        <div key={i} className="stat-block__feature">
          {entry.name && <strong className="stat-block__feature-name">{cleanTags(entry.name)}. </strong>}
          {entry.entries ? renderEntriesAsNodes(entry.entries, onRoll, `${title}-${i}`) : ''}
        </div>
      ))}
    </>
  );
}

// ── Helpers ───────────────────────────────────────────────────────────────────

function formatType(raw: FiveEToolsRawData['type']): string {
  if (!raw) return '';
  if (typeof raw === 'string') return raw;
  return raw.tags?.length ? `${raw.type} (${raw.tags.join(', ')})` : raw.type;
}

function formatSize(sizes?: string[]): string {
  if (!sizes?.length) return '';
  return sizes.map((s) => SIZE_MAP[s] ?? s).join('/');
}

function formatAlignment(alignment?: string[]): string {
  if (!alignment?.length) return 'unaligned';
  const parts = alignment.map((a) => ALIGNMENT_MAP[a] ?? a);
  // Combine L/C/N + G/E/N pairs
  if (parts.length === 2) return parts.join(' ');
  return parts.join(' ');
}

function formatAc(ac?: (number | { ac?: number; from?: string[]; condition?: string })[]): string {
  if (!ac?.length) return '—';
  return ac.map((entry) => {
    if (typeof entry === 'number') return String(entry);
    const base = entry.ac ?? '';
    const from = entry.from?.join(', ') ?? '';
    const cond = entry.condition ?? '';
    return [String(base), from && `(${from})`, cond].filter(Boolean).join(' ');
  }).join(', ');
}

function formatCr(cr: FiveEToolsRawData['cr']): string {
  if (!cr) return '—';
  const crStr = typeof cr === 'string' ? cr : cr.cr;
  const xp = CR_XP[crStr];
  return xp ? `${crStr} (${xp} XP)` : crStr;
}

function formatSpeed(speed?: FiveEToolsRawData['speed']): string {
  if (!speed) return '—';
  const parts: string[] = [];
  if (speed.walk !== undefined) parts.push(`${speed.walk} ft.`);
  if (speed.fly !== undefined) {
    if (typeof speed.fly === 'object') {
      const cond = speed.fly.condition ? ` ${speed.fly.condition}` : (speed.canHover ? ' (hover)' : '');
      parts.push(`fly ${speed.fly.number} ft.${cond}`);
    } else {
      parts.push(`fly ${speed.fly} ft.${speed.canHover ? ' (hover)' : ''}`);
    }
  }
  if (speed.swim !== undefined) parts.push(`swim ${speed.swim} ft.`);
  if (speed.burrow !== undefined) parts.push(`burrow ${speed.burrow} ft.`);
  if (speed.climb !== undefined) parts.push(`climb ${speed.climb} ft.`);
  return parts.join(', ') || '—';
}

function formatList(
  items?: (string | { immune?: string[]; resist?: string[]; vulnerable?: string[]; conditionImmune?: string[]; note?: string })[]
): string {
  if (!items?.length) return '';
  return items.map((item) => {
    if (typeof item === 'string') return capitalize(item);
    const inner = item.immune ?? item.resist ?? item.vulnerable ?? item.conditionImmune ?? [];
    const base = inner.map(capitalize).join(', ');
    return item.note ? `${base} ${item.note}` : base;
  }).join('; ');
}

// ── Main component ────────────────────────────────────────────────────────────

const CreatureStatBlock: React.FC<Props> = ({ data }) => {
  const crStr = typeof data.cr === 'string' ? data.cr : data.cr?.cr;
  const immuneStr = formatList(data.immune);
  const resistStr = formatList(data.resist);
  const vulnerableStr = formatList(data.vulnerable);

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

  return (
    <div className="stat-block">
      {/* Dice roll result banner */}
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

      {/* Header */}
      <div className="stat-block__header">
        <h2 className="stat-block__name">{data.name}</h2>
        <p className="stat-block__meta">
          {formatSize(data.size)} {formatType(data.type)}, {formatAlignment(data.alignment)}
        </p>
      </div>

      <div className="stat-block__divider stat-block__divider--thick" />

      {/* Core stats */}
      <PropertyLine label="Armor Class" value={formatAc(data.ac)} />
      <PropertyLine
        label="Hit Points"
        value={data.hp ? `${data.hp.average}${data.hp.formula ? ` (${data.hp.formula})` : ''}` : '—'}
      />
      <PropertyLine label="Speed" value={formatSpeed(data.speed)} />

      <div className="stat-block__divider stat-block__divider--thick" />

      {/* Ability scores */}
      <AbilityScores data={data} onRoll={onRoll} />

      <div className="stat-block__divider stat-block__divider--thick" />

      {/* Secondary stats */}
      {data.save && Object.keys(data.save).length > 0 && (
        <PropertyLine
          label="Saving Throws"
          value={renderDiceNodes(
            Object.entries(data.save).map(([k, v]) => `${capitalize(k)} ${v}`).join(', '),
            onRoll,
            'saving-throws'
          )}
        />
      )}
      {data.skill && Object.keys(data.skill).length > 0 && (
        <PropertyLine
          label="Skills"
          value={renderDiceNodes(
            Object.entries(data.skill).map(([k, v]) => `${capitalize(k)} ${v}`).join(', '),
            onRoll,
            'skills'
          )}
        />
      )}
      {vulnerableStr && <PropertyLine label="Damage Vulnerabilities" value={vulnerableStr} />}
      {resistStr && <PropertyLine label="Damage Resistances" value={resistStr} />}
      {immuneStr && <PropertyLine label="Damage Immunities" value={immuneStr} />}
      {data.conditionImmune && data.conditionImmune.length > 0 && (
        <PropertyLine
          label="Condition Immunities"
          value={
            data.conditionImmune.map((item, gi) => {
              const names = typeof item === 'string' ? [item] : (item.conditionImmune ?? []);
              const note = typeof item === 'string' ? undefined : item.note;
              return (
                <React.Fragment key={gi}>
                  {gi > 0 && '; '}
                  {names.map((name, ni) => (
                    <React.Fragment key={ni}>
                      {ni > 0 && ', '}
                      <ConditionLink name={capitalize(name)} />
                    </React.Fragment>
                  ))}
                  {note && ` ${note}`}
                </React.Fragment>
              );
            })
          }
        />
      )}
      {data.senses?.length ? (
        <PropertyLine label="Senses" value={[...data.senses, `passive Perception ${data.passive ?? '—'}`].join(', ')} />
      ) : data.passive !== undefined ? (
        <PropertyLine label="Senses" value={`passive Perception ${data.passive}`} />
      ) : null}
      {data.languages?.length ? (
        <PropertyLine label="Languages" value={data.languages.join(', ')} />
      ) : null}
      <PropertyLine label="Challenge" value={formatCr(data.cr)} />

      {/* Feature sections */}
      <Section
        title="Traits"
        entries={data.trait ?? []}
        onRoll={onRoll}
        spellcastingEntries={data.spellcasting?.filter(sc => !sc.displayAs || sc.displayAs === 'trait')}
      />
      <Section
        title="Actions"
        entries={data.action ?? []}
        onRoll={onRoll}
        spellcastingEntries={data.spellcasting?.filter(sc => sc.displayAs === 'action')}
      />
      <Section
        title="Bonus Actions"
        entries={data.bonus ?? []}
        onRoll={onRoll}
        spellcastingEntries={data.spellcasting?.filter(sc => sc.displayAs === 'bonus')}
      />
      <Section
        title="Reactions"
        entries={data.reaction ?? []}
        onRoll={onRoll}
        spellcastingEntries={data.spellcasting?.filter(sc => sc.displayAs === 'reaction')}
      />
      {data.legendary?.length ? (
        <Section
          title={`Legendary Actions${crStr ? ` (CR ${crStr})` : ''}`}
          entries={data.legendary}
          onRoll={onRoll}
        />
      ) : null}
    </div>
  );
};

export default CreatureStatBlock;
