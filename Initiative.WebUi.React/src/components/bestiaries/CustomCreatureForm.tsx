import React, { useState } from 'react';
import { BestiaryClient, CustomCreaturePayload } from '../../api/bestiaryClient';

interface Props {
  bestiaryId: string;
  existing?: {
    id: string;
    name: string;
    creatureType?: string;
    challengeRating?: string;
    isLegendary: boolean;
    hp?: number;
    ac?: number;
    traits?: string;
  };
  onSaved: (id: string, name: string) => void;
  onClose: () => void;
}

const client = new BestiaryClient();

export const CustomCreatureForm: React.FC<Props> = ({ bestiaryId, existing, onSaved, onClose }) => {
  const [name, setName] = useState(existing?.name ?? '');
  const [creatureType, setCreatureType] = useState(existing?.creatureType ?? '');
  const [challengeRating, setChallengeRating] = useState(existing?.challengeRating ?? '');
  const [isLegendary, setIsLegendary] = useState(existing?.isLegendary ?? false);
  const [hp, setHp] = useState<string>(existing?.hp != null ? String(existing.hp) : '');
  const [ac, setAc] = useState<string>(existing?.ac != null ? String(existing.ac) : '');
  const [traits, setTraits] = useState(existing?.traits ?? '');
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) return;

    const payload: CustomCreaturePayload = {
      name: name.trim(),
      creatureType: creatureType.trim() || undefined,
      challengeRating: challengeRating.trim() || undefined,
      isLegendary,
      hp: hp !== '' ? parseInt(hp, 10) : undefined,
      ac: ac !== '' ? parseInt(ac, 10) : undefined,
      traits: traits.trim() || undefined,
    };

    setSaving(true);
    setError(null);
    try {
      if (existing) {
        await client.updateCustomCreature(bestiaryId, existing.id, payload);
        onSaved(existing.id, payload.name);
      } else {
        const result = await client.createCustomCreature(bestiaryId, payload);
        onSaved(result.id, result.name);
      }
    } catch {
      setError('Failed to save creature. Please try again.');
      setSaving(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={e => e.stopPropagation()}>
        <h3>{existing ? 'Edit Creature' : 'New Creature'}</h3>
        <form onSubmit={handleSubmit}>
          <label>
            Name <span style={{ color: 'red' }}>*</span>
            <input value={name} onChange={e => setName(e.target.value)} required autoFocus />
          </label>
          <label>
            Type
            <input value={creatureType} onChange={e => setCreatureType(e.target.value)} placeholder="e.g. humanoid" />
          </label>
          <label>
            Challenge Rating
            <input value={challengeRating} onChange={e => setChallengeRating(e.target.value)} placeholder="e.g. 1/2" />
          </label>
          <label>
            HP
            <input type="number" min={0} value={hp} onChange={e => setHp(e.target.value)} placeholder="e.g. 45" />
          </label>
          <label>
            AC
            <input type="number" min={0} value={ac} onChange={e => setAc(e.target.value)} placeholder="e.g. 15" />
          </label>
          <label className="checkbox-label">
            <input type="checkbox" checked={isLegendary} onChange={e => setIsLegendary(e.target.checked)} />
            Legendary
          </label>
          <label>
            Traits
            <textarea
              value={traits}
              onChange={e => setTraits(e.target.value)}
              rows={4}
              placeholder="Free-text traits for this creature..."
            />
          </label>
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
