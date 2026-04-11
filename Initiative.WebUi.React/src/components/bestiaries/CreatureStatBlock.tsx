import React, { useState, useCallback } from 'react';
import { FiveEToolsRawData, FiveEToolsEntry } from '../../api/bestiaryClient';
import { spellClient, SpellDetail } from '../../api/spellClient';
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
function cleanTags(text: unknown): string {
  const normalized = typeof text === 'string'
    ? text
    : (typeof text === 'number' || typeof text === 'boolean')
      ? String(text)
      : '';

  return normalized
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

function renderLooseEntryNodes(raw: unknown, onRoll: OnRoll, keyPrefix: string): React.ReactNode {
  if (typeof raw === 'string') return renderDiceNodes(raw, onRoll, keyPrefix);
  if (typeof raw === 'number' || typeof raw === 'boolean') {
    return renderDiceNodes(String(raw), onRoll, keyPrefix);
  }

  if (raw && typeof raw === 'object') {
    const maybeEntry = raw as Partial<FiveEToolsEntry> & { text?: unknown };

    if (Array.isArray(maybeEntry.entries)) {
      return renderEntriesAsNodes(maybeEntry.entries as (string | FiveEToolsEntry)[], onRoll, keyPrefix);
    }

    if (typeof maybeEntry.text === 'string') {
      return renderDiceNodes(maybeEntry.text, onRoll, keyPrefix);
    }
  }

  return null;
}

/**
 * Split a plain-text segment on standalone to-hit modifiers like +4 or -1.
 * A to-hit modifier is a sign followed by digits that is NOT immediately
 * preceded by a digit or 'd' (which would make it part of a dice expression).
 */
function renderToHitNodes(text: string, onRoll: OnRoll, keyPrefix: string): React.ReactNode {
  // Negative lookbehind: not preceded by a digit or 'd'
  const parts = text.split(/((?<![\dd])[+-]\d+)/);
  return parts.map((part, i) => {
    if (i % 2 === 1) {
      const expr = `1d20${part.replace(/\s/g, '')}`;
      return (
        <button
          key={`${keyPrefix}-hit-${i}`}
          className="stat-block__dice-chip"
          title={`Roll ${expr}`}
          onClick={() => onRoll(rollExpression(expr))}
        >
          {part}
        </button>
      );
    }
    return <React.Fragment key={`${keyPrefix}-hit-${i}`}>{renderMarkdownInline(part, `${keyPrefix}-hit-${i}`)}</React.Fragment>;
  });
}

/**
 * Render a cleaned string as React nodes, turning dice expressions into
 * clickable <button> chips that call onRoll when clicked.
 * A second pass over plain-text segments converts standalone to-hit modifiers
 * (e.g. +4) into 1d20+4 chips.
 *
 * split() with a capturing group produces alternating [text, dice, text, dice, …]
 * so odd indices are always the captured dice expression — no re-test needed.
 */
/**
 * Splits a plain-text string on **bold** and *italic* markers and returns
 * an array of React nodes. Does NOT split on dice expressions (those are
 * handled by the caller after this pass).
 */
function renderMarkdownInline(text: string, keyPrefix: string): React.ReactNode[] {
  // Split on bold first (**...** or __...__), then italic (*...* or _..._)
  const boldParts = text.split(/(\*\*[^*]+\*\*|__[^_]+__)/);
  const nodes: React.ReactNode[] = [];
  boldParts.forEach((chunk, bi) => {
    if (/^(\*\*|__)/.test(chunk)) {
      const inner = chunk.replace(/^\*\*|^__|(\*\*|__)$/g, '');
      nodes.push(<strong key={`${keyPrefix}-b${bi}`}>{inner}</strong>);
    } else {
      // Split remaining plain text on italic markers
      const italicParts = chunk.split(/(\*[^*]+\*|_[^_]+_)/);
      italicParts.forEach((ic, ii) => {
        if (/^(\*|_)/.test(ic)) {
          const inner = ic.replace(/^\*|^_|(\*|_)$/g, '');
          nodes.push(<em key={`${keyPrefix}-b${bi}-i${ii}`}>{inner}</em>);
        } else if (ic) {
          nodes.push(<React.Fragment key={`${keyPrefix}-b${bi}-i${ii}`}>{ic}</React.Fragment>);
        }
      });
    }
  });
  return nodes;
}

function renderDiceNodes(raw: string, onRoll: OnRoll, keyPrefix: string): React.ReactNode {
  const cleaned = cleanTags(raw);
  // First pass: dice expressions (e.g. 2d6+3, 1d6 + 4)
  const parts = cleaned.split(/\b(\d*d\d+(?:\s*[+-]\s*\d+)?)\b/i);
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
    // Even index → plain text; second pass for standalone to-hit modifiers
    return (
      <React.Fragment key={`${keyPrefix}-${i}`}>
        {renderToHitNodes(part, onRoll, `${keyPrefix}-${i}`)}
      </React.Fragment>
    );
  });
}

function capitalize(s: string): string {
  return s.charAt(0).toUpperCase() + s.slice(1);
}

// ── Entry rendering ───────────────────────────────────────────────────────────

function renderEntriesAsNodes(
  entries: (string | FiveEToolsEntry)[],
  onRoll: OnRoll,
  keyPrefix: string
): React.ReactNode {
  return entries.map((e, i) => {
    if (typeof e === 'string') return renderDiceNodes(e, onRoll, `${keyPrefix}-${i}`);
    if (e.type === 'list' && e.items) {
      return (
        <ul key={`${keyPrefix}-${i}`} className="stat-block__feature-list">
          {e.items.map((item, j) => {
            if (typeof item === 'string') return <li key={j}>{renderDiceNodes(item, onRoll, `${keyPrefix}-${i}-${j}`)}</li>;
            return (
              <li key={j}>
                {item.name && <strong>{item.name}. </strong>}
                {item.entries && renderEntriesAsNodes(item.entries, onRoll, `${keyPrefix}-${i}-${j}`)}
              </li>
            );
          })}
        </ul>
      );
    }
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

/** Extract the display name and optional source from a {@spell Name|Source} tag. */
function parseSpellTag(raw: string): { name: string; source?: string } {
  const m = raw.match(/\{@spell ([^|}\s]+(?:\s[^|}\s]+)*)(?:\|([^}]*))?\}/);
  if (m) return { name: m[1], source: m[2] || undefined };
  return { name: cleanTags(raw) };
}

// ── Spell school lookup ───────────────────────────────────────────────────────

const SCHOOL_MAP: Record<string, string> = {
  A: 'Abjuration', C: 'Conjuration', D: 'Divination', E: 'Enchantment',
  V: 'Evocation', I: 'Illusion', N: 'Necromancy', T: 'Transmutation',
};

function formatSpellLevel(level: number): string {
  if (level === 0) return 'Cantrip';
  const suffixes: Record<number, string> = { 1: 'st', 2: 'nd', 3: 'rd' };
  return `${level}${suffixes[level] ?? 'th'}-level`;
}

function formatComponents(c?: SpellDetail['rawData']['components']): string {
  if (!c) return '';
  const parts: string[] = [];
  if (c.v) parts.push('V');
  if (c.s) parts.push('S');
  if (c.m) parts.push(`M (${typeof c.m === 'string' ? c.m : c.m.text})`);
  return parts.join(', ');
}

function SpellPopover({ spell, onClose }: { spell: SpellDetail; onClose: () => void }) {
  const raw = spell.rawData;
  const school = SCHOOL_MAP[raw.school] ?? raw.school;
  const levelLine = raw.level === 0
    ? `${school} cantrip`
    : `${formatSpellLevel(raw.level)} ${school}`;

  const castingTime = raw.time?.map(t => `${t.number} ${t.unit}`).join(' or ') ?? '—';
  const range = raw.range
    ? (raw.range.type === 'point' && raw.range.distance
        ? `${raw.range.distance.amount} ${raw.range.distance.type}`
        : raw.range.type)
    : '—';
  const components = formatComponents(raw.components) || '—';
  const duration = raw.duration?.map(d => {
    if (d.type === 'instant') return 'Instantaneous';
    if (d.type === 'permanent') return 'Until dispelled';
    const dur = d.duration ? `${d.duration.amount} ${d.duration.type}` : d.type;
    return d.concentration ? `Concentration, up to ${dur}` : dur;
  }).join(' or ') ?? '—';

  const [lastRoll, setLastRoll] = useState<RollResult | null>(null);
  const onRoll = useCallback((result: RollResult) => {
    if (isTaleSpire()) {
      rollInTray(result.expression, result.expression);
    } else {
      setLastRoll(result);
    }
  }, []);
  const rollDetail = lastRoll && lastRoll.rolls.length > 1 ? ` [${lastRoll.rolls.join(', ')}]` : '';

  return (
    <div className="spell-popover__overlay" onClick={onClose}>
      <div className="spell-popover__modal" onClick={e => e.stopPropagation()}>
        <button className="spell-popover__close" aria-label="Close spell" onClick={onClose}>✕</button>
        {lastRoll && (
          <div className="stat-block__roll-banner">
            <span className="stat-block__roll-expression">{lastRoll.expression}</span>
            <span className="stat-block__roll-total">{lastRoll.total}</span>
            {rollDetail && <span className="stat-block__roll-detail">{rollDetail}</span>}
            <button className="stat-block__roll-dismiss" aria-label="Dismiss" onClick={() => setLastRoll(null)}>×</button>
          </div>
        )}
        <div className="stat-block__header">
          <h2 className="stat-block__name">{raw.name}</h2>
          <p className="stat-block__meta">{levelLine}</p>
        </div>
        <div className="stat-block__divider stat-block__divider--thick" />
        <p className="stat-block__property"><strong>Casting Time </strong>{castingTime}</p>
        <p className="stat-block__property"><strong>Range </strong>{capitalize(range)}</p>
        <p className="stat-block__property"><strong>Components </strong>{components}</p>
        <p className="stat-block__property"><strong>Duration </strong>{capitalize(duration)}</p>
        <div className="stat-block__divider stat-block__divider--thick" />
        <div className="spell-popover__entries">
          {(raw.entries as unknown[] | undefined)?.map((e, i) => (
            <p key={i} className="spell-popover__entry">{renderLooseEntryNodes(e, onRoll, `spell-entry-${i}`)}</p>
          ))}
          {raw.entriesHigherLevel?.map((hl, i) => (
            <p key={`hl-${i}`} className="spell-popover__entry">
              <em><strong>{hl.name}. </strong></em>
              {(hl.entries as unknown[]).map((e, j) => renderLooseEntryNodes(e, onRoll, `spell-hl-${i}-${j}`))}
            </p>
          ))}
        </div>
      </div>
    </div>
  );
}

function SpellLink({ raw }: { raw: string }) {
  const [spell, setSpell] = useState<SpellDetail | null>(null);
  const [loading, setLoading] = useState(false);
  const { name, source } = parseSpellTag(raw);

  const open = useCallback(async (e: React.MouseEvent) => {
    e.stopPropagation();
    setLoading(true);
    const result = await spellClient.resolveSpell(name, source);
    setLoading(false);
    if (result) setSpell(result);
  }, [name, source]);

  return (
    <>
      <button
        className="stat-block__spell-link"
        onClick={open}
        disabled={loading}
        title={`View ${name}`}
      >
        {loading ? `${name}…` : name}
      </button>
      {spell && <SpellPopover spell={spell} onClose={() => setSpell(null)} />}
    </>
  );
}

type SpellcastingEntry = NonNullable<FiveEToolsRawData['spellcasting']>[number];

function SpellcastingBlock({ entry }: { entry: SpellcastingEntry }) {
  const hideDaily = entry.hidden?.includes('daily');

  return (
    <div className="stat-block__feature">
      <strong className="stat-block__feature-name">{entry.name}. </strong>
      {entry.headerEntries?.map((h, i) => (
        <span key={i}>{cleanTags(h)} </span>
      ))}
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
        <SpellcastingBlock key={`sc-${i}`} entry={sc} />
      ))}
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
      <Section
        title="Actions"
        entries={data.action ?? []}
        onRoll={onRoll}
        spellcastingEntries={data.spellcasting?.filter(sc => !sc.displayAs || sc.displayAs === 'action')}
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
