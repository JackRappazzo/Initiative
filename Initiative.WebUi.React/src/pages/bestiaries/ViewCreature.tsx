import React, { useEffect, useState, useMemo } from 'react';
import { Link, useParams, useLocation } from 'react-router-dom';
import { BestiaryClient, CreatureDetail } from '../../api/bestiaryClient';
import CreatureStatBlock from '../../components/bestiaries/CreatureStatBlock';
import './ViewCreature.css';

interface LocationState {
  bestiaryId?: string;
  bestiaryName?: string;
}

const ViewCreature: React.FC = () => {
  const { creatureId } = useParams<{ creatureId: string }>();
  const location = useLocation();
  const state = location.state as LocationState | null;

  const [creature, setCreature] = useState<CreatureDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const bestiaryClient = useMemo(() => new BestiaryClient(), []);

  useEffect(() => {
    if (!creatureId) return;
    setLoading(true);
    setError(null);
    bestiaryClient.getCreatureById(creatureId)
      .then(setCreature)
      .catch(() => setError('Failed to load creature'))
      .finally(() => setLoading(false));
  }, [creatureId, bestiaryClient]);

  const backPath = state?.bestiaryId ? `/bestiaries/${state.bestiaryId}` : '/bestiaries';
  const backLabel = state?.bestiaryName ? `\u2190 ${state.bestiaryName}` : '\u2190 Bestiary';

  return (
    <div className="view-creature-container">
      <div className="view-creature-nav">
        <Link to={backPath} state={state} className="view-creature-back">{backLabel}</Link>
      </div>

      {loading && <p className="view-creature-loading">Loading...</p>}
      {error && <p className="error-message">{error}</p>}
      {creature && <CreatureStatBlock data={creature.rawData} />}
    </div>
  );
};

export default ViewCreature;
