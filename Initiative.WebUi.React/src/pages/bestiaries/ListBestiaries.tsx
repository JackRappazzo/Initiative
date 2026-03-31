import React, { useEffect, useState, useCallback, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { BestiaryClient, BestiaryListItem } from '../../api/bestiaryClient';
import Pagination from '../../components/Pagination';
import './ListBestiaries.css';

const PAGE_SIZE = 10;

const ListBestiaries: React.FC = () => {
  const navigate = useNavigate();
  const [bestiaries, setBestiaries] = useState<BestiaryListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const bestiaryClient = useMemo(() => new BestiaryClient(), []);

  const loadBestiaries = useCallback(async () => {
    try {
      const data = await bestiaryClient.getAvailableBestiaries();
      setBestiaries(data);
    } catch {
      setError('Failed to load bestiaries');
    } finally {
      setLoading(false);
    }
  }, [bestiaryClient]);

  useEffect(() => { loadBestiaries(); }, [loadBestiaries]);

  const totalPages = Math.max(1, Math.ceil(bestiaries.length / PAGE_SIZE));
  const paginated = bestiaries.slice((currentPage - 1) * PAGE_SIZE, currentPage * PAGE_SIZE);

  const handleCardClick = (bestiary: BestiaryListItem) => {
    navigate(`/bestiaries/${bestiary.id}`, { state: { bestiaryName: bestiary.name } });
  };

  if (loading) return <div className="bestiaries-container">Loading bestiaries...</div>;
  if (error) return <div className="error-message">{error}</div>;

  return (
    <div className="bestiaries-container">
      <div className="bestiaries-header">
        <h1>Bestiaries</h1>
      </div>

      {bestiaries.length === 0 ? (
        <p>No bestiaries available.</p>
      ) : (
        <>
          <Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={setCurrentPage} />

          <div className="bestiaries-grid">
            {paginated.map((bestiary) => (
              <div
                key={bestiary.id}
                className="bestiary-card"
                onClick={() => handleCardClick(bestiary)}
                role="button"
                tabIndex={0}
                onKeyDown={(e) => e.key === 'Enter' && handleCardClick(bestiary)}
              >
                <div className="bestiary-card-name">{bestiary.name}</div>
                <div className="bestiary-card-meta">
                  <span className={`bestiary-badge ${bestiary.ownerId ? 'badge-custom' : 'badge-system'}`}>
                    {bestiary.ownerId ? 'Custom' : 'System'}
                  </span>
                  {bestiary.source && (
                    <span className="bestiary-source">{bestiary.source}</span>
                  )}
                </div>
              </div>
            ))}
          </div>

          <Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={setCurrentPage} />
        </>
      )}
    </div>
  );
};

export default ListBestiaries;
