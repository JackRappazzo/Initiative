import React, { useState, useCallback } from 'react';
import { FiveEToolsRawData, FiveEToolsEntry } from '../../api/bestiaryClient';
import { isTaleSpire, rollInTray } from '../../utils/talespire';
import './CreatureStatBlock.css';

// ── Dice rolling ──────────────────────────────────────────────────────────────

interface RollResult {
  expression: string;
  rolls: number[];
  total: number;
}

/** Roll a single die with `sides` faces. */
function rollDie(sides: number): number {
  return Math.floor(Math.random() * sides) + 1;
}

/**
 * Evaluate a dice expression like "2d6+3", "1d20", "4d8-1".
 * Returns individual rolls and the final total.
 */
function rollExpression(expr: string): RollResult {
  // Normalise: remove spaces, lower-case 'd'
  const normalised = expr.replace(/\s+/g, '').toLowerCase();
  // Pattern: optional count 'd' sides optional modifier
  const match = normalised.match(/^(\d*)d(\d+)([+-]\d+)?$/);
  if (!match) {
    // Plain number — treat as a flat bonus
    const flat = parseInt(normalised, 10);
    return { expression: expr, rolls: [], total: isNaN(flat) ? 0 : flat };
  }
  const count = parseInt(match[1] || '1', 10);
  const sides = parseInt(match[2], 10);
  const modifier = match[3] ? parseInt(match[3], 10) : 0;
  const rolls = Array.from({ length: count }, () => rollDie(sides));
  const total = rolls.reduce((s, r) => s + r, 0) + modifier;
  return { expression: expr, rolls, total };
}

// ── Dice regex (matches expressions like 2d6, 1d20+5, 3d8-2) ─────────────────
const DICE_RE = /\b(\d*d\d+(?:[+-]\d+)?)\b/gi;

interface Props {
  data: FiveEToolsRawData;
}

// Passed down so dice buttons in nested components can trigger a roll
type OnRoll = (result: RollResult) => void;

// ── Lookup tables ─────────────────────────────────────────────────────────────

const SIZE_MAP: Record<string, string> = {
  T: 'Tiny', S: 'Small', M: 'Medium', L: 'Large', H: 'Huge', G: 'Gargantuan',
};

const ALIGNMENT_MAP: Record<string, string> = {
  L: 'Lawful', N: 'Neutral', C: 'Chaotic',
  G: 'Good', E: 'Evil',
  U: 'Unaligned', A: 'Any alignment',
};

const SCORE_MOD = (score: number) => {
  const mod = Math.floor((score - 10) / 2);
  return `${mod >= 0 ? '+' : ''}${mod}`;
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

// ── Tag stripping ─────────────────────────────────────────────────────────────

/**
 * Strip 5etools {@tag ...} inline directives, leaving only the display text.
 * e.g. {@hit 5} → +5, {@damage 2d6 + 3} → 2d6 + 3, {@dc 14} → DC 14,
 *      {@spell Fireball|PHB} → Fireball, {@condition Grappled|XPHB} → Grappled
 */
function cleanTags(text: string): string {
  return text
    .replace(/\{@hit ([^}]+)\}/g, (_, n) => `+${n}`)
    .replace(/\{@damage ([^}]+)\}/g, (_, d) => d)
    .replace(/\{@dice ([^}]+)\}/g, (_, d) => d)
    .replace(/\{@dc ([^}]+)\}/g, (_, n) => `DC ${n}`)
    .replace(/\{@h\}/g, 'Hit: ')
    .replace(/\{@atkr [^}]+\}/g, '')
    .replace(/\{@actSave ([^}]+)\}/g, (_, s) => `${capitalize(s)} Saving Throw`)
    .replace(/\{@actSaveFail\}/g, 'Failure:')
    .replace(/\{@actSaveSuccess\}/g, 'Success:')
    .replace(/\{@actSaveSuccessOrFail\}/g, 'Success or Failure:')
    .replace(/\{@recharge(?:\s*\d*)?\}/g, '(Recharge)')
    .replace(/\{@spell ([^|}\s]+)[^}]*\}/g, (_, name) => name.replace(/_/g, ' '))
    .replace(/\{@condition ([^|}\s]+)[^}]*\}/g, (_, name) => name)
    .replace(/\{@variantrule ([^|}\s]+)[^}]*\}/g, (_, name) => name.replace(/_/g, ' '))
    .replace(/\{@[a-z]+ ([^|}]+)[^}]*\}/g, (_, text) => text)
    .trim();
}

/** Plain text — used for attribute values that aren't rendered as React nodes */
function renderText(text: string): string {
  return cleanTags(text);
}

/**
 * Render a cleaned string as React nodes, turning dice expressions into
 * clickable <button> chips that call onRoll when clicked.
 *
 * split() with a capturing group produces alternating [text, dice, text, dice, …]
 * so odd indices are always the captured dice expression — no re-test needed.
 */
function renderDiceNodes(raw: string, onRoll: OnRoll, keyPrefix: string): React.ReactNode {
  const cleaned = cleanTags(raw);
  // Use a non-stateful copy so the global flag doesn't bleed between calls
  const parts = cleaned.split(/\b(\d*d\d+(?:[+-]\d+)?)\b/i);
  return parts.map((part, i) => {
    if (i % 2 === 1) {
      // odd index → captured dice expression
      return (
        <button
          key={`${keyPrefix}-${i}`}
          className="stat-block__dice-chip"
          title={`Roll ${part}`}
          onClick={() => onRoll(rollExpression(part))}
        >
          {part}
        </button>
      );
    }
    return <React.Fragment key={`${keyPrefix}-${i}`}>{part}</React.Fragment>;
  });
}

function capitalize(s: string): string {
  return s.charAt(0).toUpperCase() + s.slice(1);
}

// ── Entry rendering ───────────────────────────────────────────────────────────

function renderEntries(entries: (string | FiveEToolsEntry)[]): string {
  return entries.map((e) => {
    if (typeof e === 'string') return renderText(e);
    if (e.entries) return renderEntries(e.entries);
    return '';
  }).filter(Boolean).join(' ');
}

function renderEntriesAsNodes(
  entries: (string | FiveEToolsEntry)[],
  onRoll: OnRoll,
  keyPrefix: string
): React.ReactNode {
  return entries.map((e, i) => {
    if (typeof e === 'string') return renderDiceNodes(e, onRoll, `${keyPrefix}-${i}`);
    if (e.entries) return renderEntriesAsNodes(e.entries, onRoll, `${keyPrefix}-${i}`);
    return null;
  });
}

// ── Sub-components ────────────────────────────────────────────────────────────

function AbilityScores({ data, onRoll }: { data: FiveEToolsRawData; onRoll: OnRoll }) {
  const scores: [string, number | undefined][] = [
    ['STR', data.str], ['DEX', data.dex], ['CON', data.con],
    ['INT', data.int], ['WIS', data.wis], ['CHA', data.cha],
  ];
  return (
    <div className="stat-block__abilities">
      {scores.map(([label, score]) => {
        const modStr = score !== undefined ? SCORE_MOD(score) : null;
        const modNum = score !== undefined ? Math.floor((score - 10) / 2) : null;
        const rollExpr = modNum !== null
          ? `1d20${modNum >= 0 ? '+' : ''}${modNum}`
          : '1d20';
        return (
          <div key={label} className="stat-block__ability">
            <div className="stat-block__ability-label">{label}</div>
            <div className="stat-block__ability-score">
              {score ?? '—'}
              {modStr !== null && (
                <>
                  {' '}
                  <button
                    className="stat-block__dice-chip stat-block__dice-chip--mod"
                    title={`Roll 1d20 ${modStr}`}
                    onClick={() => onRoll(rollExpression(rollExpr))}
                  >
                    ({modStr})
                  </button>
                </>
              )}
            </div>
          </div>
        );
      })}
    </div>
  );
}

function PropertyLine({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <p className="stat-block__property">
      <strong>{label} </strong>{value}
    </p>
  );
}

function Section({ title, entries, onRoll }: { title: string; entries: FiveEToolsEntry[]; onRoll: OnRoll }) {
  if (!entries.length) return null;
  return (
    <>
      <div className="stat-block__divider" />
      <h4 className="stat-block__section-title">{title}</h4>
      {entries.map((entry, i) => (
        <div key={i} className="stat-block__feature">
          {entry.name && <strong className="stat-block__feature-name">{entry.name}. </strong>}
          <span>{entry.entries ? renderEntriesAsNodes(entry.entries, onRoll, `${title}-${i}`) : ''}</span>
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
  if (speed.fly !== undefined) parts.push(`fly ${speed.fly} ft.${speed.canHover ? ' (hover)' : ''}`);
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
  const condImmStr = formatList(data.conditionImmune);

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
          value={Object.entries(data.save).map(([k, v]) => `${capitalize(k)} ${v}`).join(', ')}
        />
      )}
      {data.skill && Object.keys(data.skill).length > 0 && (
        <PropertyLine
          label="Skills"
          value={Object.entries(data.skill).map(([k, v]) => `${capitalize(k)} ${v}`).join(', ')}
        />
      )}
      {vulnerableStr && <PropertyLine label="Damage Vulnerabilities" value={vulnerableStr} />}
      {resistStr && <PropertyLine label="Damage Resistances" value={resistStr} />}
      {immuneStr && <PropertyLine label="Damage Immunities" value={immuneStr} />}
      {condImmStr && <PropertyLine label="Condition Immunities" value={condImmStr} />}
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
      {data.trait?.length ? <Section title="Traits" entries={data.trait} onRoll={onRoll} /> : null}
      {data.action?.length ? <Section title="Actions" entries={data.action} onRoll={onRoll} /> : null}
      {data.bonus?.length ? <Section title="Bonus Actions" entries={data.bonus} onRoll={onRoll} /> : null}
      {data.reaction?.length ? <Section title="Reactions" entries={data.reaction} onRoll={onRoll} /> : null}
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
