import React, { useRef, useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { BestiaryClient, CreatureDetail, CreatureListItem } from '../../api/bestiaryClient';
import Pagination from '../../components/Pagination';
import CreatureStatBlock from '../../components/bestiaries/CreatureStatBlock';
import { useBestiarySearch } from '../../hooks';
import './ListBestiaries.css';

const bestiaryClient = new BestiaryClient();

const ListBestiaries: React.FC = () => {
  const navigate = useNavigate();

  const {
    bestiaries, selectedIds, bestiariesLoading, bestiariesError,
    toggleBestiary, selectAll, clearAll, refreshBestiaries,
    nameInput, creatureTypeFilter, sort, handleNameInputChange, handleCreatureTypeChange, handleSortClick, sortIndicator,
    creatures, totalCount, totalPages, currentPage,
    creaturesLoading, creaturesError, handlePageChange,
  } = useBestiarySearch();

  const [newBestiaryName, setNewBestiaryName] = useState('');
  const [creatingBestiary, setCreatingBestiary] = useState(false);
  const [showNewInput, setShowNewInput] = useState(false);
  const [createError, setCreateError] = useState<string | null>(null);
  const creatingRef = useRef(false);

  const [statBlockCreature, setStatBlockCreature] = useState<CreatureDetail | null>(null);
  const [statBlockLoading, setStatBlockLoading] = useState(false);

  const handleCreatureClick = async (creature: CreatureListItem) => {
    setStatBlockLoading(true);
    try {
      const detail = await bestiaryClient.getCreatureById(creature.id);
      setStatBlockCreature(detail);
    } finally {
      setStatBlockLoading(false);
    }
  };

  const onPageChange = (page: number) => {
    handlePageChange(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleCreateBestiary = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newBestiaryName.trim() || creatingRef.current) return;
    creatingRef.current = true;
    setCreatingBestiary(true);
    setCreateError(null);
    try {
      await bestiaryClient.createBestiary(newBestiaryName.trim());
      setNewBestiaryName('');
      setShowNewInput(false);
      refreshBestiaries();
    } catch {
      setCreateError('Failed to create bestiary.');
    } finally {
      creatingRef.current = false;
      setCreatingBestiary(false);
    }
  };

  // ── Render ────────────────────────────────────────────────────────────────
  return (
    <>
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

        {showNewInput ? (
          <form className="new-bestiary-form" onSubmit={handleCreateBestiary}>
            <input
              value={newBestiaryName}
              onChange={e => setNewBestiaryName(e.target.value)}
              placeholder="Bestiary name…"
              disabled={creatingBestiary}
              autoFocus
            />
            <button type="submit" disabled={creatingBestiary || !newBestiaryName.trim()}>
              {creatingBestiary ? '…' : 'Create'}
            </button>
            <button type="button" onClick={() => setShowNewInput(false)} disabled={creatingBestiary}>Cancel</button>
            {createError && <p className="sidebar-error">{createError}</p>}
          </form>
        ) : (
          <button className="new-bestiary-btn" onClick={() => setShowNewInput(true)}>+ New Bestiary</button>
        )}

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
                  {b.ownerId && (
                    <button
                      className="sidebar-edit-btn"
                      onClick={e => { e.preventDefault(); navigate(`/bestiaries/${b.id}/edit`); }}
                    >Edit</button>
                  )}
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
          <select
            className="bestiary-type-filter"
            value={creatureTypeFilter}
            onChange={handleCreatureTypeChange}
            aria-label="Filter by creature type"
          >
            <option value="">All types</option>
            <option value="aberration">Aberration</option>
            <option value="beast">Beast</option>
            <option value="celestial">Celestial</option>
            <option value="construct">Construct</option>
            <option value="dragon">Dragon</option>
            <option value="elemental">Elemental</option>
            <option value="fey">Fey</option>
            <option value="fiend">Fiend</option>
            <option value="giant">Giant</option>
            <option value="humanoid">Humanoid</option>
            <option value="monstrosity">Monstrosity</option>
            <option value="ooze">Ooze</option>
            <option value="plant">Plant</option>
            <option value="undead">Undead</option>
          </select>
          {!creaturesLoading && (
            <span className="bestiary-count">{totalCount.toLocaleString()} creatures</span>
          )}
        </div>

        {creaturesError && <div className="error-message">{creaturesError}</div>}

        {!creaturesError && (
          <>
            <Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={onPageChange} disabled={creaturesLoading} />

            <table className="creature-table">
              <thead>
                <tr>
                  <th className="creature-th-link"></th>
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
                  <th>Source</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {creaturesLoading ? (
                  <tr>
                    <td colSpan={6} className="table-loading">Loading...</td>
                  </tr>
                ) : creatures.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="table-empty">No creatures match your filters.</td>
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
                      <td className="creature-td-link">
                        <button
                          className="creature-page-btn"
                          onClick={(e) => { e.stopPropagation(); navigate(`/bestiaries/creatures/${creature.id}`); }}
                          title={`Open ${creature.name} page`}
                          aria-label={`Open ${creature.name} full page`}
                        >↗</button>
                      </td>
                      <td className="creature-name">{creature.name}</td>
                      <td className="creature-type">{creature.creatureType ?? '—'}</td>
                      <td className="creature-cr">{creature.challengeRating ?? '—'}</td>
                      <td className="creature-source">{creature.source ?? '—'}</td>
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

            <Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={onPageChange} disabled={creaturesLoading} />
          </>
        )}
      </main>
    </div>

    {statBlockLoading && (
      <div className="stat-block-overlay">
        <div className="stat-block-modal stat-block-modal--loading">Loading…</div>
      </div>
    )}

    {statBlockCreature && (
      <div className="stat-block-overlay" onClick={() => setStatBlockCreature(null)}>
        <div className="stat-block-modal" onClick={(e) => e.stopPropagation()}>
          <button className="stat-block-close" onClick={() => setStatBlockCreature(null)} aria-label="Close">✕</button>
          <CreatureStatBlock data={statBlockCreature!.rawData} />
        </div>
      </div>
    )}
  </>
  );
};

export default ListBestiaries;
