import React, { useEffect, useState, useMemo } from 'react';
import { Link, useParams } from 'react-router-dom';
import { BestiaryClient, CreatureDetail } from '../../api/bestiaryClient';
import CreatureStatBlock from '../../components/bestiaries/CreatureStatBlock';
import './ViewCreature.css';

const ViewCreature: React.FC = () => {
  const { creatureId } = useParams<{ creatureId: string }>();

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

  const backPath = '/bestiaries';
  const backLabel = '\u2190 Bestiaries';

  return (
    <div className="view-creature-container">
      <div className="view-creature-nav">
        <Link to={backPath} className="view-creature-back">{backLabel}</Link>
      </div>

      {loading && <p className="view-creature-loading">Loading...</p>}
      {error && <p className="error-message">{error}</p>}
      {creature && <CreatureStatBlock data={creature.rawData} />}
    </div>
  );
};

export default ViewCreature;
