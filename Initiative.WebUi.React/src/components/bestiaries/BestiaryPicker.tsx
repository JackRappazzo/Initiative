import React, { useState, useMemo, useCallback } from 'react';
import { BestiaryClient, FiveEToolsRawData, CreatureListItem } from '../../api/bestiaryClient';
import CreatureStatBlock from './CreatureStatBlock';
import Pagination from '../Pagination';
import { useBestiarySearch } from '../../hooks';
import '../../pages/bestiaries/ListBestiaries.css';
import './BestiaryPicker.css';

interface BestiaryPickerProps {
  onCreatureSelect: (creature: CreatureListItem) => void;
  onClose: () => void;
}

const BestiaryPicker: React.FC<BestiaryPickerProps> = ({ onCreatureSelect, onClose }) => {
  const {
    bestiaries, selectedIds, bestiariesLoading, bestiariesError,
    toggleBestiary, selectAll, clearAll,
    nameInput, sort, handleNameInputChange, handleSortClick, sortIndicator,
    creatures, totalCount, totalPages, currentPage,
    creaturesLoading, creaturesError, handlePageChange,
  } = useBestiarySearch();

  const bestiaryClient = useMemo(() => new BestiaryClient(), []);
  const [statBlockData, setStatBlockData] = useState<FiveEToolsRawData | null>(null);

  const openStatBlock = useCallback(async (creature: CreatureListItem) => {
    try {
      const detail = await bestiaryClient.getCreatureById(creature.id);
      setStatBlockData(detail.rawData);
    } catch {
      // ignore load errors
    }
  }, [bestiaryClient]);

  return (
    <>
      <div className="bestiary-picker-backdrop" onClick={onClose}>
        <div
          className="bestiary-picker-panel"
          onClick={(e) => e.stopPropagation()}
          role="dialog"
          aria-modal="true"
          aria-label="Add creature from bestiary"
        >
          {/* Header */}
          <div className="bestiary-picker-header">
            <span className="bestiary-picker-title">Add Creature</span>
            <button
              className="bestiary-picker-close"
              onClick={onClose}
              aria-label="Close bestiary picker"
            >
              ✕
            </button>
          </div>

          <div className="bestiary-picker-body">
            {/* Sidebar */}
            <aside className="bestiaries-sidebar bestiary-picker-sidebar">
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
            <main className="bestiaries-main bestiary-picker-main">
              <div className="bestiaries-toolbar">
                <input
                  type="search"
                  className="bestiary-search-bar"
                  placeholder="Search by name…"
                  value={nameInput}
                  onChange={handleNameInputChange}
                  aria-label="Search creatures by name"
                  autoFocus
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
                        <th></th>
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
                            onClick={() => openStatBlock(creature)}
                            tabIndex={0}
                            onKeyDown={(e) => e.key === 'Enter' && openStatBlock(creature)}
                          >
                            <td className="creature-statbtn-cell">
                              <button
                                className="control-button secondary"
                                onClick={(e) => { e.stopPropagation(); onCreatureSelect(creature); }}
                                aria-label={`Add ${creature.name} to encounter`}
                                title="Add to encounter"
                              >
                                +
                              </button>
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

                  <Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={handlePageChange} disabled={creaturesLoading} />
                </>
              )}
            </main>
          </div>
        </div>
      </div>

      {statBlockData && (
        <div className="stat-block-overlay picker-stat-block-overlay" onClick={() => setStatBlockData(null)}>
          <div className="stat-block-modal" onClick={(e) => e.stopPropagation()}>
            <button className="stat-block-close" onClick={() => setStatBlockData(null)}>✕</button>
            <CreatureStatBlock data={statBlockData} />
          </div>
        </div>
      )}
    </>
  );
};

export default BestiaryPicker;
