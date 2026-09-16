import React, { useState, useCallback } from 'react';
import { FiveEToolsRawData, FiveEToolsEntry } from '../../api/bestiaryClient';
import { spellClient, SpellDetail } from '../../api/spellClient';
import { conditionClient, ConditionDetail } from '../../api/conditionClient';
import { isTaleSpire, rollInTray } from '../../utils/talespire';

// ── Dice rolling ──────────────────────────────────────────────────────────────

export interface RollResult {
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
export function rollExpression(expr: string): RollResult {
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

// Passed down so dice buttons in nested components can trigger a roll
export type OnRoll = (result: RollResult) => void;

export const SCORE_MOD = (score: number) => {
  const mod = Math.floor((score - 10) / 2);
  return `${mod >= 0 ? '+' : ''}${mod}`;
};

// ── Tag stripping ─────────────────────────────────────────────────────────────

/**
 * Strip 5etools {@tag ...} inline directives, leaving only the display text.
 * e.g. {@hit 5} → +5, {@damage 2d6 + 3} → 2d6 + 3, {@dc 14} → DC 14,
 *      {@spell Fireball|PHB} → Fireball, {@condition Grappled|XPHB} → Grappled
 */
export function cleanTags(text: unknown): string {
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
    .replace(/\{@recharge(?: (\d+))?\}/g, (_, n) =>
      n && Number(n) < 6 ? `(Recharge ${n}-6)` : '(Recharge 6)')
    .replace(/\{@spell ([^|}\s]+)[^}]*\}/g, (_, name) => name.replace(/_/g, ' '))
    .replace(/\{@variantrule ([^|}\s]+)[^}]*\}/g, (_, name) => name.replace(/_/g, ' '))
    .replace(/\{@[a-z]+ ([^|}]+)[^}]*\}/g, (_, text) => text);
}

export function renderLooseEntryNodes(raw: unknown, onRoll: OnRoll, keyPrefix: string): React.ReactNode {
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
export function renderToHitNodes(text: string, onRoll: OnRoll, keyPrefix: string): React.ReactNode {
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
 * Splits a plain-text string on **bold** and *italic* markers and returns
 * an array of React nodes. Does NOT split on dice expressions (those are
 * handled by the caller after this pass).
 */
export function renderMarkdownInline(text: string, keyPrefix: string): React.ReactNode[] {
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

export function renderDiceNodes(raw: string, onRoll: OnRoll, keyPrefix: string): React.ReactNode {
  // First pass: split on {@condition ...} tags, keeping them as captured groups
  const condParts = raw.split(/(\{@condition [^}]+\})/g);
  return condParts.map((part, ci) => {
    if (part.startsWith('{@condition ')) {
      const { name } = parseConditionTag(part);
      return <ConditionLink key={`${keyPrefix}-cond-${ci}`} name={name} />;
    }
    const cleaned = cleanTags(part);
    // Second pass: dice expressions (e.g. 2d6+3, 1d6 + 4)
    const parts = cleaned.split(/\b(\d*d\d+(?:\s*[+-]\s*\d+)?)\b/i);
    return parts.map((dp, di) => {
      if (di % 2 === 1) {
        return (
          <button
            key={`${keyPrefix}-cond-${ci}-${di}`}
            className="stat-block__dice-chip"
            title={`Roll ${dp}`}
            onClick={() => onRoll(rollExpression(dp))}
          >
            {dp}
          </button>
        );
      }
      return (
        <React.Fragment key={`${keyPrefix}-cond-${ci}-${di}`}>
          {renderToHitNodes(dp, onRoll, `${keyPrefix}-cond-${ci}-${di}`)}
        </React.Fragment>
      );
    });
  });
}

export function capitalize(s: string): string {
  return s.charAt(0).toUpperCase() + s.slice(1);
}

// ── Entry rendering ───────────────────────────────────────────────────────────

export function renderEntriesAsNodes(
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
    if (e.entries) {
      const body = renderEntriesAsNodes(e.entries, onRoll, `${keyPrefix}-${i}`);
      if (e.name) {
        return (
          <div key={`${keyPrefix}-${i}`} className="stat-block__entry-line">
            <strong>{e.name}. </strong>
            {body}
          </div>
        );
      }
      return <React.Fragment key={`${keyPrefix}-${i}`}>{body}</React.Fragment>;
    }
    return null;
  });
}

// ── Sub-components ────────────────────────────────────────────────────────────

export function AbilityScores({ data, onRoll }: { data: Pick<FiveEToolsRawData, 'str' | 'dex' | 'con' | 'int' | 'wis' | 'cha'>; onRoll: OnRoll }) {
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

export function PropertyLine({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <p className="stat-block__property">
      <strong>{label} </strong>{value}
    </p>
  );
}

/** Extract the display name and optional source from a {@spell Name|Source} tag. */
export function parseSpellTag(raw: string): { name: string; source?: string } {
  const m = raw.match(/\{@spell ([^|}\s]+(?:\s[^|}\s]+)*)(?:\|([^}]*))?\}/);
  if (m) return { name: m[1], source: m[2] || undefined };
  return { name: cleanTags(raw) };
}

/** Extract the display name and optional source from a {@condition Name|Source} tag. */
export function parseConditionTag(raw: string): { name: string; source?: string } {
  const m = raw.match(/\{@condition ([^|}\s]+)(?:\|([^}]*))?\}/);
  if (m) return { name: m[1], source: m[2] || undefined };
  return { name: raw };
}

// ── Spell school lookup ───────────────────────────────────────────────────────

export const SCHOOL_MAP: Record<string, string> = {
  A: 'Abjuration', C: 'Conjuration', D: 'Divination', E: 'Enchantment',
  V: 'Evocation', I: 'Illusion', N: 'Necromancy', T: 'Transmutation',
};

export function formatSpellLevel(level: number): string {
  if (level === 0) return 'Cantrip';
  const suffixes: Record<number, string> = { 1: 'st', 2: 'nd', 3: 'rd' };
  return `${level}${suffixes[level] ?? 'th'}-level`;
}

export function formatComponents(c?: SpellDetail['rawData']['components']): string {
  if (!c) return '';
  const parts: string[] = [];
  if (c.v) parts.push('V');
  if (c.s) parts.push('S');
  if (c.m) parts.push(`M (${typeof c.m === 'string' ? c.m : c.m.text})`);
  return parts.join(', ');
}

export function SpellPopover({ spell, onClose }: { spell: SpellDetail; onClose: () => void }) {
  const raw = spell.rawData;
  const school = SCHOOL_MAP[raw.school] ?? raw.school;
  const levelLine = raw.level === 0
    ? `${school} cantrip`
    : `${formatSpellLevel(raw.level)} ${school}`;

  const castingTime = raw.time?.map(t => `${t.number} ${t.unit}`).join(' or ') ?? '—';
  const range = raw.range
    ? (raw.range.type === 'point' && raw.range.distance
        ? (raw.range.distance.amount != null
            ? `${raw.range.distance.amount} ${raw.range.distance.type}`
            : raw.range.distance.type)
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
            <div key={i} className="spell-popover__entry">{renderLooseEntryNodes(e, onRoll, `spell-entry-${i}`)}</div>
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

export function ConditionPopover({ condition, onClose }: { condition: ConditionDetail; onClose: () => void }) {
  const raw = condition.rawData;

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
    <div className="condition-popover__overlay" onClick={onClose}>
      <div className="condition-popover__modal" onClick={e => e.stopPropagation()}>
        <button className="condition-popover__close" aria-label="Close condition" onClick={onClose}>✕</button>
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
          <p className="stat-block__meta">{condition.source} {condition.type}</p>
        </div>
        <div className="stat-block__divider stat-block__divider--thick" />
        <div className="condition-popover__entries">
          {(raw.entries as unknown[] | undefined)?.map((e, i) => (
            <div key={i} className="condition-popover__entry">{renderLooseEntryNodes(e, onRoll, `cond-entry-${i}`)}</div>
          ))}
        </div>
      </div>
    </div>
  );
}

export function SpellLink({ raw }: { raw: string }) {
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

export function ConditionLink({ name }: { name: string }) {
  const [condition, setCondition] = useState<ConditionDetail | null>(null);
  const [loading, setLoading] = useState(false);

  const open = useCallback(async (e: React.MouseEvent) => {
    e.stopPropagation();
    setLoading(true);
    const result = await conditionClient.resolveCondition(name);
    setLoading(false);
    if (result) setCondition(result);
  }, [name]);

  return (
    <>
      <button
        className="stat-block__condition-link"
        onClick={open}
        disabled={loading}
        title={`View ${name}`}
      >
        {loading ? `${name}…` : name}
      </button>
      {condition && <ConditionPopover condition={condition} onClose={() => setCondition(null)} />}
    </>
  );
}
