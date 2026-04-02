import React, { useEffect, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { BestiaryClient, BestiaryListItem, CreatureListItem } from '../../api/bestiaryClient';
import { CustomCreatureForm } from '../../components/bestiaries/CustomCreatureForm';
import './EditBestiary.css';
import './EditBestiary.css';

const client = new BestiaryClient();

interface EditingCreature {
  id: string;
  name: string;
  creatureType?: string;
  challengeRating?: string;
  isLegendary: boolean;
  hp?: number;
  ac?: number;
  traits?: string;
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
      const hp = typeof rd.hp?.average === 'number' ? rd.hp.average : undefined;
      let ac: number | undefined;
      if (Array.isArray(rd.ac) && rd.ac.length > 0) {
        const first = rd.ac[0];
        ac = typeof first === 'number' ? first : (first as { ac?: number }).ac;
      }
      let traits: string | undefined;
      if (Array.isArray(rd.trait)) {
        const traitEntry = rd.trait.find(t => t.name === 'Traits');
        if (traitEntry && Array.isArray(traitEntry.entries) && traitEntry.entries.length > 0) {
          traits = typeof traitEntry.entries[0] === 'string' ? traitEntry.entries[0] : undefined;
        }
      }
      setEditingCreature({
        id: creature.id,
        name: creature.name,
        creatureType: creature.creatureType,
        challengeRating: creature.challengeRating,
        isLegendary: creature.isLegendary,
        hp,
        ac,
        traits,
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
