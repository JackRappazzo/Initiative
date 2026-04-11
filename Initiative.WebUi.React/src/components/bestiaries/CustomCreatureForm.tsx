import './CustomCreatureForm.css';
import React, { useCallback, useEffect, useRef, useState } from 'react';
import {
  BestiaryClient,
  CustomCreatureEntry,
  CustomCreaturePayload,
  CustomCreatureSpellcasting,
} from '../../api/bestiaryClient';
import { SpellClient, SpellListItem } from '../../api/spellClient';

// ── Types ─────────────────────────────────────────────────────────────────────

interface Props {
  bestiaryId: string;
  existing?: {
    id: string;
    name: string;
    size?: string;
    creatureType?: string;
    subtype?: string;
    alignment?: string;
    challengeRating?: string;
    isLegendary: boolean;
    proficiencyBonus?: number;
    hp?: number;
    hitDice?: string;
    ac?: number;
    acNote?: string;
    abilityScores?: { str?: number; dex?: number; con?: number; int?: number; wis?: number; cha?: number };
    speed?: { walk?: number; fly?: number; swim?: number; burrow?: number; climb?: number; canHover?: boolean };
    savingThrows?: Record<string, string>;
    skills?: Record<string, string>;
    damageResistances?: string[];
    damageImmunities?: string[];
    damageVulnerabilities?: string[];
    conditionImmunities?: string[];
    senses?: string[];
    languages?: string[];
    traits?: string;
    actions?: CustomCreatureEntry[];
    bonusActions?: CustomCreatureEntry[];
    reactions?: CustomCreatureEntry[];
    legendaryActions?: CustomCreatureEntry[];
    legendaryActionCount?: number;
    spellcasting?: CustomCreatureSpellcasting;
  };
  initial?: Omit<NonNullable<Props['existing']>, 'id'>;
  onSaved: (id: string, name: string) => void;
  onClose: () => void;
}

// ── Helpers ───────────────────────────────────────────────────────────────────

const client = new BestiaryClient();

function numStr(n: number | undefined) { return n != null ? String(n) : ''; }
function parseNum(s: string) { const n = parseInt(s, 10); return isNaN(n) ? undefined : n; }
function listToStr(arr: string[] | undefined) { return arr?.join('\n') ?? ''; }
function strToList(s: string) { return s.split('\n').map(l => l.trim()).filter(Boolean); }

const ABILITIES = ['str', 'dex', 'con', 'int', 'wis', 'cha'] as const;
const SAVES     = ['str', 'dex', 'con', 'int', 'wis', 'cha'] as const;
const SKILLS    = ['Acrobatics','Animal Handling','Arcana','Athletics','Deception',
                   'History','Insight','Intimidation','Investigation','Medicine',
                   'Nature','Perception','Performance','Persuasion','Religion',
                   'Sleight of Hand','Stealth','Survival'] as const;
const DMG_TYPES = ['acid','bludgeoning','cold','fire','force','lightning','necrotic',
                   'piercing','poison','psychic','radiant','slashing','thunder'] as const;
const CONDITIONS = ['Blinded','Charmed','Deafened','Exhaustion','Frightened','Grappled',
                    'Incapacitated','Invisible','Paralyzed','Petrified','Poisoned',
                    'Prone','Restrained','Stunned','Unconscious'] as const;
const SIZES = ['T','S','M','L','H','G'] as const;
const SIZE_LABELS: Record<string, string> = { T:'Tiny', S:'Small', M:'Medium', L:'Large', H:'Huge', G:'Gargantuan' };
const CASTING_ABILITIES = ['str','dex','con','int','wis','cha'] as const;

const MD_HINT = <span className="ccf-md-hint"><strong>**bold**</strong> <em>*italic*</em></span>;

// ── SpellTagTextarea ──────────────────────────────────────────────────────────

const spellClient = new SpellClient();

interface SpellTagTextareaProps {
  value: string;
  onChange: (value: string) => void;
  rows?: number;
  placeholder?: string;
}

const SpellTagTextarea: React.FC<SpellTagTextareaProps> = ({ value, onChange, rows = 4, placeholder }) => {
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const [suggestions, setSuggestions] = useState<SpellListItem[]>([]);
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  // Parse the partial spell name from {@spell prefix at the cursor position
  const getSpellPrefix = (text: string, cursor: number): string | null => {
    const before = text.slice(0, cursor);
    const match = before.match(/\{@spell ([^}]*)$/);
    return match ? match[1] : null;
  };

  const handleKeyUp = useCallback(() => {
    const el = textareaRef.current;
    if (!el) return;
    const prefix = getSpellPrefix(el.value, el.selectionStart);
    if (prefix === null) {
      setDropdownOpen(false);
      return;
    }
    if (debounceRef.current) clearTimeout(debounceRef.current);
    debounceRef.current = setTimeout(async () => {
      if (prefix.trim().length === 0) {
        setSuggestions([]);
        setDropdownOpen(false);
        return;
      }
      try {
        const results = await spellClient.searchSpells(prefix.trim(), 8);
        setSuggestions(results);
        setDropdownOpen(results.length > 0);
      } catch {
        setSuggestions([]);
        setDropdownOpen(false);
      }
    }, 200);
  }, []);

  const selectSuggestion = (spell: SpellListItem) => {
    const el = textareaRef.current;
    if (!el) return;
    const cursor = el.selectionStart;
    const before = el.value.slice(0, cursor);
    const after = el.value.slice(cursor);
    const matchStart = before.search(/\{@spell [^}]*$/);
    if (matchStart === -1) return;
    const newBefore = before.slice(0, matchStart) + `{@spell ${spell.name}}`;
    onChange(newBefore + after);
    setDropdownOpen(false);
    // Restore focus and position cursor after the inserted tag
    setTimeout(() => {
      el.focus();
      el.selectionStart = el.selectionEnd = newBefore.length;
    }, 0);
  };

  useEffect(() => {
    return () => { if (debounceRef.current) clearTimeout(debounceRef.current); };
  }, []);

  return (
    <div className="ccf-spell-tag-wrap">
      <textarea
        ref={textareaRef}
        value={value}
        onChange={e => onChange(e.target.value)}
        onKeyUp={handleKeyUp}
        rows={rows}
        placeholder={placeholder}
        className="ccf-spell-tag-textarea"
      />
      {dropdownOpen && (
        <ul className="ccf-spell-suggestions" role="listbox">
          {suggestions.map(s => (
            <li
              key={s.id}
              role="option"
              aria-selected={false}
              onMouseDown={e => { e.preventDefault(); selectSuggestion(s); }}
              className="ccf-spell-suggestion-item"
            >
              {s.name}
              {s.source && <span className="ccf-spell-suggestion-source"> ({s.source})</span>}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

// Collapsible section wrapper
const Section: React.FC<{ title: string; children: React.ReactNode; defaultOpen?: boolean }> = ({ title, children, defaultOpen = false }) => {
  const [open, setOpen] = useState(defaultOpen);
  return (
    <div className="ccf-section">
      <button type="button" className="ccf-section-legend" onClick={() => setOpen(o => !o)}>
        <span className="ccf-toggle">{open ? '▾' : '▸'}</span> {title}
      </button>
      {open && <div className="ccf-section-body">{children}</div>}
    </div>
  );
};

// Editable list of name+description entries (actions, reactions, etc.)
const EntryList: React.FC<{
  entries: CustomCreatureEntry[];
  onChange: (entries: CustomCreatureEntry[]) => void;
  placeholder?: string;
}> = ({ entries, onChange, placeholder }) => {
  const add = () => onChange([...entries, { name: '', description: '' }]);
  const update = (i: number, field: keyof CustomCreatureEntry, val: string) => {
    const next = entries.map((e, idx) => idx === i ? { ...e, [field]: val } : e);
    onChange(next);
  };
  const remove = (i: number) => onChange(entries.filter((_, idx) => idx !== i));

  return (
    <div className="ccf-entry-list">
      {entries.map((e, i) => (
        <div key={i} className="ccf-entry">
          <input
            className="ccf-entry-name"
            value={e.name}
            onChange={ev => update(i, 'name', ev.target.value)}
            placeholder="Name"
          />
          <div className="ccf-entry-desc-wrap">
            <textarea
              className="ccf-entry-desc"
              value={e.description}
              onChange={ev => update(i, 'description', ev.target.value)}
              rows={2}
              placeholder={placeholder ?? 'Description… **bold** *italic*'}
            />
            {MD_HINT}
          </div>
          <button type="button" className="ccf-entry-remove" onClick={() => remove(i)} title="Remove">✕</button>
        </div>
      ))}
      <button type="button" className="ccf-add-btn" onClick={add}>+ Add</button>
    </div>
  );
};

// Multi-select chip list (damage types, conditions)
const ChipSelect: React.FC<{
  options: readonly string[];
  selected: string[];
  onChange: (v: string[]) => void;
}> = ({ options, selected, onChange }) => {
  const toggle = (v: string) => onChange(
    selected.includes(v) ? selected.filter(s => s !== v) : [...selected, v]
  );
  return (
    <div className="ccf-chip-group">
      {options.map(o => (
        <button
          key={o}
          type="button"
          className={`ccf-chip ${selected.includes(o) ? 'ccf-chip--on' : ''}`}
          onClick={() => toggle(o)}
        >{o}</button>
      ))}
    </div>
  );
};

// ── Main form ─────────────────────────────────────────────────────────────────

export const CustomCreatureForm: React.FC<Props> = ({ bestiaryId, existing, initial, onSaved, onClose }) => {
  const ex = existing;
  const seed = existing ?? initial;

  // ── Identity
  const [name, setName]                   = useState(seed?.name ?? '');
  const [size, setSize]                   = useState(seed?.size ?? '');
  const [creatureType, setCreatureType]   = useState(seed?.creatureType ?? '');
  const [subtype, setSubtype]             = useState(seed?.subtype ?? '');
  const [alignment, setAlignment]         = useState(seed?.alignment ?? '');
  const [cr, setCr]                       = useState(seed?.challengeRating ?? '');
  const [isLegendary, setIsLegendary]     = useState(seed?.isLegendary ?? false);
  const [profBonus, setProfBonus]         = useState(numStr(seed?.proficiencyBonus));

  // ── Vitals
  const [hp, setHp]         = useState(numStr(seed?.hp));
  const [hitDice, setHitDice] = useState(seed?.hitDice ?? '');
  const [ac, setAc]         = useState(numStr(seed?.ac));
  const [acNote, setAcNote] = useState(seed?.acNote ?? '');

  // ── Ability scores
  const [scores, setScores] = useState<Record<string, string>>({
    str: numStr(seed?.abilityScores?.str), dex: numStr(seed?.abilityScores?.dex),
    con: numStr(seed?.abilityScores?.con), int: numStr(seed?.abilityScores?.int),
    wis: numStr(seed?.abilityScores?.wis), cha: numStr(seed?.abilityScores?.cha),
  });

  // ── Speed
  const [walk, setWalk]       = useState(numStr(seed?.speed?.walk));
  const [fly, setFly]         = useState(numStr(seed?.speed?.fly));
  const [canHover, setCanHover] = useState(seed?.speed?.canHover ?? false);
  const [swim, setSwim]       = useState(numStr(seed?.speed?.swim));
  const [burrow, setBurrow]   = useState(numStr(seed?.speed?.burrow));
  const [climb, setClimb]     = useState(numStr(seed?.speed?.climb));

  // ── Saves & skills (stored as "ability → modifier string" maps)
  const [saves, setSaves]   = useState<Record<string, string>>(seed?.savingThrows ?? {});
  const [skills, setSkills] = useState<Record<string, string>>(seed?.skills ?? {});

  // ── Defenses
  const [resist, setResist]     = useState<string[]>(seed?.damageResistances ?? []);
  const [immune, setImmune]     = useState<string[]>(seed?.damageImmunities ?? []);
  const [vuln, setVuln]         = useState<string[]>(seed?.damageVulnerabilities ?? []);
  const [condImm, setCondImm]   = useState<string[]>(seed?.conditionImmunities ?? []);

  // ── Senses / languages
  const [senses, setSenses]       = useState(listToStr(seed?.senses));
  const [languages, setLanguages] = useState(listToStr(seed?.languages));

  // ── Traits (free text)
  const [traits, setTraits] = useState(seed?.traits ?? '');

  // ── Action lists
  const [actions, setActions]         = useState<CustomCreatureEntry[]>(seed?.actions ?? []);
  const [bonusActions, setBonusActions] = useState<CustomCreatureEntry[]>(seed?.bonusActions ?? []);
  const [reactions, setReactions]     = useState<CustomCreatureEntry[]>(seed?.reactions ?? []);
  const [legendary, setLegendary]     = useState<CustomCreatureEntry[]>(seed?.legendaryActions ?? []);
  const [legCount, setLegCount]       = useState(numStr(seed?.legendaryActionCount));

  // ── Spellcasting
  const [scAbility, setScAbility]     = useState(seed?.spellcasting?.ability ?? '');
  const [scDc, setScDc]               = useState(numStr(seed?.spellcasting?.spellSaveDc));
  const [scAtk, setScAtk]             = useState(numStr(seed?.spellcasting?.spellAttackBonus));
  const [scCantrips, setScCantrips]   = useState(seed?.spellcasting?.slotSpells?.['0']?.join('\n') ?? '');
  const [scSlots, setScSlots]         = useState<Record<string, string>>(() => {
    const slots: Record<string, string> = {};
    const ss = seed?.spellcasting?.slotSpells ?? {};
    for (let l = 1; l <= 9; l++) slots[l] = ss[l]?.join('\n') ?? '';
    return slots;
  });
  const [scDaily, setScDaily]         = useState(seed?.spellcasting?.dailySpells?.join('\n') ?? '');
  const [scDescription, setScDescription] = useState(seed?.spellcasting?.description ?? '');
  const [scFreeform, setScFreeform]       = useState(seed?.spellcasting?.freeformText ?? '');
  const [scFormat, setScFormat]           = useState<'slot' | 'day' | 'freeform'>(
    seed?.spellcasting?.freeformText ? 'freeform' : seed?.spellcasting?.dailySpells?.length ? 'day' : 'slot'
  );

  // ── Submit
  const [saving, setSaving] = useState(false);
  const [error, setError]   = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) return;

    // Build spellcasting block only if any sc data provided
    const hasSlots  = scCantrips.trim() || Object.values(scSlots).some(v => v.trim());
    const hasDaily  = scDaily.trim();
    const hasFreeform = scFreeform.trim();
    let spellcasting: CustomCreatureSpellcasting | undefined;
    if (scAbility || hasSlots || hasDaily || hasFreeform || scDescription.trim()) {
      let slotSpells: Record<string, string[]> | undefined;
      let dailySpells: string[] | undefined;
      let freeformText: string | undefined;
      if (scFormat === 'freeform') {
        freeformText = hasFreeform ? scFreeform : undefined;
      } else if (scFormat === 'day') {
        dailySpells = hasDaily ? strToList(scDaily) : undefined;
      } else {
        const ss: Record<string, string[]> = {};
        if (scCantrips.trim()) ss['0'] = strToList(scCantrips);
        for (let l = 1; l <= 9; l++) {
          if (scSlots[l]?.trim()) ss[String(l)] = strToList(scSlots[l]);
        }
        slotSpells = Object.keys(ss).length ? ss : undefined;
      }
      spellcasting = {
        ability:          scAbility || undefined,
        spellSaveDc:      parseNum(scDc),
        spellAttackBonus: parseNum(scAtk),
        description:      scDescription.trim() || undefined,
        slotSpells,
        dailySpells,
        freeformText,
      };
    }

    const anyScore = ABILITIES.some(a => scores[a] !== '');
    const anySpeed = [walk, fly, swim, burrow, climb].some(v => v !== '');

    const payload: CustomCreaturePayload = {
      name: name.trim(),
      size:              size || undefined,
      creatureType:      creatureType.trim() || undefined,
      subtype:           subtype.trim() || undefined,
      alignment:         alignment || undefined,
      challengeRating:   cr.trim() || undefined,
      isLegendary,
      proficiencyBonus:  parseNum(profBonus),
      hp:                parseNum(hp),
      hitDice:           hitDice.trim() || undefined,
      ac:                parseNum(ac),
      acNote:            acNote.trim() || undefined,
      abilityScores: anyScore ? {
        str: parseNum(scores.str), dex: parseNum(scores.dex), con: parseNum(scores.con),
        int: parseNum(scores.int), wis: parseNum(scores.wis), cha: parseNum(scores.cha),
      } : undefined,
      speed: anySpeed || canHover ? {
        walk: parseNum(walk), fly: parseNum(fly), swim: parseNum(swim),
        burrow: parseNum(burrow), climb: parseNum(climb), canHover: canHover || undefined,
      } : undefined,
      savingThrows:         Object.keys(saves).length ? saves : undefined,
      skills:               Object.keys(skills).length ? skills : undefined,
      damageResistances:    resist.length ? resist : undefined,
      damageImmunities:     immune.length ? immune : undefined,
      damageVulnerabilities: vuln.length ? vuln : undefined,
      conditionImmunities:  condImm.length ? condImm : undefined,
      senses:    senses.trim() ? strToList(senses) : undefined,
      languages: languages.trim() ? strToList(languages) : undefined,
      traits:    traits.trim() || undefined,
      actions:       actions.length ? actions : undefined,
      bonusActions:  bonusActions.length ? bonusActions : undefined,
      reactions:     reactions.length ? reactions : undefined,
      legendaryActions: legendary.length ? legendary : undefined,
      legendaryActionCount: parseNum(legCount),
      spellcasting,
    };

    setSaving(true);
    setError(null);
    try {
      if (ex) {
        await client.updateCustomCreature(bestiaryId, ex.id, payload);
        onSaved(ex.id, payload.name);
      } else {
        const result = await client.createCustomCreature(bestiaryId, payload);
        onSaved(result.id, result.name);
      }
    } catch {
      setError('Failed to save creature. Please try again.');
      setSaving(false);
    }
  };

  // ── Saves/skills row helpers
  const saveToggle = (ab: string) => setSaves(prev => {
    if (ab in prev) { const n = { ...prev }; delete n[ab]; return n; }
    return { ...prev, [ab]: '+0' };
  });
  const skillToggle = (sk: string) => setSkills(prev => {
    const key = sk.toLowerCase();
    if (key in prev) { const n = { ...prev }; delete n[key]; return n; }
    return { ...prev, [key]: '+0' };
  });

  // ── Render
  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content ccf-modal" onClick={e => e.stopPropagation()}>
        <h3>{ex ? 'Edit Creature' : 'New Creature'}</h3>
        <form onSubmit={handleSubmit}>

          {/* ── Identity ──────────────────────────────────────────────── */}
          <Section title="Identity" defaultOpen>
            <label>
              Name <span className="ccf-required">*</span>
              <input value={name} onChange={e => setName(e.target.value)} required autoFocus />
            </label>
            <div className="ccf-row">
              <label>
                Size
                <select value={size} onChange={e => setSize(e.target.value)}>
                  <option value="">—</option>
                  {SIZES.map(s => <option key={s} value={s}>{SIZE_LABELS[s]}</option>)}
                </select>
              </label>
              <label>
                Type
                <input value={creatureType} onChange={e => setCreatureType(e.target.value)} placeholder="e.g. humanoid" />
              </label>
              <label>
                Subtype
                <input value={subtype} onChange={e => setSubtype(e.target.value)} placeholder="e.g. shapechanger" />
              </label>
            </div>
            <div className="ccf-row">
              <label>
                Alignment
                <input value={alignment} onChange={e => setAlignment(e.target.value)} placeholder="e.g. chaotic evil" />
              </label>
              <label>
                CR
                <input value={cr} onChange={e => setCr(e.target.value)} placeholder="e.g. 1/2" />
              </label>
              <label>
                Proficiency Bonus
                <input type="number" value={profBonus} onChange={e => setProfBonus(e.target.value)} placeholder="e.g. 3" />
              </label>
            </div>
            <label className="checkbox-label">
              <input type="checkbox" checked={isLegendary} onChange={e => setIsLegendary(e.target.checked)} />
              Legendary creature
            </label>
          </Section>

          {/* ── Vitals ────────────────────────────────────────────────── */}
          <Section title="Vitals (HP & AC)">
            <div className="ccf-row">
              <label>
                HP (flat)
                <input type="number" min={0} value={hp} onChange={e => setHp(e.target.value)} placeholder="e.g. 45" />
              </label>
              <label>
                Hit Dice
                <input value={hitDice} onChange={e => setHitDice(e.target.value)} placeholder="e.g. 8d8+16" />
              </label>
            </div>
            <div className="ccf-row">
              <label>
                AC
                <input type="number" min={0} value={ac} onChange={e => setAc(e.target.value)} placeholder="e.g. 15" />
              </label>
              <label>
                AC source note
                <input value={acNote} onChange={e => setAcNote(e.target.value)} placeholder="e.g. natural armour" />
              </label>
            </div>
          </Section>

          {/* ── Ability Scores ────────────────────────────────────────── */}
          <Section title="Ability Scores">
            <div className="ccf-ability-grid">
              {ABILITIES.map(ab => (
                <label key={ab}>
                  {ab.toUpperCase()}
                  <input
                    type="number" min={1} max={30}
                    value={scores[ab]}
                    onChange={e => setScores(prev => ({ ...prev, [ab]: e.target.value }))}
                    placeholder="—"
                  />
                </label>
              ))}
            </div>
          </Section>

          {/* ── Speed ─────────────────────────────────────────────────── */}
          <Section title="Speed">
            <div className="ccf-row ccf-speed-row">
              <label>Walk <input type="number" min={0} value={walk} onChange={e => setWalk(e.target.value)} placeholder="30" /></label>
              <label>Fly  <input type="number" min={0} value={fly}  onChange={e => setFly(e.target.value)}  placeholder="—"  /></label>
              <label className="checkbox-label">
                <input type="checkbox" checked={canHover} onChange={e => setCanHover(e.target.checked)} /> Hover
              </label>
              <label>Swim   <input type="number" min={0} value={swim}   onChange={e => setSwim(e.target.value)}   placeholder="—" /></label>
              <label>Burrow <input type="number" min={0} value={burrow} onChange={e => setBurrow(e.target.value)} placeholder="—" /></label>
              <label>Climb  <input type="number" min={0} value={climb}  onChange={e => setClimb(e.target.value)}  placeholder="—" /></label>
            </div>
          </Section>

          {/* ── Saves & Skills ────────────────────────────────────────── */}
          <Section title="Saving Throws &amp; Skills">
            <p className="ccf-hint">Click an ability or skill to add a proficiency override, then edit the modifier.</p>
            <div className="ccf-save-row">
              {SAVES.map(ab => (
                <label key={ab} className={`ccf-save-chip ${ab in saves ? 'ccf-save-chip--on' : ''}`}>
                  <input type="checkbox" checked={ab in saves} onChange={() => saveToggle(ab)} />
                  {ab.toUpperCase()}
                  {ab in saves && (
                    <input
                      className="ccf-mod-input"
                      value={saves[ab]}
                      onChange={e => setSaves(prev => ({ ...prev, [ab]: e.target.value }))}
                    />
                  )}
                </label>
              ))}
            </div>
            <div className="ccf-skill-grid">
              {SKILLS.map(sk => {
                const key = sk.toLowerCase();
                return (
                  <label key={sk} className={`ccf-save-chip ${key in skills ? 'ccf-save-chip--on' : ''}`}>
                    <input type="checkbox" checked={key in skills} onChange={() => skillToggle(sk)} />
                    {sk}
                    {key in skills && (
                      <input
                        className="ccf-mod-input"
                        value={skills[key]}
                        onChange={e => setSkills(prev => ({ ...prev, [key]: e.target.value }))}
                      />
                    )}
                  </label>
                );
              })}
            </div>
          </Section>

          {/* ── Defenses ─────────────────────────────────────────────── */}
          <Section title="Damage &amp; Condition Modifiers">
            <p className="ccf-hint">Resistances</p>
            <ChipSelect options={DMG_TYPES} selected={resist} onChange={setResist} />
            <p className="ccf-hint">Immunities</p>
            <ChipSelect options={DMG_TYPES} selected={immune} onChange={setImmune} />
            <p className="ccf-hint">Vulnerabilities</p>
            <ChipSelect options={DMG_TYPES} selected={vuln} onChange={setVuln} />
            <p className="ccf-hint">Condition immunities</p>
            <ChipSelect options={CONDITIONS} selected={condImm} onChange={setCondImm} />
          </Section>

          {/* ── Senses & Languages ───────────────────────────────────── */}
          <Section title="Senses &amp; Languages">
            <label>
              Senses <span className="ccf-hint-inline">(one per line)</span>
              <textarea value={senses} onChange={e => setSenses(e.target.value)} rows={2} placeholder="darkvision 60 ft." />
            </label>
            <label>
              Languages <span className="ccf-hint-inline">(one per line)</span>
              <textarea value={languages} onChange={e => setLanguages(e.target.value)} rows={2} placeholder="Common, Draconic" />
            </label>
          </Section>

          {/* ── Traits ───────────────────────────────────────────────── */}
          <Section title="Traits">
            <label>
              <textarea
                value={traits}
                onChange={e => setTraits(e.target.value)}
                rows={4}
                placeholder="Free-text traits for this creature… **bold** *italic*"
              />
              {MD_HINT}
            </label>
          </Section>

          {/* ── Actions ──────────────────────────────────────────────── */}
          <Section title="Actions">
            <EntryList entries={actions} onChange={setActions} />
          </Section>

          <Section title="Bonus Actions">
            <EntryList entries={bonusActions} onChange={setBonusActions} />
          </Section>

          <Section title="Reactions">
            <EntryList entries={reactions} onChange={setReactions} />
          </Section>

          {/* ── Legendary ────────────────────────────────────────────── */}
          <Section title="Legendary Actions">
            <label>
              Actions per round
              <input type="number" min={1} value={legCount} onChange={e => setLegCount(e.target.value)} placeholder="3" />
            </label>
            <EntryList entries={legendary} onChange={setLegendary} placeholder="Legendary action description… **bold** *italic*" />
          </Section>

          {/* ── Spellcasting ─────────────────────────────────────────── */}
          <Section title="Spellcasting">
            <div className="ccf-row">
              <label>
                Casting ability
                <select value={scAbility} onChange={e => setScAbility(e.target.value)}>
                  <option value="">—</option>
                  {CASTING_ABILITIES.map(a => <option key={a} value={a}>{a.toUpperCase()}</option>)}
                </select>
              </label>
              <label>
                Spell save DC
                <input type="number" value={scDc} onChange={e => setScDc(e.target.value)} placeholder="—" />
              </label>
              <label>
                Spell attack bonus
                <input type="number" value={scAtk} onChange={e => setScAtk(e.target.value)} placeholder="—" />
              </label>
            </div>

            <label>
              Description <span className="ccf-md-hint">(optional intro text)</span>
              <textarea
                value={scDescription}
                onChange={e => setScDescription(e.target.value)}
                rows={2}
                placeholder="The dragon is an 8th-level spellcaster. Its spellcasting ability is Charisma…"
              />
            </label>

            <div className="ccf-sc-format-wrap">
              <span className="ccf-sc-format-label">Spell list format</span>
              <div className="ccf-sc-format-group" role="group" aria-label="Spell list format">
                <button
                  type="button"
                  className={`ccf-sc-format-btn ${scFormat === 'slot' ? 'ccf-sc-format-btn--active' : ''}`}
                  onClick={() => setScFormat('slot')}
                >
                  Spell slots
                </button>
                <button
                  type="button"
                  className={`ccf-sc-format-btn ${scFormat === 'day' ? 'ccf-sc-format-btn--active' : ''}`}
                  onClick={() => setScFormat('day')}
                >
                  Per day
                </button>
                <button
                  type="button"
                  className={`ccf-sc-format-btn ${scFormat === 'freeform' ? 'ccf-sc-format-btn--active' : ''}`}
                  onClick={() => setScFormat('freeform')}
                >
                  Freeform
                </button>
              </div>
            </div>

            {scFormat === 'slot' && (
              <>
                <p className="ccf-hint">One spell name per line. Use <code>{'{@spell Name}'}</code> for linked spells.</p>
                <label>
                  Cantrips / At-will
                  <textarea value={scCantrips} onChange={e => setScCantrips(e.target.value)} rows={2} placeholder="Prestidigitation" />
                </label>
                {[1,2,3,4,5,6,7,8,9].map(lvl => (
                  <label key={lvl}>
                    {lvl}{lvl === 1 ? 'st' : lvl === 2 ? 'nd' : lvl === 3 ? 'rd' : 'th'}-level slots
                    <textarea
                      value={scSlots[lvl]}
                      onChange={e => setScSlots(prev => ({ ...prev, [lvl]: e.target.value }))}
                      rows={2}
                      placeholder="Fireball"
                    />
                  </label>
                ))}
              </>
            )}

            {scFormat === 'day' && (
              <>
                <p className="ccf-hint">One entry per line, e.g. <code>3/day: Fireball</code> or <code>1/day each: Misty Step, Fly</code></p>
                <textarea
                  value={scDaily}
                  onChange={e => setScDaily(e.target.value)}
                  rows={4}
                  placeholder={"3/day: Fireball\n1/day each: Misty Step, Fly"}
                />
              </>
            )}

            {scFormat === 'freeform' && (
              <>
                <p className="ccf-hint">Free text. Type <code>{'{@spell '}</code> to autocomplete a spell name.</p>
                <SpellTagTextarea
                  value={scFreeform}
                  onChange={setScFreeform}
                  rows={6}
                  placeholder={"At will: {@spell Prestidigitation}, {@spell Minor Illusion}\n3/day each: {@spell Fireball}, {@spell Fly}\n1/day: {@spell Wish}"}
                />
              </>
            )}
          </Section>

          {error && <p className="form-error">{error}</p>}
          <div className="form-actions">
            <button type="button" onClick={onClose} disabled={saving}>Cancel</button>
            <button type="submit" disabled={saving || !name.trim()}>
              {saving ? 'Saving…' : 'Save'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
