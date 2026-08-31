import React, { useEffect, useMemo, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { PartyClient, PartyMember } from '../../api/partyClient';
import { DndBeyondClient, DndBeyondCharacter, DndBeyondCharacterDetail } from '../../api/dndBeyondClient';
import { useDndBeyondSession } from '../../contexts/DndBeyondContext';
import './EditParty.css';

interface MemberRow extends PartyMember {
  id: number;
  editing: boolean;
  dndBeyondLabel?: string;
}

let nextId = 1;

const buildCharacterLabel = (detail: DndBeyondCharacterDetail): string => {
  const parts: string[] = [];
  if (detail.className) parts.push(detail.className);
  parts.push(`Level ${detail.level}`);
  return `${detail.name ?? 'Character'} (${parts.join(', ')})`;
};

const EditParty: React.FC = () => {
  const { partyId } = useParams<{ partyId?: string }>();
  const isNew = !partyId;
  const navigate = useNavigate();
  const partyClient = useMemo(() => new PartyClient(), []);
  const dndBeyondClient = useMemo(() => new DndBeyondClient(), []);
  const { hasToken } = useDndBeyondSession();

  const [partyName, setPartyName] = useState('');
  const [members, setMembers] = useState<MemberRow[]>([]);
  const [loading, setLoading] = useState(!isNew);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [pickerMemberId, setPickerMemberId] = useState<number | null>(null);
  const [pickerCharacters, setPickerCharacters] = useState<DndBeyondCharacter[]>([]);
  const [pickerLoading, setPickerLoading] = useState(false);
  const [pickerError, setPickerError] = useState<string | null>(null);

  const [confirmingMemberId, setConfirmingMemberId] = useState<number | null>(null);
  const [confirmDetail, setConfirmDetail] = useState<DndBeyondCharacterDetail | null>(null);
  const [confirmLoading, setConfirmLoading] = useState(false);
  const [confirmError, setConfirmError] = useState<string | null>(null);

  useEffect(() => {
    if (isNew) return;
    (async () => {
      try {
        const party = await partyClient.getParty(partyId!);
        setPartyName(party.name);
        const rows = party.members.map((m) => ({ ...m, id: nextId++, editing: false }));
        setMembers(rows);

        for (const row of rows) {
          if (!row.dndBeyondCharacterId) continue;
          dndBeyondClient.getCharacter(row.dndBeyondCharacterId)
            .then((detail) => {
              setMembers((prev) => prev.map((r) =>
                r.id === row.id ? { ...r, dndBeyondLabel: buildCharacterLabel(detail) } : r
              ));
            })
            .catch(() => {
              setMembers((prev) => prev.map((r) =>
                r.id === row.id ? { ...r, dndBeyondLabel: `DDB #${row.dndBeyondCharacterId}` } : r
              ));
            });
        }
      } catch (err) {
        console.error('Error loading party:', err);
        setError('Failed to load party');
      } finally {
        setLoading(false);
      }
    })();
  }, [isNew, partyId, partyClient, dndBeyondClient]);

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

  const openCharacterPicker = async (memberId: number) => {
    if (!hasToken) {
      setError('No D&D Beyond token set. Add one in Settings first.');
      return;
    }
    setError(null);
    setPickerMemberId(memberId);
    setPickerLoading(true);
    setPickerError(null);
    try {
      const characters = await dndBeyondClient.getCharacters();
      setPickerCharacters(characters);
    } catch (err) {
      console.error('Error loading D&D Beyond characters:', err);
      setPickerError('Failed to load D&D Beyond characters');
      setPickerCharacters([]);
    } finally {
      setPickerLoading(false);
    }
  };

  const closePicker = () => {
    setPickerMemberId(null);
    setPickerCharacters([]);
    setPickerError(null);
  };

  const selectCharacter = async (character: DndBeyondCharacter) => {
    const memberId = pickerMemberId;
    closePicker();
    if (memberId === null) return;

    setConfirmingMemberId(memberId);
    setConfirmLoading(true);
    setConfirmError(null);
    setConfirmDetail(null);
    try {
      const detail = await dndBeyondClient.getCharacter(character.id);
      setConfirmDetail(detail);
    } catch (err) {
      console.error('Error loading character details:', err);
      setConfirmError('Failed to load character details');
    } finally {
      setConfirmLoading(false);
    }
  };

  const confirmLink = () => {
    if (confirmingMemberId === null || !confirmDetail) return;
    const memberId = confirmingMemberId;
    const characterId = String(confirmDetail.id);
    setMembers((prev) =>
      prev.map((m) =>
        m.id === memberId
          ? { ...m, dndBeyondCharacterId: characterId, dndBeyondLabel: buildCharacterLabel(confirmDetail) }
          : m
      )
    );
    setConfirmingMemberId(null);
    setConfirmDetail(null);
    setConfirmError(null);
  };

  const cancelConfirm = () => {
    setConfirmingMemberId(null);
    setConfirmDetail(null);
    setConfirmError(null);
  };

  const unlink = (memberId: number) => {
    setMembers((prev) =>
      prev.map((m) =>
        m.id === memberId ? { ...m, dndBeyondCharacterId: undefined, dndBeyondLabel: undefined } : m
      )
    );
  };

  const handleSave = async () => {
    if (!partyName.trim()) {
      setError('Party name is required');
      return;
    }
    setSaving(true);
    setError(null);
    try {
      const payload = members.map(({ name, level, dndBeyondCharacterId }) => ({ name, level, dndBeyondCharacterId }));
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
            <React.Fragment key={member.id}>
              <MemberRowItem
                member={member}
                onStartEditing={() => startEditing(member.id)}
                onCommitName={(name) => commitName(member.id, name)}
                onSetLevel={(level) => setLevel(member.id, level)}
                onRemove={() => removeMember(member.id)}
                onLinkCharacter={() => openCharacterPicker(member.id)}
                onUnlink={() => unlink(member.id)}
              />
              {confirmingMemberId === member.id && (
                <li className="member-confirm-row">
                  {confirmLoading && <span>Loading character…</span>}
                  {confirmError && <span className="confirm-error">{confirmError}</span>}
                  {!confirmLoading && !confirmError && confirmDetail && (
                    <>
                      <span className="member-confirm-text">
                        Link &ldquo;{member.name || 'this member'}&rdquo; to {buildCharacterLabel(confirmDetail)}?
                      </span>
                      <button className="btn-primary" onClick={confirmLink}>Confirm</button>
                      <button className="btn-secondary" onClick={cancelConfirm}>Cancel</button>
                    </>
                  )}
                </li>
              )}
            </React.Fragment>
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

      {pickerMemberId !== null && (
        <div className="ddb-picker-overlay" onClick={closePicker}>
          <div className="ddb-picker-modal" onClick={(e) => e.stopPropagation()}>
            <div className="ddb-picker-header">
              <h2>Choose D&D Beyond Character</h2>
              <button className="ddb-picker-close" onClick={closePicker} aria-label="Close">×</button>
            </div>

            {pickerLoading && <div className="ddb-picker-body">Loading characters...</div>}
            {pickerError && <div className="ddb-picker-body ddb-picker-error">{pickerError}</div>}

            {!pickerLoading && !pickerError && (
              <ul className="ddb-picker-list">
                {pickerCharacters.length === 0 && (
                  <li className="ddb-picker-empty">No characters found.</li>
                )}
                {pickerCharacters.map((character) => (
                  <li
                    key={character.id}
                    className="ddb-picker-item"
                    onClick={() => selectCharacter(character)}
                  >
                    {character.name ?? `Character ${character.id}`}
                  </li>
                ))}
              </ul>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

interface MemberRowItemProps {
  member: MemberRow;
  onStartEditing: () => void;
  onCommitName: (name: string) => void;
  onSetLevel: (level: number) => void;
  onRemove: () => void;
  onLinkCharacter: () => void;
  onUnlink: () => void;
}

const MemberRowItem: React.FC<MemberRowItemProps> = ({
  member,
  onStartEditing,
  onCommitName,
  onSetLevel,
  onRemove,
  onLinkCharacter,
  onUnlink,
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
      {member.dndBeyondCharacterId ? (
        <span className="member-ddb-link" title={member.dndBeyondLabel ?? `DDB #${member.dndBeyondCharacterId}`}>
          <span className="member-ddb-badge">
            {member.dndBeyondLabel ?? `DDB #${member.dndBeyondCharacterId}`}
          </span>
          <button className="btn-unlink" onClick={onUnlink} title="Unlink from D&D Beyond">×</button>
        </span>
      ) : (
        <button className="btn-link-ddb" onClick={onLinkCharacter} title="Link a D&D Beyond character">
          Link D&D Beyond
        </button>
      )}
      <button className="btn-remove-member" onClick={onRemove} title="Remove member">
        ×
      </button>
    </li>
  );
};

export default EditParty;
