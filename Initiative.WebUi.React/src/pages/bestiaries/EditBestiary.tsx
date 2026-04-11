import React, { useEffect, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { BestiaryClient, BestiaryListItem, CreatureListItem, CustomCreatureAbilityScores, CustomCreatureEntry, CustomCreatureSpeed, CustomCreatureSpellcasting } from '../../api/bestiaryClient';
import { CustomCreatureForm } from '../../components/bestiaries/CustomCreatureForm';
import './EditBestiary.css';

const client = new BestiaryClient();

interface EditingCreature {
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
  abilityScores?: CustomCreatureAbilityScores;
  speed?: CustomCreatureSpeed;
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
}

const EditBestiary: React.FC = () => {
  const { bestiaryId } = useParams<{ bestiaryId: string }>();
  const navigate = useNavigate();

  const [bestiary, setBestiary] = useState<BestiaryListItem | null>(null);
  const [creatures, setCreatures] = useState<CreatureListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [renaming, setRenaming] = useState(false);
  const [renameValue, setRenameValue] = useState('');
  const [renameSaving, setRenameSaving] = useState(false);

  const [showForm, setShowForm] = useState(false);
  const [editingCreature, setEditingCreature] = useState<EditingCreature | undefined>(undefined);

  const [confirmDelete, setConfirmDelete] = useState(false);
  const [deleting, setDeleting] = useState(false);

  const renameInputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (!bestiaryId) return;
    const load = async () => {
      setLoading(true);
      setError(null);
      try {
        const [all, creatureResult] = await Promise.all([
          client.getAvailableBestiaries(),
          client.searchCreatures({ bestiaryIds: [bestiaryId], pageSize: 1000 }),
        ]);
        const found = all.find(b => b.id === bestiaryId) ?? null;
        if (!found || !found.ownerId) {
          setError('Bestiary not found or not editable.');
        } else {
          setBestiary(found);
          setRenameValue(found.name);
          setCreatures(creatureResult.creatures);
        }
      } catch {
        setError('Failed to load bestiary.');
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [bestiaryId]);

  useEffect(() => {
    if (renaming) renameInputRef.current?.focus();
  }, [renaming]);

  const handleRenameSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!bestiaryId || !renameValue.trim()) return;
    setRenameSaving(true);
    try {
      await client.renameBestiary(bestiaryId, renameValue.trim());
      setBestiary(b => b ? { ...b, name: renameValue.trim() } : b);
      setRenaming(false);
    } catch {
      setError('Failed to rename bestiary.');
    } finally {
      setRenameSaving(false);
    }
  };

  const handleDeleteBestiary = async () => {
    if (!bestiaryId) return;
    setDeleting(true);
    try {
      await client.deleteBestiary(bestiaryId);
      navigate('/bestiaries');
    } catch {
      setError('Failed to delete bestiary.');
      setDeleting(false);
      setConfirmDelete(false);
    }
  };

  const handleCreatureSaved = (id: string, name: string) => {
    setCreatures(prev => {
      const idx = prev.findIndex(c => c.id === id);
      if (idx >= 0) {
        const updated = [...prev];
        updated[idx] = { ...updated[idx], name };
        return updated;
      }
      return [...prev, { id, name, bestiaryId: bestiaryId!, isLegendary: false }];
    });
    setShowForm(false);
    setEditingCreature(undefined);
  };

  const handleEditCreature = async (creature: CreatureListItem) => {
    try {
      const detail = await client.getCreatureById(creature.id);
      const rd = detail.rawData;

      // HP
      const hp = typeof rd.hp?.average === 'number' ? rd.hp.average : undefined;
      const hitDice = rd.hp?.formula;

      // AC
      let ac: number | undefined;
      let acNote: string | undefined;
      if (Array.isArray(rd.ac) && rd.ac.length > 0) {
        const first = rd.ac[0];
        if (typeof first === 'number') {
          ac = first;
        } else {
          ac = (first as { ac?: number }).ac;
          const fromArr = (first as { from?: string[] }).from;
          acNote = fromArr?.[0];
        }
      }

      // Traits free-text
      let traits: string | undefined;
      if (Array.isArray(rd.trait)) {
        const traitEntry = rd.trait.find(t => t.name === 'Traits');
        if (traitEntry && Array.isArray(traitEntry.entries) && traitEntry.entries.length > 0) {
          traits = typeof traitEntry.entries[0] === 'string' ? traitEntry.entries[0] : undefined;
        }
      }

      // Ability scores
      const abilityScores: CustomCreatureAbilityScores | undefined =
        (rd.str || rd.dex || rd.con || rd.int || rd.wis || rd.cha)
          ? { str: rd.str, dex: rd.dex, con: rd.con, int: rd.int, wis: rd.wis, cha: rd.cha }
          : undefined;

      // Speed
      let speed: CustomCreatureSpeed | undefined;
      if (rd.speed) {
        const s = rd.speed;
        const flyVal = typeof s.fly === 'number' ? s.fly : (s.fly as { number?: number })?.number;
        const canHover = typeof s.fly === 'object' && (s.fly as { condition?: string })?.condition?.includes('hover');
        speed = { walk: s.walk, fly: flyVal, swim: s.swim, burrow: s.burrow, climb: s.climb, canHover: canHover || undefined };
      }

      // Actions helper: parse 5etools entry arrays
      const parseEntries = (arr?: { name?: string; entries?: (string | { entries?: string[] })[] }[]): CustomCreatureEntry[] | undefined => {
        if (!arr?.length) return undefined;
        return arr.map(e => ({
          name: e.name ?? '',
          description: typeof e.entries?.[0] === 'string' ? e.entries[0] : '',
        }));
      };

      // Spellcasting
      let spellcasting: CustomCreatureSpellcasting | undefined;
      if (Array.isArray(rd.spellcasting) && rd.spellcasting.length > 0) {
        const sc = rd.spellcasting[0];
        const slotSpells: Record<string, string[]> = {};
        if (Array.isArray(sc.will)) slotSpells['0'] = sc.will;
        const dailySpells: string[] = [];
        if (sc.daily) {
          for (const [key, spells] of Object.entries(sc.daily)) {
            const count = key.replace('e', '');
            if (Array.isArray(spells)) dailySpells.push(...spells.map(s => `${count}/day: ${s}`));
          }
        }
        spellcasting = {
          ability: sc.ability,
          slotSpells: Object.keys(slotSpells).length ? slotSpells : undefined,
          dailySpells: dailySpells.length ? dailySpells : undefined,
        };
      }

      setEditingCreature({
        id: creature.id,
        name: creature.name,
        size: Array.isArray(rd.size) ? rd.size[0] : undefined,
        creatureType: creature.creatureType,
        subtype: undefined,
        alignment: Array.isArray(rd.alignment) ? rd.alignment[0] : undefined,
        challengeRating: creature.challengeRating,
        isLegendary: creature.isLegendary,
        hp,
        hitDice,
        ac,
        acNote,
        abilityScores,
        speed,
        savingThrows: rd.save as Record<string, string> | undefined,
        skills: rd.skill as Record<string, string> | undefined,
        damageResistances: rd.resist?.map(r => typeof r === 'string' ? r : '') as string[] | undefined,
        damageImmunities: rd.immune?.map(r => typeof r === 'string' ? r : '') as string[] | undefined,
        damageVulnerabilities: rd.vulnerable?.map(r => typeof r === 'string' ? r : '') as string[] | undefined,
        conditionImmunities: rd.conditionImmune?.map(r => typeof r === 'string' ? r : '') as string[] | undefined,
        senses: rd.senses,
        languages: rd.languages,
        traits,
        actions: parseEntries(rd.action as any),
        bonusActions: parseEntries(rd.bonus as any),
        reactions: parseEntries(rd.reaction as any),
        legendaryActions: parseEntries(rd.legendary as any),
        spellcasting,
      });
      setShowForm(true);
    } catch {
      setError('Failed to load creature details.');
    }
  };

  const handleDeleteCreature = async (creature: CreatureListItem) => {
    if (!bestiaryId) return;
    if (!window.confirm(`Delete "${creature.name}"?`)) return;
    try {
      await client.deleteCustomCreature(bestiaryId, creature.id);
      setCreatures(prev => prev.filter(c => c.id !== creature.id));
    } catch {
      setError('Failed to delete creature.');
    }
  };

  const handleOpenNewForm = () => {
    setEditingCreature(undefined);
    setShowForm(true);
  };

  const handleCloseForm = () => {
    setShowForm(false);
    setEditingCreature(undefined);
  };

  if (loading) return <div className="edit-bestiary-page"><p>Loading…</p></div>;
  if (error && !bestiary) return <div className="edit-bestiary-page"><p className="error-message">{error}</p></div>;

  return (
    <div className="edit-bestiary-page">
      <div className="edit-bestiary-header">
        <button className="back-link" onClick={() => navigate('/bestiaries')}>&larr; Back to Bestiaries</button>

        {renaming ? (
          <form className="rename-form" onSubmit={handleRenameSubmit}>
            <input
              ref={renameInputRef}
              value={renameValue}
              onChange={e => setRenameValue(e.target.value)}
              required
              disabled={renameSaving}
            />
            <button type="submit" disabled={renameSaving || !renameValue.trim()}>Save</button>
            <button type="button" onClick={() => setRenaming(false)} disabled={renameSaving}>Cancel</button>
          </form>
        ) : (
          <div className="bestiary-title-row">
            <h1>{bestiary?.name}</h1>
            <button onClick={() => setRenaming(true)}>Rename</button>
            {confirmDelete ? (
              <>
                <span className="confirm-text">Are you sure? This will delete all creatures in this bestiary.</span>
                <button className="danger-btn" onClick={handleDeleteBestiary} disabled={deleting}>
                  {deleting ? 'Deleting…' : 'Yes, Delete'}
                </button>
                <button onClick={() => setConfirmDelete(false)} disabled={deleting}>Cancel</button>
              </>
            ) : (
              <button className="danger-btn" onClick={() => setConfirmDelete(true)}>Delete Bestiary</button>
            )}
          </div>
        )}
      </div>

      {error && <p className="error-message">{error}</p>}

      <div className="edit-bestiary-toolbar">
        <h2>Creatures ({creatures.length})</h2>
        <button onClick={handleOpenNewForm}>+ Add Creature</button>
      </div>

      <table className="creature-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Type</th>
            <th>CR</th>
            <th>Legendary</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {creatures.length === 0 ? (
            <tr><td colSpan={5} className="table-empty">No creatures yet. Add one above.</td></tr>
          ) : creatures.map(c => (
            <tr key={c.id}>
              <td>{c.name}</td>
              <td>{c.creatureType ?? '—'}</td>
              <td>{c.challengeRating ?? '—'}</td>
              <td>{c.isLegendary ? 'Yes' : '—'}</td>
              <td className="creature-actions">
                <button onClick={() => handleEditCreature(c)}>Edit</button>
                <button className="danger-btn" onClick={() => handleDeleteCreature(c)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {showForm && bestiaryId && (
        <CustomCreatureForm
          bestiaryId={bestiaryId}
          existing={editingCreature}
          onSaved={handleCreatureSaved}
          onClose={handleCloseForm}
        />
      )}
    </div>
  );
};

export default EditBestiary;
