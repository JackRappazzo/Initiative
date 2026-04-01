import React, { useEffect, useMemo, useState } from 'react';
import { Party, PartyClient } from '../../api/partyClient';
import './PartyPicker.css';

interface PartyPickerProps {
  onPartySelect: (party: Party) => void;
  onClose: () => void;
}

const PartyPicker: React.FC<PartyPickerProps> = ({ onPartySelect, onClose }) => {
  const partyClient = useMemo(() => new PartyClient(), []);
  const [parties, setParties] = useState<Party[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    (async () => {
      try {
        const data = await partyClient.getParties();
        setParties(data);
      } catch (err) {
        console.error('Error loading parties:', err);
        setError('Failed to load parties');
      } finally {
        setLoading(false);
      }
    })();
  }, [partyClient]);

  return (
    <div className="party-picker-overlay" onClick={onClose}>
      <div className="party-picker-modal" onClick={(e) => e.stopPropagation()}>
        <div className="party-picker-header">
          <h2>Choose Party</h2>
          <button className="party-picker-close" onClick={onClose} aria-label="Close">×</button>
        </div>

        {loading && <div className="party-picker-body">Loading parties...</div>}
        {error && <div className="party-picker-body party-picker-error">{error}</div>}

        {!loading && !error && (
          <ul className="party-picker-list">
            {parties.length === 0 && (
              <li className="party-picker-empty">No parties found. Create one first.</li>
            )}
            {parties.map((party) => (
              <li key={party.partyId} className="party-picker-item" onClick={() => onPartySelect(party)}>
                <div className="party-picker-item-name">{party.name}</div>
                <div className="party-picker-item-members">
                  {party.members.map((m, i) => (
                    <span key={i} className="party-picker-member">
                      <span className="party-picker-member-level">{m.level}</span>
                      {m.name}
                    </span>
                  ))}
                  {party.members.length === 0 && <span className="party-picker-no-members">No members</span>}
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
};

export default PartyPicker;
