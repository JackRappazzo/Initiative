import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  PartyClient,
  Sidekick,
  SidekickChoice,
  SidekickClass,
  SidekickHpRoll,
  SidekickSpellcasterKind,
  SidekickWarriorRole,
} from '../../../api/partyClient';
import { BestiaryClient, CreatureListItem, FiveEToolsRawData } from '../../../api/bestiaryClient';
import BestiaryPicker from '../../../components/bestiaries/BestiaryPicker';
import CreatureStatBlock from '../../../components/bestiaries/CreatureStatBlock';
import './SidekickWizard.css';

type Ability = 'str' | 'dex' | 'con' | 'int' | 'wis' | 'cha';

const ABILITIES: Ability[] = ['str', 'dex', 'con', 'int', 'wis', 'cha'];
const ABILITY_LABEL: Record<Ability, string> = {
  str: 'STR', dex: 'DEX', con: 'CON', int: 'INT', wis: 'WIS', cha: 'CHA',
};

const CLASSES: SidekickClass[] = ['Expert', 'Spellcaster', 'Warrior'];
const SPELLCASTER_KINDS: SidekickSpellcasterKind[] = ['Magician', 'Healer', 'Prodigy'];
const WARRIOR_ROLES: SidekickWarriorRole[] = ['Attacker', 'Defender'];
const ASI_LEVELS = [4, 8, 12, 16, 19];

const HIT_DIE_MAX: Record<SidekickClass, number> = { Expert: 8, Spellcaster: 6, Warrior: 10 };

const modifier = (score: number) => Math.floor((score - 10) / 2);

function apiToAbility(sidekick: Sidekick): Record<Ability, number> {
  return {
    str: sidekick.abilityScores.strength,
    dex: sidekick.abilityScores.dexterity,
    con: sidekick.abilityScores.constitution,
    int: sidekick.abilityScores.intelligence,
    wis: sidekick.abilityScores.wisdom,
    cha: sidekick.abilityScores.charisma,
  };
}

function baseScoresFromRawData(rawData: FiveEToolsRawData): Record<Ability, number> {
  return {
    str: rawData.str ?? 10,
    dex: rawData.dex ?? 10,
    con: rawData.con ?? 10,
    int: rawData.int ?? 10,
    wis: rawData.wis ?? 10,
    cha: rawData.cha ?? 10,
  };
}

function buildHpRolls(sidekickClass: SidekickClass, level: number, conMod: number, mode: 'fixed' | 'roll'): SidekickHpRoll[] {
  const max = HIT_DIE_MAX[sidekickClass];
  const average = Math.floor(max / 2) + 1;
  const rolls: SidekickHpRoll[] = [];
  for (let l = 1; l <= level; l++) {
    let dieValue: number;
    let isFixed: boolean;
    if (l === 1) {
      dieValue = max;
      isFixed = true;
    } else if (mode === 'fixed') {
      dieValue = average;
      isFixed = true;
    } else {
      dieValue = 1 + Math.floor(Math.random() * max);
      isFixed = false;
    }
    rolls.push({ level: l, isFixed, dieValue, constitutionModifier: conMod, result: dieValue + conMod });
  }
  return rolls;
}

function parseSpellList(raw: string): { cantrips: string[]; knownSpells: Record<number, string[]> } {
  const cantrips: string[] = [];
  const knownSpells: Record<number, string[]> = {};
  raw.split('\n').map((l) => l.trim()).filter(Boolean).forEach((line) => {
    const match = line.match(/^(\d+)\s+(.+)$/);
    if (!match) return;
    const level = Number(match[1]);
    const spells = match[2].split(',').map((s) => s.trim()).filter(Boolean);
    if (level === 0) cantrips.push(...spells);
    else knownSpells[level] = [...(knownSpells[level] ?? []), ...spells];
  });
  return { cantrips, knownSpells };
}

function spellsToText(cantrips: string[], knownSpells: Record<number, string[]>): string {
  const lines: string[] = [];
  if (cantrips.length) lines.push(`0 ${cantrips.join(', ')}`);
  for (const level of Object.keys(knownSpells).map(Number).sort((a, b) => a - b)) {
    if (knownSpells[level]?.length) lines.push(`${level} ${knownSpells[level].join(', ')}`);
  }
  return lines.join('\n');
}

const SidekickWizard: React.FC = () => {
  const { partyId, memberId } = useParams<{ partyId: string; memberId?: string }>();
  const isEditing = Boolean(memberId);
  const navigate = useNavigate();
  const partyClient = useMemo(() => new PartyClient(), []);
  const bestiaryClient = useMemo(() => new BestiaryClient(), []);

  const [loading, setLoading] = useState(isEditing);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const [showBasePicker, setShowBasePicker] = useState(false);
  const [baseCreatureId, setBaseCreatureId] = useState<string | null>(null);
  const [baseCreatureName, setBaseCreatureName] = useState('');
  const [previewRawData, setPreviewRawData] = useState<FiveEToolsRawData | null>(null);

  const [name, setName] = useState('');
  const [sidekickClass, setSidekickClass] = useState<SidekickClass>('Expert');
  const [spellcasterKind, setSpellcasterKind] = useState<SidekickSpellcasterKind>('Magician');
  const [warriorRole, setWarriorRole] = useState<SidekickWarriorRole>('Attacker');
  const [level, setLevel] = useState(1);
  const [hpMode, setHpMode] = useState<'fixed' | 'roll'>('fixed');
  const [hpRolls, setHpRolls] = useState<SidekickHpRoll[]>([]);
  const [baseScores, setBaseScores] = useState<Record<Ability, number>>({ str: 10, dex: 10, con: 10, int: 10, wis: 10, cha: 10 });
  const [asiSelections, setAsiSelections] = useState<Record<number, Ability>>({});
  const [spellText, setSpellText] = useState('');

  // Load existing sidekick when editing
  useEffect(() => {
    if (!isEditing || !partyId || !memberId) return;
    (async () => {
      try {
        const party = await partyClient.getParty(partyId);
        const member = party.members.find((m) => m.id === memberId);
        if (!member?.sidekick) throw new Error('Sidekick not found');
        const s = member.sidekick;
        setBaseCreatureId(s.baseCreatureId);
        setBaseCreatureName(s.baseCreatureName);
        setName(member.name);
        setSidekickClass(s.class);
        setLevel(s.level);
        setHpRolls(s.hpRolls ?? []);
        setHpMode((s.hpRolls ?? []).some((r) => !r.isFixed) ? 'roll' : 'fixed');
        setBaseScores(apiToAbility(s));
        setPreviewRawData(s.rawData);
        if (s.spellcasting) {
          setSpellcasterKind(s.spellcasting.kind);
          setSpellText(spellsToText(s.spellcasting.cantrips, s.spellcasting.knownSpells));
        }
        const asis: Record<number, Ability> = {};
        const roleChoice = s.choices.find((c) => c.type === 'WarriorRole');
        if (roleChoice?.value) setWarriorRole(roleChoice.value as SidekickWarriorRole);
        s.choices.forEach((c) => {
          if (c.type === 'AbilityScoreIncrease' && c.value) asis[c.level] = c.value as Ability;
        });
        setAsiSelections(asis);
      } catch (err) {
        console.error('Error loading sidekick:', err);
        setError('Failed to load sidekick');
      } finally {
        setLoading(false);
      }
    })();
  }, [isEditing, partyId, memberId, partyClient]);

  const chooseBaseCreature = useCallback(async (creature: CreatureListItem) => {
    setShowBasePicker(false);
    setBaseCreatureId(creature.id);
    setBaseCreatureName(creature.name);
    setName(creature.name);
    try {
      const detail = await bestiaryClient.getCreatureById(creature.id);
      setPreviewRawData(detail.rawData);
      const scores = baseScoresFromRawData(detail.rawData);
      setBaseScores(scores);
      setHpRolls(buildHpRolls(sidekickClass, level, modifier(scores.con), 'fixed'));
    } catch {
      setError('Failed to load base creature');
    }
  }, [bestiaryClient, sidekickClass, level]);

  // Compute final ability scores (base + ASI)
  const finalScores = useMemo(() => {
    const scores = { ...baseScores };
    Object.values(asiSelections).forEach((abil) => {
      scores[abil] = (scores[abil] ?? 0) + 2;
    });
    return scores;
  }, [baseScores, asiSelections]);

  const regenHp = useCallback((mode: 'fixed' | 'roll', lvl: number, cls: SidekickClass, scores: Record<Ability, number>) => {
    setHpRolls(buildHpRolls(cls, lvl, modifier(scores.con), mode));
  }, []);

  const applyHpMode = useCallback((mode: 'fixed' | 'roll') => {
    setHpMode(mode);
    regenHp(mode, level, sidekickClass, finalScores);
  }, [regenHp, level, sidekickClass, finalScores]);

  const handleLevelChange = useCallback((value: number) => {
    const lvl = Math.min(20, Math.max(1, value));
    setLevel(lvl);
    regenHp(hpMode, lvl, sidekickClass, finalScores);
  }, [regenHp, hpMode, sidekickClass, finalScores]);

  const handleClassChange = useCallback((cls: SidekickClass) => {
    setSidekickClass(cls);
    regenHp(hpMode, level, cls, finalScores);
  }, [regenHp, hpMode, level, finalScores]);

  const handleAsiChange = useCallback((lvl: number, ability: Ability) => {
    const next = { ...asiSelections, [lvl]: ability };
    setAsiSelections(next);
    const scores = { ...baseScores };
    Object.values(next).forEach((abil) => {
      scores[abil] = (scores[abil] ?? 0) + 2;
    });
    regenHp(hpMode, level, sidekickClass, scores);
  }, [regenHp, hpMode, level, sidekickClass, asiSelections, baseScores]);

  const maxHitPoints = useMemo(() => hpRolls.reduce((sum, r) => sum + r.result, 0), [hpRolls]);

  const buildChoices = useCallback((): SidekickChoice[] => {
    const choices: SidekickChoice[] = [];
    if (sidekickClass === 'Spellcaster') {
      choices.push({ level: 1, type: 'SpellcasterKind', value: spellcasterKind, values: [] });
    }
    if (sidekickClass === 'Warrior') {
      choices.push({ level: 1, type: 'WarriorRole', value: warriorRole, values: [] });
    }
    Object.entries(asiSelections).forEach(([lvl, abil]) => {
      choices.push({ level: Number(lvl), type: 'AbilityScoreIncrease', value: abil, values: [] });
    });
    return choices;
  }, [sidekickClass, spellcasterKind, warriorRole, asiSelections]);

  const handleSave = useCallback(async () => {
    if (!partyId || !baseCreatureId) return;
    setSaving(true);
    setError(null);
    try {
      const { cantrips, knownSpells } = sidekickClass === 'Spellcaster'
        ? parseSpellList(spellText)
        : { cantrips: [], knownSpells: {} as Record<number, string[]> };

      const payload = {
        name: name.trim() || baseCreatureName,
        baseCreatureId,
        baseCreatureName,
        class: sidekickClass,
        level,
        abilityScores: {
          strength: finalScores.str,
          dexterity: finalScores.dex,
          constitution: finalScores.con,
          intelligence: finalScores.int,
          wisdom: finalScores.wis,
          charisma: finalScores.cha,
        },
        hpRolls,
        spellcasterKind: sidekickClass === 'Spellcaster' ? spellcasterKind : undefined,
        warriorRole: sidekickClass === 'Warrior' ? warriorRole : undefined,
        cantrips,
        knownSpells,
        choices: buildChoices(),
      };

      if (isEditing && memberId) {
        await partyClient.updateSidekick(partyId, memberId, payload);
      } else {
        await partyClient.createSidekick(partyId, payload);
      }
      navigate(`/parties/${partyId}/edit`);
    } catch (err) {
      console.error('Error saving sidekick:', err);
      setError('Failed to save sidekick');
      setSaving(false);
    }
  }, [partyId, memberId, isEditing, baseCreatureId, baseCreatureName, name, sidekickClass, level, hpRolls, spellText, spellcasterKind, warriorRole, finalScores, buildChoices, partyClient, navigate]);

  if (loading) {
    return <div className="sidekick-wizard">Loading sidekick...</div>;
  }

  const conMod = modifier(finalScores.con);

  return (
    <div className="sidekick-wizard">
      <h1>{isEditing ? 'Edit Sidekick' : 'Create Sidekick'}</h1>
      {error && <div className="error-message">{error}</div>}

      {!baseCreatureId ? (
        <div className="sidekick-wizard-start">
          <p>Choose a base creature with a challenge rating of 1/2 or lower to begin.</p>
          <button className="btn-primary" onClick={() => setShowBasePicker(true)}>Choose Base Creature</button>
        </div>
      ) : (
        <div className="sidekick-wizard-body">
          <div className="sidekick-wizard-form">
            <div className="sidekick-field">
              <label htmlFor="sk-name">Name</label>
              <input id="sk-name" type="text" value={name} onChange={(e) => setName(e.target.value)} />
            </div>

            <div className="sidekick-field">
              <label htmlFor="sk-class">Class</label>
              <select id="sk-class" value={sidekickClass} onChange={(e) => handleClassChange(e.target.value as SidekickClass)}>
                {CLASSES.map((c) => <option key={c} value={c}>{c}</option>)}
              </select>
            </div>

            {sidekickClass === 'Spellcaster' && (
              <div className="sidekick-field">
                <label htmlFor="sk-kind">Spellcaster Kind</label>
                <select id="sk-kind" value={spellcasterKind} onChange={(e) => setSpellcasterKind(e.target.value as SidekickSpellcasterKind)}>
                  {SPELLCASTER_KINDS.map((k) => <option key={k} value={k}>{k}</option>)}
                </select>
              </div>
            )}

            {sidekickClass === 'Warrior' && (
              <div className="sidekick-field">
                <label htmlFor="sk-role">Martial Role</label>
                <select id="sk-role" value={warriorRole} onChange={(e) => setWarriorRole(e.target.value as SidekickWarriorRole)}>
                  {WARRIOR_ROLES.map((r) => <option key={r} value={r}>{r}</option>)}
                </select>
              </div>
            )}

            <div className="sidekick-field">
              <label htmlFor="sk-level">Level</label>
              <input
                id="sk-level"
                type="number"
                min={1}
                max={20}
                value={level}
                onChange={(e) => handleLevelChange(Number(e.target.value))}
              />
            </div>

            <div className="sidekick-field sidekick-hp-row">
              <span>Hit Points (Con mod {conMod >= 0 ? `+${conMod}` : conMod})</span>
              <div className="sidekick-hp-actions">
                <button
                  className={hpMode === 'fixed' ? 'btn-primary' : 'btn-secondary'}
                  onClick={() => applyHpMode('fixed')}
                >
                  Fixed (average)
                </button>
                <button
                  className={hpMode === 'roll' ? 'btn-primary' : 'btn-secondary'}
                  onClick={() => applyHpMode('roll')}
                >
                  Roll
                </button>
              </div>
              <span className="sidekick-hp-total">Max HP: {maxHitPoints}</span>
            </div>

            {hpMode === 'roll' && (
              <div className="sidekick-hp-rolls">
                {hpRolls.map((r) => (
                  <span key={r.level} className="sidekick-hp-roll">
                    L{r.level}: {r.dieValue}+{r.constitutionModifier} = {r.result}
                  </span>
                ))}
              </div>
            )}

            <div className="sidekick-asi-section">
              <h3>Ability Score Increases</h3>
              {ASI_LEVELS.filter((l) => l <= level).map((l) => (
                <div key={l} className="sidekick-field">
                  <label htmlFor={`asi-${l}`}>Level {l} (+2 to)</label>
                  <select
                    id={`asi-${l}`}
                    value={asiSelections[l] ?? 'str'}
                    onChange={(e) => handleAsiChange(l, e.target.value as Ability)}
                  >
                    {ABILITIES.map((a) => <option key={a} value={a}>{ABILITY_LABEL[a]}</option>)}
                  </select>
                </div>
              ))}
            </div>

            {sidekickClass === 'Spellcaster' && (
              <div className="sidekick-field">
                <label htmlFor="sk-spells">Spells (one per line: &quot;level spell1, spell2&quot;)</label>
                <textarea
                  id="sk-spells"
                  rows={6}
                  value={spellText}
                  onChange={(e) => setSpellText(e.target.value)}
                  placeholder={'0 fire bolt, light\n1 magic missile, shield\n2 misty step'}
                />
              </div>
            )}

            <div className="sidekick-wizard-actions">
              <button className="btn-primary" onClick={handleSave} disabled={saving || !name.trim()}>
                {saving ? 'Saving…' : 'Save Sidekick'}
              </button>
              <button className="btn-secondary" onClick={() => navigate(`/parties/${partyId}/edit`)} disabled={saving}>
                Cancel
              </button>
            </div>
          </div>

          <div className="sidekick-wizard-preview">
            <h3>{isEditing ? 'Current Stat Block' : 'Base Creature'}</h3>
            {previewRawData ? <CreatureStatBlock data={previewRawData} /> : <div>No stat block available</div>}
          </div>
        </div>
      )}

      {showBasePicker && (
        <BestiaryPicker
          title="Choose Base Creature (CR 1/2 or lower)"
          selectButtonLabel="Choose"
          selectButtonTitle="Use as base creature"
          maxChallengeRating="1/2"
          nonLegendaryOnly
          onCreatureSelect={chooseBaseCreature}
          onClose={() => setShowBasePicker(false)}
        />
      )}
    </div>
  );
};

export default SidekickWizard;
