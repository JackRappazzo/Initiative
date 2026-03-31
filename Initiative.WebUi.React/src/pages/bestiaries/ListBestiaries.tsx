import React, { useEffect, useState, useMemo, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { BestiaryClient, BestiaryListItem, CreatureListItem, CreatureSortBy } from '../../api/bestiaryClient';
import Pagination from '../../components/Pagination';
import './ListBestiaries.css';

const PAGE_SIZE = 20;
const DEBOUNCE_MS = 300;

interface SortState { col: CreatureSortBy; desc: boolean; }

const ListBestiaries: React.FC = () => {
  const navigate = useNavigate();
  const bestiaryClient = useMemo(() => new BestiaryClient(), []);

  // ── Sidebar / filter state ────────────────────────────────────────────────
  const [bestiaries, setBestiaries] = useState<BestiaryListItem[]>([]);
  const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set());
  const [bestiariesLoading, setBestiariesLoading] = useState(true);
  const [bestiariesError, setBestiariesError] = useState<string | null>(null);

  // ── Search / creature state ───────────────────────────────────────────────
  const [nameInput, setNameInput] = useState('');
  const [nameFilter, setNameFilter] = useState('');
  const [sort, setSort] = useState<SortState>({ col: 'Name', desc: false });
  const [creatures, setCreatures] = useState<CreatureListItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [creaturesLoading, setCreaturesLoading] = useState(false);
  const [creaturesError, setCreaturesError] = useState<string | null>(null);

  const debounceTimer = useRef<ReturnType<typeof setTimeout> | null>(null);

  // ── Load bestiaries on mount; default-select all ──────────────────────────
  useEffect(() => {
    bestiaryClient.getAvailableBestiaries()
      .then((data) => {
        setBestiaries(data);
        setSelectedIds(new Set(data.map((b) => b.id)));
      })
      .catch(() => setBestiariesError('Failed to load bestiaries'))
      .finally(() => setBestiariesLoading(false));
  }, [bestiaryClient]);

  // ── Load creatures whenever filters or page changes ───────────────────────
  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));

  useEffect(() => {
    if (bestiariesLoading) return;
    let cancelled = false;
    const run = async () => {
      setCreaturesLoading(true);
      setCreaturesError(null);
      try {
        const skip = (currentPage - 1) * PAGE_SIZE;
        const idArray = Array.from(selectedIds);
        const params = {
          bestiaryIds: idArray,
          name: nameFilter || undefined,
          sortBy: sort.col,
          sortDescending: sort.desc ? true : undefined,
          pageSize: PAGE_SIZE,
          skip,
        };
        const [results, count] = await Promise.all([
          bestiaryClient.searchCreatures(params),
          bestiaryClient.countCreatures({ bestiaryIds: idArray, name: nameFilter || undefined }),
        ]);
        if (!cancelled) { setCreatures(results); setTotalCount(count); }
      } catch {
        if (!cancelled) setCreaturesError('Failed to load creatures');
      } finally {
        if (!cancelled) setCreaturesLoading(false);
      }
    };
    run();
    return () => { cancelled = true; };
  }, [bestiariesLoading, selectedIds, nameFilter, sort, currentPage, bestiaryClient]);

  // ── Debounce name input → nameFilter ─────────────────────────────────────
  const handleNameInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setNameInput(value);
    if (debounceTimer.current) clearTimeout(debounceTimer.current);
    debounceTimer.current = setTimeout(() => {
      setNameFilter(value);
      setCurrentPage(1);
    }, DEBOUNCE_MS);
  };

  // ── Sort handler ──────────────────────────────────────────────────────────
  const handleSortClick = (col: CreatureSortBy) => {
    setSort((prev) => ({ col, desc: prev.col === col ? !prev.desc : false }));
    if (currentPage !== 1) setCurrentPage(1);
  };

  const sortIndicator = (col: CreatureSortBy) => {
    if (sort.col !== col) return '';
    return sort.desc ? ' ▼' : ' ▲';
  };

  // ── Bestiary sidebar handlers ─────────────────────────────────────────────
  const toggleBestiary = (id: string) => {
    setSelectedIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) { next.delete(id); } else { next.add(id); }
      return next;
    });
    setCurrentPage(1);
  };

  const selectAll = () => {
    setSelectedIds(new Set(bestiaries.map((b) => b.id)));
    setCurrentPage(1);
  };

  const clearAll = () => {
    setSelectedIds(new Set());
    setCurrentPage(1);
  };

  // ── Creature navigation ───────────────────────────────────────────────────
  const handleCreatureClick = (creature: CreatureListItem) => {
    navigate(`/bestiaries/creatures/${creature.id}`);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  // ── Render ────────────────────────────────────────────────────────────────
  return (
    <div className="bestiaries-layout">
      {/* Sidebar */}
      <aside className="bestiaries-sidebar">
        <div className="sidebar-header">
          <span className="sidebar-title">Bestiaries</span>
          <div className="sidebar-controls">
            <button className="sidebar-ctrl-btn" onClick={selectAll} disabled={bestiariesLoading}>All</button>
            <span className="sidebar-ctrl-sep">·</span>
            <button className="sidebar-ctrl-btn" onClick={clearAll} disabled={bestiariesLoading}>None</button>
          </div>
        </div>

        {bestiariesError && <p className="sidebar-error">{bestiariesError}</p>}

        {bestiariesLoading ? (
          <p className="sidebar-loading">Loading...</p>
        ) : (
          <ul className="sidebar-list">
            {bestiaries.map((b) => (
              <li key={b.id} className="sidebar-item">
                <label className="sidebar-label">
                  <input
                    type="checkbox"
                    className="sidebar-checkbox"
                    checked={selectedIds.has(b.id)}
                    onChange={() => toggleBestiary(b.id)}
                  />
                  <span className="sidebar-name">{b.name}</span>
                  {b.source && <span className="sidebar-source">{b.source}</span>}
                  <span className={`sidebar-badge ${b.ownerId ? 'badge-custom' : 'badge-system'}`}>
                    {b.ownerId ? 'Custom' : 'System'}
                  </span>
                </label>
              </li>
            ))}
          </ul>
        )}
      </aside>

      {/* Main content */}
      <main className="bestiaries-main">
        <div className="bestiaries-toolbar">
          <h1 className="bestiaries-title">Creatures</h1>
          <input
            type="search"
            className="bestiary-search-bar"
            placeholder="Search by name…"
            value={nameInput}
            onChange={handleNameInputChange}
            aria-label="Search creatures by name"
          />
          {!creaturesLoading && (
            <span className="bestiary-count">{totalCount.toLocaleString()} creatures</span>
          )}
        </div>

        {creaturesError && <div className="error-message">{creaturesError}</div>}

        {!creaturesError && (
          <>
            <Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={handlePageChange} disabled={creaturesLoading} />

            <table className="creature-table">
              <thead>
                <tr>
                  <th
                    className={`sortable-th${sort.col === 'Name' ? ' sortable-th--active' : ''}`}
                    onClick={() => handleSortClick('Name')}
                    aria-sort={sort.col === 'Name' ? (sort.desc ? 'descending' : 'ascending') : 'none'}
                  >
                    Name{sortIndicator('Name')}
                  </th>
                  <th
                    className={`sortable-th${sort.col === 'Type' ? ' sortable-th--active' : ''}`}
                    onClick={() => handleSortClick('Type')}
                    aria-sort={sort.col === 'Type' ? (sort.desc ? 'descending' : 'ascending') : 'none'}
                  >
                    Type{sortIndicator('Type')}
                  </th>
                  <th
                    className={`sortable-th${sort.col === 'ChallengeRating' ? ' sortable-th--active' : ''}`}
                    onClick={() => handleSortClick('ChallengeRating')}
                    aria-sort={sort.col === 'ChallengeRating' ? (sort.desc ? 'descending' : 'ascending') : 'none'}
                  >
                    CR{sortIndicator('ChallengeRating')}
                  </th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {creaturesLoading ? (
                  <tr>
                    <td colSpan={4} className="table-loading">Loading...</td>
                  </tr>
                ) : creatures.length === 0 ? (
                  <tr>
                    <td colSpan={4} className="table-empty">No creatures match your filters.</td>
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

            <Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={handlePageChange} disabled={creaturesLoading} />
          </>
        )}
      </main>
    </div>
  );
};

export default ListBestiaries;
