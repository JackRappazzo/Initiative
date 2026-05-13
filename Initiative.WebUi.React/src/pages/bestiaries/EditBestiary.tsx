import React, { useEffect, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { BestiaryClient, BestiaryListItem, CreatureListItem, CreatureSortBy, CustomCreatureAbilityScores, CustomCreatureEntry, CustomCreatureSpeed, CustomCreatureSpellcasting } from '../../api/bestiaryClient';
import { SortState } from '../../hooks/useBestiarySearch';
import CreatureBrowser from '../../components/bestiaries/CreatureBrowser';
import { CustomCreatureForm } from '../../components/bestiaries/CustomCreatureForm';
import { fantasyStatblockYamlToCustomPayload } from '../../utils/fantasyStatblockYaml';
import './EditBestiary.css';

const client = new BestiaryClient();
const PAGE_SIZE = 20;
const DEBOUNCE_MS = 300;

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

type NewCreatureSeed = Omit<EditingCreature, 'id'>;

const EditBestiary: React.FC = () => {
  const { bestiaryId } = useParams<{ bestiaryId: string }>();
  const navigate = useNavigate();

  const [bestiary, setBestiary] = useState<BestiaryListItem | null>(null);
  const [loadingBestiary, setLoadingBestiary] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [refreshToken, setRefreshToken] = useState(0);

  const [nameInput, setNameInput] = useState('');
  const [nameFilter, setNameFilter] = useState('');
  const [creatureTypeFilter, setCreatureTypeFilter] = useState('');
  const [sort, setSort] = useState<SortState>({ col: 'Name', desc: false });
  const [creatures, setCreatures] = useState<CreatureListItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [creaturesLoading, setCreaturesLoading] = useState(false);
  const [creaturesError, setCreaturesError] = useState<string | null>(null);

  const [renaming, setRenaming] = useState(false);
  const [renameValue, setRenameValue] = useState('');
  const [renameSaving, setRenameSaving] = useState(false);

  const [showForm, setShowForm] = useState(false);
  const [editingCreature, setEditingCreature] = useState<EditingCreature | undefined>(undefined);
  const [newCreatureSeed, setNewCreatureSeed] = useState<NewCreatureSeed | undefined>(undefined);

  const [showYamlImport, setShowYamlImport] = useState(false);
  const [yamlInput, setYamlInput] = useState('');
  const [yamlError, setYamlError] = useState<string | null>(null);

  const [confirmDelete, setConfirmDelete] = useState(false);
  const [deleting, setDeleting] = useState(false);

  const renameInputRef = useRef<HTMLInputElement>(null);
  const debounceTimer = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    if (!bestiaryId) return;
    const load = async () => {
      setLoadingBestiary(true);
      setError(null);
      try {
        const all = await client.getAvailableBestiaries();
        const found = all.find(b => b.id === bestiaryId) ?? null;
        if (!found || !found.ownerId) {
          setError('Bestiary not found or not editable.');
        } else {
          setBestiary(found);
          setRenameValue(found.name);
        }
      } catch {
        setError('Failed to load bestiary.');
      } finally {
        setLoadingBestiary(false);
      }
    };
    load();
  }, [bestiaryId]);

  useEffect(() => {
    if (debounceTimer.current) clearTimeout(debounceTimer.current);
    debounceTimer.current = setTimeout(() => {
      setNameFilter(nameInput.trim());
      setCurrentPage(1);
    }, DEBOUNCE_MS);

    return () => {
      if (debounceTimer.current) clearTimeout(debounceTimer.current);
    };
  }, [nameInput]);

  useEffect(() => {
    if (!bestiaryId || loadingBestiary || !bestiary) return;
    let cancelled = false;

    const loadCreatures = async () => {
      setCreaturesLoading(true);
      setCreaturesError(null);
      try {
        const skip = (currentPage - 1) * PAGE_SIZE;
        const result = await client.searchCreatures({
          bestiaryIds: [bestiaryId],
          name: nameFilter || undefined,
          creatureType: creatureTypeFilter || undefined,
          sortBy: sort.col,
          sortDescending: sort.desc ? true : undefined,
          pageSize: PAGE_SIZE,
          skip,
        });

        if (!cancelled) {
          setCreatures(result.creatures);
          setTotalCount(result.totalCount);
        }
      } catch {
        if (!cancelled) setCreaturesError('Failed to load creatures');
      } finally {
        if (!cancelled) setCreaturesLoading(false);
      }
    };

    loadCreatures();
    return () => {
      cancelled = true;
    };
  }, [bestiaryId, loadingBestiary, bestiary, nameFilter, creatureTypeFilter, sort, currentPage, refreshToken]);

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
    setRefreshToken(x => x + 1);
    setCurrentPage(1);
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

        // Parse headerEntries to recover description, DC, attack bonus, and freeform lines
        const statLineRe = /spell save DC|to spell attack/i;
        const rawHeaders: string[] = Array.isArray(sc.headerEntries)
          ? sc.headerEntries.filter((h: unknown) => typeof h === 'string')
          : [];
        const statLineIdx = rawHeaders.findIndex(h => statLineRe.test(h));
        const statLine = statLineIdx >= 0 ? rawHeaders[statLineIdx] : undefined;
        // Description is the first headerEntry if it is NOT the stat line
        const descCandidate = rawHeaders.length > 0 && rawHeaders[0] !== statLine ? rawHeaders[0] : undefined;
        // Freeform lines: headerEntries after the fixed description/stat entries
        const fixedCount = (descCandidate ? 1 : 0) + (statLine ? 1 : 0);
        const freeformLines = rawHeaders.slice(fixedCount).filter(h => h !== statLine);

        // Parse spellSaveDc and spellAttackBonus from the stat line
        let spellSaveDc: number | undefined;
        let spellAttackBonus: number | undefined;
        if (statLine) {
          const dcMatch = statLine.match(/spell save DC (\d+)/i);
          if (dcMatch) spellSaveDc = parseInt(dcMatch[1], 10);
          const atkMatch = statLine.match(/\+(\d+) to spell attack/i);
          if (atkMatch) spellAttackBonus = parseInt(atkMatch[1], 10);
        }

        const slotSpells: Record<string, string[]> = {};
        if (Array.isArray(sc.will)) slotSpells['0'] = sc.will;
        // Level 1–9 slot spells stored under sc.spells[level].spells
        if (sc.spells && typeof sc.spells === 'object') {
          for (const [lvl, lvlData] of Object.entries(sc.spells as Record<string, any>)) {
            if (Array.isArray(lvlData?.spells)) slotSpells[lvl] = lvlData.spells;
          }
        }
        const dailySpells: string[] = [];
        if (sc.daily) {
          for (const [key, spells] of Object.entries(sc.daily)) {
            const count = key.replace('e', '');
            if (Array.isArray(spells)) dailySpells.push(...spells.map(s => `${count}/day: ${s}`));
          }
        }

        // Detect freeform: no slot/daily data but extra headerEntries beyond description+stat
        const hasFreeform = freeformLines.length > 0 && !sc.will && !sc.spells && !sc.daily;

        spellcasting = {
          ability: sc.ability,
          spellSaveDc,
          spellAttackBonus,
          description: descCandidate,
          slotSpells: Object.keys(slotSpells).length ? slotSpells : undefined,
          dailySpells: dailySpells.length ? dailySpells : undefined,
          freeformText: hasFreeform ? freeformLines.join('\n') : undefined,
        };
      }

      setEditingCreature({
        id: creature.id,
        name: creature.name,
        size: Array.isArray(rd.size) ? rd.size[0] : undefined,
        creatureType: creature.creatureType,
        subtype:
          rd.type && typeof rd.type === 'object'
            ? rd.type.tags?.[0]
            : undefined,
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
      setRefreshToken(x => x + 1);
    } catch {
      setError('Failed to delete creature.');
    }
  };

  const handleNameInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setNameInput(e.target.value);
  };

  const handleCreatureTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setCreatureTypeFilter(e.target.value);
    setCurrentPage(1);
  };

  const handleSortClick = (col: CreatureSortBy) => {
    setSort(prev => prev.col === col ? { col, desc: !prev.desc } : { col, desc: false });
    setCurrentPage(1);
  };

  const sortIndicator = (col: CreatureSortBy) => {
    if (sort.col !== col) return '';
    return sort.desc ? ' ↓' : ' ↑';
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleOpenNewForm = () => {
    setEditingCreature(undefined);
    setNewCreatureSeed(undefined);
    setShowForm(true);
  };

  const handleCloseForm = () => {
    setShowForm(false);
    setEditingCreature(undefined);
    setNewCreatureSeed(undefined);
  };

  const closeYamlImport = () => {
    setShowYamlImport(false);
    setYamlError(null);
  };

  const parseYamlToForm = () => {
    try {
      const payload = fantasyStatblockYamlToCustomPayload(yamlInput);
      setEditingCreature(undefined);
      setNewCreatureSeed({
        name: payload.name,
        size: payload.size,
        creatureType: payload.creatureType,
        subtype: payload.subtype,
        alignment: payload.alignment,
        challengeRating: payload.challengeRating,
        isLegendary: payload.isLegendary,
        proficiencyBonus: payload.proficiencyBonus,
        hp: payload.hp,
        hitDice: payload.hitDice,
        ac: payload.ac,
        acNote: payload.acNote,
        abilityScores: payload.abilityScores,
        speed: payload.speed,
        savingThrows: payload.savingThrows,
        skills: payload.skills,
        damageResistances: payload.damageResistances,
        damageImmunities: payload.damageImmunities,
        damageVulnerabilities: payload.damageVulnerabilities,
        conditionImmunities: payload.conditionImmunities,
        senses: payload.senses,
        languages: payload.languages,
        traits: payload.traits,
        actions: payload.actions,
        bonusActions: payload.bonusActions,
        reactions: payload.reactions,
        legendaryActions: payload.legendaryActions,
        legendaryActionCount: payload.legendaryActionCount,
        spellcasting: payload.spellcasting,
      });
      setShowYamlImport(false);
      setYamlError(null);
      setShowForm(true);
    } catch (error) {
      const reason = error instanceof Error ? error.message : 'Unable to parse YAML.';
      setYamlError(reason);
    }
  };

  if (loadingBestiary) return <div className="edit-bestiary-page"><p>Loading…</p></div>;
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

      <CreatureBrowser
        creatures={creatures}
        totalCount={totalCount}
        totalPages={Math.max(1, Math.ceil(totalCount / PAGE_SIZE))}
        currentPage={currentPage}
        creaturesLoading={creaturesLoading}
        creaturesError={creaturesError}
        nameInput={nameInput}
        creatureTypeFilter={creatureTypeFilter}
        sort={sort}
        handleNameInputChange={handleNameInputChange}
        handleCreatureTypeChange={handleCreatureTypeChange}
        handleSortClick={handleSortClick}
        sortIndicator={sortIndicator}
        handlePageChange={handlePageChange}
        toolbarExtras={(
          <div className="edit-bestiary-browser-tools">
            <h1 className="bestiaries-title">Creatures</h1>
            <button type="button" onClick={handleOpenNewForm}>+ Add Creature</button>
            <button type="button" onClick={() => setShowYamlImport(true)}>+ Create from YAML</button>
          </div>
        )}
        firstColumn={{
          header: <span>Actions</span>,
          cell: (creature) => (
            <div className="edit-bestiary-actions-cell">
              <button
                type="button"
                onClick={(e) => {
                  e.stopPropagation();
                  handleEditCreature(creature);
                }}
              >
                Edit
              </button>
              <button
                type="button"
                className="danger-btn"
                onClick={(e) => {
                  e.stopPropagation();
                  handleDeleteCreature(creature);
                }}
              >
                Delete
              </button>
            </div>
          ),
        }}
      />

      {showForm && bestiaryId && (
        <CustomCreatureForm
          key={editingCreature?.id ?? `new-${newCreatureSeed?.name ?? 'blank'}`}
          bestiaryId={bestiaryId}
          existing={editingCreature}
          initial={newCreatureSeed}
          onSaved={handleCreatureSaved}
          onClose={handleCloseForm}
        />
      )}

      {showYamlImport && (
        <div className="modal-overlay" onClick={closeYamlImport}>
          <div className="modal-content yaml-import-modal" onClick={(event) => event.stopPropagation()}>
            <h3>Create Creature from Fantasy Statblock YAML</h3>
            <p className="yaml-import-hint">Paste Basic 5e YAML fields. The form opens prefilled for review before saving.</p>
            <textarea
              className="yaml-import-textarea"
              value={yamlInput}
              onChange={(event) => setYamlInput(event.target.value)}
              placeholder={"layout: Basic 5e Layout\nname: Goblin\nsize: Small\ntype: humanoid\nac: 15\nhp: 7\nstats: [8, 14, 10, 10, 8, 8]"}
              rows={14}
            />
            {yamlError && <p className="form-error">{yamlError}</p>}
            <div className="form-actions">
              <button type="button" onClick={closeYamlImport}>Cancel</button>
              <button type="button" onClick={parseYamlToForm} disabled={!yamlInput.trim()}>Prefill Form</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default EditBestiary;
