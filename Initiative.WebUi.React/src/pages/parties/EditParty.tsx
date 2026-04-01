import React, { useEffect, useMemo, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { PartyClient, PartyMember } from '../../api/partyClient';
import './EditParty.css';

interface MemberRow extends PartyMember {
  id: number;
  editing: boolean;
}

let nextId = 1;

const EditParty: React.FC = () => {
  const { partyId } = useParams<{ partyId?: string }>();
  const isNew = !partyId;
  const navigate = useNavigate();
  const partyClient = useMemo(() => new PartyClient(), []);

  const [partyName, setPartyName] = useState('');
  const [members, setMembers] = useState<MemberRow[]>([]);
  const [loading, setLoading] = useState(!isNew);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Load existing party when editing
  useEffect(() => {
    if (isNew) return;
    (async () => {
      try {
        const party = await partyClient.getParty(partyId!);
        setPartyName(party.name);
        setMembers(
          party.members.map((m) => ({ ...m, id: nextId++, editing: false }))
        );
      } catch (err) {
        console.error('Error loading party:', err);
        setError('Failed to load party');
      } finally {
        setLoading(false);
      }
    })();
  }, [isNew, partyId, partyClient]);

  const addMember = () => {
    setMembers((prev) => [
      ...prev,
      { id: nextId++, name: '', level: 1, editing: true },
    ]);
  };

  const startEditing = (id: number) => {
    setMembers((prev) =>
      prev.map((m) => (m.id === id ? { ...m, editing: true } : m))
    );
  };

  const commitName = (id: number, name: string) => {
    setMembers((prev) =>
      prev.map((m) => (m.id === id ? { ...m, name, editing: false } : m))
    );
  };

  const setLevel = (id: number, level: number) => {
    setMembers((prev) =>
      prev.map((m) => (m.id === id ? { ...m, level } : m))
    );
  };

  const removeMember = (id: number) => {
    setMembers((prev) => prev.filter((m) => m.id !== id));
  };

  const handleSave = async () => {
    if (!partyName.trim()) {
      setError('Party name is required');
      return;
    }
    setSaving(true);
    setError(null);
    try {
      const payload = members.map(({ name, level }) => ({ name, level }));
      if (isNew) {
        await partyClient.createParty(partyName.trim(), payload);
      } else {
        // Delete the old party and recreate with updated data (no PUT endpoint yet)
        await partyClient.deleteParty(partyId!);
        await partyClient.createParty(partyName.trim(), payload);
      }
      navigate('/parties');
    } catch (err) {
      console.error('Error saving party:', err);
      setError('Failed to save party');
      setSaving(false);
    }
  };

  if (loading) return <div className="edit-party-container">Loading...</div>;

  return (
    <div className="edit-party-container">
      <h1>{isNew ? 'Create Party' : 'Edit Party'}</h1>

      {error && <div className="error-message">{error}</div>}

      <div className="party-name-field">
        <label htmlFor="party-name">Party Name</label>
        <input
          id="party-name"
          type="text"
          value={partyName}
          onChange={(e) => setPartyName(e.target.value)}
          placeholder="e.g. The Fellowship"
        />
      </div>

      <div className="members-section">
        <h2>Members</h2>
        <ul className="members-list">
          {members.map((member) => (
            <MemberRowItem
              key={member.id}
              member={member}
              onStartEditing={() => startEditing(member.id)}
              onCommitName={(name) => commitName(member.id, name)}
              onSetLevel={(level) => setLevel(member.id, level)}
              onRemove={() => removeMember(member.id)}
            />
          ))}
        </ul>
        <button className="btn-add-member" onClick={addMember}>
          + Add Member
        </button>
      </div>

      <div className="edit-party-actions">
        <button className="btn-primary" onClick={handleSave} disabled={saving}>
          {saving ? 'Saving…' : 'Save Party'}
        </button>
        <button className="btn-secondary" onClick={() => navigate('/parties')} disabled={saving}>
          Cancel
        </button>
      </div>
    </div>
  );
};

interface MemberRowItemProps {
  member: MemberRow;
  onStartEditing: () => void;
  onCommitName: (name: string) => void;
  onSetLevel: (level: number) => void;
  onRemove: () => void;
}

const MemberRowItem: React.FC<MemberRowItemProps> = ({
  member,
  onStartEditing,
  onCommitName,
  onSetLevel,
  onRemove,
}) => {
  const inputRef = useRef<HTMLInputElement>(null);
  const [draftName, setDraftName] = useState(member.name);

  useEffect(() => {
    if (member.editing && inputRef.current) {
      inputRef.current.focus();
    }
  }, [member.editing]);

  const commit = () => onCommitName(draftName);

  return (
    <li className="member-row">
      {member.editing ? (
        <input
          ref={inputRef}
          className="member-name-input"
          value={draftName}
          onChange={(e) => setDraftName(e.target.value)}
          onBlur={commit}
          onKeyDown={(e) => {
            if (e.key === 'Enter') commit();
            if (e.key === 'Escape') {
              setDraftName(member.name);
              onCommitName(member.name);
            }
          }}
          placeholder="Member name"
        />
      ) : (
        <span
          className={`member-name-display${!member.name ? ' placeholder' : ''}`}
          onClick={onStartEditing}
          title="Click to edit name"
        >
          {member.name || 'Click to set name…'}
        </span>
      )}
      <span className="member-level-label">Level</span>
      <input
        className="member-level-input"
        type="number"
        min={1}
        max={20}
        value={member.level}
        onChange={(e) => onSetLevel(Math.min(20, Math.max(1, Number(e.target.value))))}
      />
      <button className="btn-remove-member" onClick={onRemove} title="Remove member">
        ×
      </button>
    </li>
  );
};

export default EditParty;
