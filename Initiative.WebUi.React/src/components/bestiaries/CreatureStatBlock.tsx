import React from 'react';
import { FiveEToolsRawData, FiveEToolsEntry } from '../../api/bestiaryClient';
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
function renderText(text: string): string {
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

// ── Sub-components ────────────────────────────────────────────────────────────

function AbilityScores({ data }: { data: FiveEToolsRawData }) {
  const scores: [string, number | undefined][] = [
    ['STR', data.str], ['DEX', data.dex], ['CON', data.con],
    ['INT', data.int], ['WIS', data.wis], ['CHA', data.cha],
  ];
  return (
    <div className="stat-block__abilities">
      {scores.map(([label, score]) => (
        <div key={label} className="stat-block__ability">
          <div className="stat-block__ability-label">{label}</div>
          <div className="stat-block__ability-score">
            {score ?? '—'} {score !== undefined ? `(${SCORE_MOD(score)})` : ''}
          </div>
        </div>
      ))}
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

function Section({ title, entries }: { title: string; entries: FiveEToolsEntry[] }) {
  if (!entries.length) return null;
  return (
    <>
      <div className="stat-block__divider" />
      <h4 className="stat-block__section-title">{title}</h4>
      {entries.map((entry, i) => (
        <div key={i} className="stat-block__feature">
          {entry.name && <strong className="stat-block__feature-name">{entry.name}. </strong>}
          <span>{entry.entries ? renderEntries(entry.entries) : ''}</span>
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

  return (
    <div className="stat-block">
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
      <AbilityScores data={data} />

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
      {data.trait?.length ? <Section title="Traits" entries={data.trait} /> : null}
      {data.action?.length ? <Section title="Actions" entries={data.action} /> : null}
      {data.bonus?.length ? <Section title="Bonus Actions" entries={data.bonus} /> : null}
      {data.reaction?.length ? <Section title="Reactions" entries={data.reaction} /> : null}
      {data.legendary?.length ? (
        <Section
          title={`Legendary Actions${crStr ? ` (CR ${crStr})` : ''}`}
          entries={data.legendary}
        />
      ) : null}
    </div>
  );
};

export default CreatureStatBlock;
