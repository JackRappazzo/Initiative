import React, { useEffect, useState, useCallback, useMemo } from 'react';
import { Link, useParams, useLocation, useNavigate } from 'react-router-dom';
import { BestiaryClient, CreatureListItem } from '../../api/bestiaryClient';
import Pagination from '../../components/Pagination';
import './BrowseBestiary.css';

const PAGE_SIZE = 20;

interface LocationState {
  bestiaryName?: string;
}

const BrowseBestiary: React.FC = () => {
  const { bestiaryId } = useParams<{ bestiaryId: string }>();
  const location = useLocation();
  const navigate = useNavigate();
  const locationState = location.state as LocationState | null;

  const [bestiaryName, setBestiaryName] = useState<string>(locationState?.bestiaryName ?? '');
  const [creatures, setCreatures] = useState<CreatureListItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const bestiaryClient = useMemo(() => new BestiaryClient(), []);
  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));

  // If we arrived directly (no router state), resolve the bestiary name
  useEffect(() => {
    if (bestiaryName || !bestiaryId) return;
    bestiaryClient.getAvailableBestiaries().then((list) => {
      const match = list.find((b) => b.id === bestiaryId);
      if (match) setBestiaryName(match.name);
    }).catch(() => { /* non-fatal */ });
  }, [bestiaryId, bestiaryName, bestiaryClient]);

  const loadPage = useCallback(async (page: number) => {
    if (!bestiaryId) return;
    setLoading(true);
    setError(null);
    try {
      const skip = (page - 1) * PAGE_SIZE;
      const [pageCreatures, count] = await Promise.all([
        bestiaryClient.searchCreatures({ bestiaryIds: [bestiaryId], pageSize: PAGE_SIZE, skip }),
        bestiaryClient.countCreatures({ bestiaryIds: [bestiaryId] }),
      ]);
      setCreatures(pageCreatures);
      setTotalCount(count);
    } catch {
      setError('Failed to load creatures');
    } finally {
      setLoading(false);
    }
  }, [bestiaryId, bestiaryClient]);

  useEffect(() => { loadPage(currentPage); }, [loadPage, currentPage]);

  const handlePageChange = (page: number) => {
    if (page < 1 || page > totalPages) return;
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleCreatureClick = (creature: CreatureListItem) => {
    navigate(`/bestiaries/${bestiaryId}/creatures/${creature.id}`, {
      state: { bestiaryId, bestiaryName },
    });
  };

  return (
    <div className="browse-container">
      <div className="browse-header">
        <Link to="/bestiaries" className="browse-back-link">&#8592; Bestiaries</Link>
        <h1>{bestiaryName || 'Bestiary'}</h1>
        {!loading && <p className="browse-subtitle">{totalCount.toLocaleString()} creatures</p>}
      </div>

      {error && <div className="error-message">{error}</div>}

      {!error && (
        <>
          <Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={handlePageChange} disabled={loading} />

          <table className="creature-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Type</th>
                <th>CR</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={4} className="table-loading">Loading...</td>
                </tr>
              ) : creatures.length === 0 ? (
                <tr>
                  <td colSpan={4} className="table-empty">No creatures found.</td>
                </tr>
              ) : (
                creatures.map((creature) => (
                  <tr
                    key={creature.id}
                    className="creature-row creature-row--clickable"
                    onClick={() => handleCreatureClick(creature)}
                    tabIndex={0}
                    onKeyDown={(e) => e.key === 'Enter' && handleCreatureClick(creature)}
                  >
                    <td className="creature-name">{creature.name}</td>
                    <td className="creature-type">{creature.creatureType ?? '—'}</td>
                    <td className="creature-cr">{creature.challengeRating ?? '—'}</td>
                    <td className="creature-flags">
                      {creature.isLegendary && (
                        <span className="legendary-badge">Legendary</span>
                      )}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>

          <Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={handlePageChange} disabled={loading} />
        </>
      )}
    </div>
  );
};

export default BrowseBestiary;
