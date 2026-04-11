import { parse, stringify } from 'yaml';
import { CustomCreaturePayload, FiveEToolsEntry, FiveEToolsRawData } from '../api/bestiaryClient';

type StatblockMap = Record<string, unknown>;

const ABILITY_ORDER = ['str', 'dex', 'con', 'int', 'wis', 'cha'] as const;
const ABILITY_TO_FULL: Record<string, string> = {
  str: 'strength',
  dex: 'dexterity',
  con: 'constitution',
  int: 'intelligence',
  wis: 'wisdom',
  cha: 'charisma',
};
const FULL_TO_ABILITY: Record<string, string> = {
  strength: 'str',
  dexterity: 'dex',
  constitution: 'con',
  intelligence: 'int',
  wisdom: 'wis',
  charisma: 'cha',
};

const SIZE_TO_SHORT: Record<string, string> = {
  tiny: 'T',
  small: 'S',
  medium: 'M',
  large: 'L',
  huge: 'H',
  gargantuan: 'G',
};

const SHORT_TO_SIZE: Record<string, string> = {
  T: 'Tiny',
  S: 'Small',
  M: 'Medium',
  L: 'Large',
  H: 'Huge',
  G: 'Gargantuan',
};

export function creatureToFantasyStatblockYaml(rawData: FiveEToolsRawData): string {
  const stats = ABILITY_ORDER.map((ability) => toNumber(rawData[ability]) ?? 10);

  const yamlData: StatblockMap = {
    layout: 'Basic 5e Layout',
    name: rawData.name,
    size: mapSizeToLong(rawData.size?.[0]),
    type: getType(rawData.type),
    alignment: rawData.alignment?.join(' ') || undefined,
    ac: getArmorClass(rawData.ac),
    hp: rawData.hp?.average,
    hit_dice: rawData.hp?.formula,
    speed: formatSpeed(rawData.speed),
    stats,
    saves: mapModifierRecord(rawData.save, ABILITY_TO_FULL),
    skillsaves: mapModifierRecord(rawData.skill),
    damage_vulnerabilities: flattenDamageList(rawData.vulnerable),
    damage_resistances: flattenDamageList(rawData.resist),
    damage_immunities: flattenDamageList(rawData.immune),
    condition_immunities: flattenDamageList(rawData.conditionImmune),
    senses: rawData.senses?.join(', '),
    languages: rawData.languages?.join(', '),
    cr: getChallengeRating(rawData.cr),
    traits: mapEntries(rawData.trait),
    actions: mapEntries(rawData.action),
    bonus_actions: mapEntries(rawData.bonus),
    reactions: mapEntries(rawData.reaction),
    legendary_actions: mapEntries(rawData.legendary),
    spells: mapSpells(rawData),
  };

  return stringify(cleanUndefined(yamlData));
}

export function fantasyStatblockYamlToCustomPayload(yamlText: string): CustomCreaturePayload {
  let parsed: unknown;
  try {
    parsed = parse(yamlText);
  } catch (error) {
    const reason = error instanceof Error ? error.message : 'Invalid YAML content.';
    throw new Error(`Unable to parse YAML: ${reason}`);
  }

  if (!parsed || typeof parsed !== 'object' || Array.isArray(parsed)) {
    throw new Error('Expected a YAML object containing Basic 5e statblock fields.');
  }

  const source = parsed as StatblockMap;
  const name = getString(source.name)?.trim();
  if (!name) {
    throw new Error('Missing required field: name');
  }

  const statArray = toNumberArray(source.stats);
  if (statArray && statArray.length !== 6) {
    throw new Error('Field "stats" must contain exactly 6 values: [str, dex, con, int, wis, cha].');
  }

  const speed = parseSpeed(source.speed);
  const saves = parseModifierList(source.saves, FULL_TO_ABILITY);
  const skills = parseModifierList(source.skillsaves);
  const traits = parseTraitsAsText(source.traits);

  return {
    name,
    size: mapSizeToShort(getString(source.size)),
    creatureType: getString(source.type)?.toLowerCase(),
    subtype: getString(source.subtype),
    alignment: getString(source.alignment),
    challengeRating: getScalarString(source.cr),
    isLegendary: (toEntryList(source.legendary_actions)?.length ?? 0) > 0,
    hp: toNumber(source.hp),
    hitDice: getString(source.hit_dice),
    ac: toNumber(source.ac),
    abilityScores: statArray
      ? {
          str: statArray[0],
          dex: statArray[1],
          con: statArray[2],
          int: statArray[3],
          wis: statArray[4],
          cha: statArray[5],
        }
      : undefined,
    speed,
    savingThrows: Object.keys(saves).length ? saves : undefined,
    skills: Object.keys(skills).length ? skills : undefined,
    damageVulnerabilities: toTextList(source.damage_vulnerabilities),
    damageResistances: toTextList(source.damage_resistances),
    damageImmunities: toTextList(source.damage_immunities),
    conditionImmunities: toTextList(source.condition_immunities),
    senses: toTextList(source.senses),
    languages: toTextList(source.languages),
    traits,
    actions: toEntryList(source.actions),
    bonusActions: toEntryList(source.bonus_actions),
    reactions: toEntryList(source.reactions),
    legendaryActions: toEntryList(source.legendary_actions),
    spellcasting: toSpellcasting(source.spells),
  };
}

function cleanUndefined<T>(value: T): T {
  if (Array.isArray(value)) {
    return value
      .map((item) => cleanUndefined(item))
      .filter((item) => item !== undefined && item !== null) as unknown as T;
  }
  if (value && typeof value === 'object') {
    const next: Record<string, unknown> = {};
    for (const [key, inner] of Object.entries(value as Record<string, unknown>)) {
      const cleaned = cleanUndefined(inner);
      if (cleaned !== undefined && cleaned !== null && cleaned !== '') {
        if (Array.isArray(cleaned) && cleaned.length === 0) continue;
        next[key] = cleaned;
      }
    }
    return next as T;
  }
  return value;
}

function getType(typeValue: FiveEToolsRawData['type']): string | undefined {
  if (!typeValue) return undefined;
  if (typeof typeValue === 'string') return typeValue;
  return typeValue.type;
}

function getArmorClass(ac: FiveEToolsRawData['ac']): number | undefined {
  if (!Array.isArray(ac) || ac.length === 0) return undefined;
  const first = ac[0];
  if (typeof first === 'number') return first;
  return toNumber(first.ac);
}

function getChallengeRating(cr: FiveEToolsRawData['cr']): string | undefined {
  if (!cr) return undefined;
  if (typeof cr === 'string') return cr;
  return cr.cr;
}

function mapEntries(entries?: FiveEToolsEntry[]): Array<{ name: string; desc: string }> | undefined {
  if (!entries?.length) return undefined;
  const list = entries
    .map((entry) => {
      const desc = flattenEntryDescription(entry.entries);
      if (!entry.name || !desc) return undefined;
      return {
        name: entry.name,
        desc,
      };
    })
    .filter((entry): entry is { name: string; desc: string } => Boolean(entry));
  return list.length ? list : undefined;
}

function flattenEntryDescription(entries?: (string | FiveEToolsEntry)[]): string {
  if (!entries?.length) return '';
  return entries
    .map((value) => {
      if (typeof value === 'string') return value;
      if (Array.isArray(value.entries)) {
        return flattenEntryDescription(value.entries);
      }
      return value.name ?? '';
    })
    .filter(Boolean)
    .join('\n');
}

function mapSpells(rawData: FiveEToolsRawData): string[] | undefined {
  const statblock = rawData.spellcasting?.[0];
  if (!statblock) return undefined;
  const lines: string[] = [];
  if (Array.isArray(statblock.headerEntries)) {
    lines.push(...statblock.headerEntries.filter(Boolean));
  }
  if (Array.isArray(statblock.will) && statblock.will.length) {
    lines.push(`At will: ${statblock.will.join(', ')}`);
  }
  if (statblock.daily) {
    Object.entries(statblock.daily).forEach(([timesPerDay, spells]) => {
      if (!Array.isArray(spells) || !spells.length) return;
      const count = timesPerDay.replace('e', '');
      lines.push(`${count}/day: ${spells.join(', ')}`);
    });
  }
  if (statblock.spells) {
    Object.entries(statblock.spells).forEach(([level, data]) => {
      if (!Array.isArray(data.spells) || !data.spells.length) return;
      const label = level === '0' ? 'Cantrips (at will)' : `${level}${ordinalSuffix(level)} level`;
      const slotLabel = data.slots ? ` (${data.slots} slots)` : '';
      lines.push(`${label}${slotLabel}: ${data.spells.join(', ')}`);
    });
  }
  return lines.length ? lines : undefined;
}

function ordinalSuffix(level: string): string {
  const numeric = Number(level);
  if (numeric === 1) return 'st';
  if (numeric === 2) return 'nd';
  if (numeric === 3) return 'rd';
  return 'th';
}

function mapModifierRecord(record?: Record<string, string>, keyMap?: Record<string, string>): Array<Record<string, number | string>> | undefined {
  if (!record) return undefined;
  const mapped = Object.entries(record)
    .map(([key, value]) => {
      const normalizedKey = keyMap?.[key.toLowerCase()] ?? key.toLowerCase();
      const numeric = toNumber(value);
      return { [normalizedKey]: numeric ?? value };
    })
    .filter((entry) => Object.values(entry)[0] !== undefined);
  return mapped.length ? mapped : undefined;
}

function flattenDamageList(
  values?:
    | FiveEToolsRawData['vulnerable']
    | FiveEToolsRawData['resist']
    | FiveEToolsRawData['immune']
    | FiveEToolsRawData['conditionImmune']
): string | undefined {
  if (!values?.length) return undefined;
  const list: string[] = [];
  values.forEach((value) => {
    if (typeof value === 'string') {
      list.push(value);
      return;
    }
    Object.entries(value).forEach(([key, inner]) => {
      if (key === 'note') return;
      if (Array.isArray(inner)) {
        list.push(...inner);
      }
    });
  });
  return list.length ? list.join(', ') : undefined;
}

function formatSpeed(speed?: FiveEToolsRawData['speed']): string | undefined {
  if (!speed) return undefined;
  const chunks: string[] = [];
  if (speed.walk != null) chunks.push(`${speed.walk} ft.`);
  if (speed.fly != null) {
    const flyValue = typeof speed.fly === 'number' ? speed.fly : speed.fly.number;
    const hover = typeof speed.fly === 'object' && speed.fly.condition?.toLowerCase().includes('hover') ? ' (hover)' : '';
    chunks.push(`fly ${flyValue} ft.${hover}`);
  }
  if (speed.swim != null) chunks.push(`swim ${speed.swim} ft.`);
  if (speed.burrow != null) chunks.push(`burrow ${speed.burrow} ft.`);
  if (speed.climb != null) chunks.push(`climb ${speed.climb} ft.`);
  return chunks.length ? chunks.join(', ') : undefined;
}

function toEntryList(value: unknown): Array<{ name: string; description: string }> | undefined {
  if (!Array.isArray(value)) return undefined;
  const list = value
    .map((entry) => {
      if (!entry || typeof entry !== 'object' || Array.isArray(entry)) return undefined;
      const typed = entry as StatblockMap;
      const name = getString(typed.name);
      const desc = getString(typed.desc);
      if (!name || !desc) return undefined;
      return { name, description: desc };
    })
    .filter((entry): entry is { name: string; description: string } => Boolean(entry));
  return list.length ? list : undefined;
}

function toSpellcasting(value: unknown): CustomCreaturePayload['spellcasting'] {
  const lines = toTextList(value);
  if (!lines || !lines.length) return undefined;
  return {
    freeformText: lines.join('\n'),
  };
}

function parseSpeed(value: unknown): CustomCreaturePayload['speed'] {
  const raw = getString(value);
  if (!raw) return undefined;
  const speed: NonNullable<CustomCreaturePayload['speed']> = {};
  const segments = raw.split(',').map((segment) => segment.trim()).filter(Boolean);

  segments.forEach((segment) => {
    const normalized = segment.toLowerCase();
    const number = parseInt(segment, 10);
    if (Number.isNaN(number)) return;
    if (normalized.startsWith('fly')) {
      speed.fly = number;
      if (normalized.includes('hover')) speed.canHover = true;
      return;
    }
    if (normalized.startsWith('swim')) {
      speed.swim = number;
      return;
    }
    if (normalized.startsWith('burrow')) {
      speed.burrow = number;
      return;
    }
    if (normalized.startsWith('climb')) {
      speed.climb = number;
      return;
    }
    speed.walk = number;
  });

  return Object.keys(speed).length ? speed : undefined;
}

function parseModifierList(value: unknown, keyMap?: Record<string, string>): Record<string, string> {
  if (!Array.isArray(value)) return {};
  const result: Record<string, string> = {};
  value.forEach((entry) => {
    if (!entry || typeof entry !== 'object' || Array.isArray(entry)) return;
    const entries = Object.entries(entry as Record<string, unknown>);
    if (entries.length !== 1) return;
    const [rawKey, rawValue] = entries[0];
    const key = keyMap?.[rawKey.toLowerCase()] ?? rawKey.toLowerCase();
    const numberValue = toNumber(rawValue);
    if (numberValue != null) {
      result[key] = numberValue >= 0 ? `+${numberValue}` : `${numberValue}`;
      return;
    }
    const text = getString(rawValue);
    if (text) result[key] = text;
  });
  return result;
}

function parseTraitsAsText(value: unknown): string | undefined {
  const entries = toEntryList(value);
  if (!entries?.length) return undefined;
  return entries.map((entry) => `**${entry.name}.** ${entry.description}`).join('\n\n');
}

function mapSizeToShort(value?: string): string | undefined {
  if (!value) return undefined;
  return SIZE_TO_SHORT[value.toLowerCase()] ?? undefined;
}

function mapSizeToLong(value?: string): string | undefined {
  if (!value) return undefined;
  return SHORT_TO_SIZE[value] ?? value;
}

function toTextList(value: unknown): string[] | undefined {
  if (Array.isArray(value)) {
    const list = value.map((entry) => getString(entry)).filter((entry): entry is string => Boolean(entry));
    return list.length ? list : undefined;
  }
  const raw = getString(value);
  if (!raw) return undefined;
  const parts = raw
    .split(',')
    .map((segment) => segment.trim())
    .filter(Boolean);
  return parts.length ? parts : undefined;
}

function toNumberArray(value: unknown): number[] | undefined {
  if (!Array.isArray(value)) return undefined;
  const numeric = value.map((entry) => toNumber(entry));
  if (numeric.some((entry) => entry == null)) return undefined;
  return numeric as number[];
}

function toNumber(value: unknown): number | undefined {
  if (typeof value === 'number' && Number.isFinite(value)) return value;
  if (typeof value === 'string' && value.trim()) {
    const parsed = Number(value);
    if (!Number.isNaN(parsed)) return parsed;
  }
  return undefined;
}

function getString(value: unknown): string | undefined {
  return typeof value === 'string' && value.trim() ? value.trim() : undefined;
}

function getScalarString(value: unknown): string | undefined {
  if (typeof value === 'number') return String(value);
  return getString(value);
}