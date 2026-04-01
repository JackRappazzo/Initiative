import React, { useEffect, useState, useCallback, useMemo } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { PartyClient, Party } from '../../api/partyClient';
import './ListParties.css';

const ListParties: React.FC = () => {
  const navigate = useNavigate();
  const [parties, setParties] = useState<Party[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const partyClient = useMemo(() => new PartyClient(), []);

  const loadParties = useCallback(async () => {
    try {
      const data = await partyClient.getParties();
      setParties(data);
    } catch (err) {
      console.error('Error loading parties:', err);
      setError('Failed to load parties');
    } finally {
      setLoading(false);
    }
  }, [partyClient]);

  useEffect(() => {
    loadParties();
  }, [loadParties]);

  const handleDeleteParty = async (partyId: string, partyName: string) => {
    if (!window.confirm(`Are you sure you want to delete "${partyName}"? This action cannot be undone.`)) {
      return;
    }
    try {
      await partyClient.deleteParty(partyId);
      await loadParties();
    } catch (err) {
      console.error('Error deleting party:', err);
      setError('Failed to delete party');
    }
  };

  if (loading) return <div className="parties-container">Loading parties...</div>;

  return (
    <div className="parties-container">
      <div className="parties-header">
        <h1>My Parties</h1>
        <button onClick={() => navigate('/parties/new')} className="btn-primary">
          Create New Party
        </button>
      </div>

      {error && <div className="error-message">{error}</div>}

      <div className="parties-grid">
        {parties.length > 0 ? (
          parties.map((party) => (
            <div key={party.partyId} className="party-card">
              <div className="party-card-header">
                <h2 className="party-title">{party.name}</h2>
              </div>
              <div className="party-member-count">
                {party.members.length} member{party.members.length !== 1 ? 's' : ''}
              </div>
              {party.members.length > 0 && (
                <ul className="party-members-preview">
                  {party.members.slice(0, 4).map((m, i) => (
                    <li key={i}>
                      <span className="member-level-badge">{m.level}</span>
                      {m.name}
                    </li>
                  ))}
                  {party.members.length > 4 && (
                    <li className="party-overflow">+{party.members.length - 4} more…</li>
                  )}
                </ul>
              )}
              <div className="party-actions">
                <Link to={`/parties/${party.partyId}/edit`} className="btn-secondary">
                  Edit
                </Link>
                <button
                  onClick={() => handleDeleteParty(party.partyId, party.name)}
                  className="btn-danger"
                >
                  Delete
                </button>
              </div>
            </div>
          ))
        ) : (
          <p>No parties found. Create one to get started!</p>
        )}
      </div>
    </div>
  );
};

export default ListParties;
