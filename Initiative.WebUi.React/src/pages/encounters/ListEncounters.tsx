import React, { useEffect, useState, useCallback, useMemo } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { EncounterClient, EncounterListItem } from '../../api/encounterClient';
import './ListEncounters.css';

const ListEncounters: React.FC = () => {
  const navigate = useNavigate();
  const [encounters, setEncounters] = useState<EncounterListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const encounterClient = useMemo(() => new EncounterClient(), []);

  const loadEncounters = useCallback(async () => {
    try {
      const data = await encounterClient.getEncounterList();
      console.log('Loaded encounters: ', data)
      setEncounters(data);
    } catch (err) {
      console.error('Error loading encounters:', err);
      setError('Failed to load encounters');
    } finally {
      setLoading(false);
    }
  }, [encounterClient]);

  useEffect(() => {
    loadEncounters();
  }, [loadEncounters]);

  const handleCreateEncounter = async () => {
    try {
      const result = await encounterClient.createEncounter("New Encounter");
      navigate(`/encounters/${result.encounterId}`);
    } catch (err) {
      console.error('Error creating encounter:', err);
      setError('Failed to create encounter');
    }
  };

  const handleDeleteEncounter = async (encounterId: string, encounterName: string) => {
    if (!window.confirm(`Are you sure you want to delete "${encounterName}"? This action cannot be undone.`)) {
      return;
    }

    try {
      await encounterClient.deleteEncounter(encounterId);
      // Reload the encounters list after successful deletion
      await loadEncounters();
    } catch (err) {
      console.error('Error deleting encounter:', err);
      setError('Failed to delete encounter');
    }
  };

  if (loading) 
    return <div className="encounters-container">Loading encounters...</div>;
  if (error) 
    return <div className="error-message">{error}</div>;

  return (
    <div className="encounters-container">
      <div className="encounters-header">
        <h1>My Encounters</h1>
        <button onClick={handleCreateEncounter} className="btn-primary">
          Create New Encounter
        </button>
      </div>
      <div className="encounters-grid">
        {encounters.length > 0 ? (
          encounters.map((encounter) => (
            <div key={encounter.encounterId} className="encounter-card">
              <div className="encounter-header">
                <h2 className="encounter-title">{encounter.encounterName}</h2>
                <div className="encounter-date">
                  {new Date(encounter.createdAt).toLocaleDateString()}
                </div>
              </div>
              <div className="encounter-stats">
                Creatures: {encounter.numberOfCreatures}
              </div>
              <div className="encounter-actions">
                <Link to={`/encounters/${encounter.encounterId}`} className="btn-secondary">
                  View Details
                </Link>
                <button 
                  onClick={() => handleDeleteEncounter(encounter.encounterId, encounter.encounterName)}
                  className="btn-danger"
                >
                  Delete
                </button>
              </div>
            </div>
          ))
        ) : (
          <p>No encounters found. Create one to get started!</p>
        )}
      </div>
    </div>
  );
};

export default ListEncounters;
