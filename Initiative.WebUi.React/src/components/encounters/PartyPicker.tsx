import React, { useEffect, useMemo, useState } from 'react';
import ReactDOM from 'react-dom';
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
  const [tooltip, setTooltip] = useState<{ text: string; x: number; y: number } | null>(null);

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
    <>
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
            {parties.map((party) => {
              const count = party.members.length;
              const avgLevel = count > 0
                ? Math.round(party.members.reduce((sum, m) => sum + m.level, 0) / count)
                : null;
              const tooltipText = party.members.map(m => `Lv${m.level} ${m.name}`).join('\n');
              return (
                <li key={party.partyId} className="party-picker-item" onClick={() => onPartySelect(party)}>
                  <div className="party-picker-item-name">{party.name}</div>
                  <div
                    className="party-picker-item-summary"
                    onMouseEnter={(e) => {
                      if (count === 0) return;
                      const rect = (e.currentTarget as HTMLElement).closest('li')!.getBoundingClientRect();
                      setTooltip({ text: tooltipText, x: rect.left + 16, y: rect.bottom + 4 });
                    }}
                    onMouseLeave={() => setTooltip(null)}
                  >
                    {count === 0
                      ? <span className="party-picker-no-members">No members</span>
                      : <>{count} member{count !== 1 ? 's' : ''} &middot; avg level {avgLevel}</>}
                  </div>
                </li>
              );
            })}
          </ul>
        )}
      </div>
    </div>

    {tooltip && ReactDOM.createPortal(
      <div
        className="party-picker-tooltip"
        style={{ top: tooltip.y - 8, left: tooltip.x }}
      >
        {tooltip.text}
      </div>,
      document.body
    )}
  </>
  );
};

export default PartyPicker;
