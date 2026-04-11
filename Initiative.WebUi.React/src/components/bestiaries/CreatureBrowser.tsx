import React, { useState, useCallback, useMemo } from 'react';
import { BestiaryClient, CreatureListItem, CreatureSortBy } from '../../api/bestiaryClient';
import { SortState } from '../../hooks/useBestiarySearch';
import './CreatureBrowser.css';
import Pagination from '../Pagination';
import CreatureStatBlock from './CreatureStatBlock';

interface FirstColumnConfig {
  header: React.ReactNode;
  cell: (creature: CreatureListItem) => React.ReactNode;
}

interface CreatureBrowserProps {
  creatures: CreatureListItem[];
  totalCount: number;
  totalPages: number;
  currentPage: number;
  creaturesLoading: boolean;
  creaturesError: string | null;
  nameInput: string;
  creatureTypeFilter: string;
  sort: SortState;
  handleNameInputChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  handleCreatureTypeChange: (e: React.ChangeEvent<HTMLSelectElement>) => void;
  handleSortClick: (col: CreatureSortBy) => void;
  sortIndicator: (col: CreatureSortBy) => string;
  handlePageChange: (page: number) => void;
  firstColumn: FirstColumnConfig;
  toolbarExtras?: React.ReactNode;
  statBlockOverlayClass?: string;
  searchAutoFocus?: boolean;
  mainClassName?: string;
}

const CreatureBrowser: React.FC<CreatureBrowserProps> = ({
  creatures,
  totalCount,
  totalPages,
  currentPage,
  creaturesLoading,
  creaturesError,
  nameInput,
  sort,
  handleNameInputChange,
  handleCreatureTypeChange,
  creatureTypeFilter,
  handleSortClick,
  sortIndicator,
  handlePageChange,
  firstColumn,
  toolbarExtras,
  statBlockOverlayClass,
  searchAutoFocus,
  mainClassName,
}) => {
  const bestiaryClient = useMemo(() => new BestiaryClient(), []);
  const [statBlockData, setStatBlockData] = useState<import('../../api/bestiaryClient').FiveEToolsRawData | null>(null);
  const [statBlockLoading, setStatBlockLoading] = useState(false);

  const openStatBlock = useCallback(async (creature: CreatureListItem) => {
    setStatBlockLoading(true);
    try {
      const detail = await bestiaryClient.getCreatureById(creature.id);
      setStatBlockData(detail.rawData);
    } catch {
      // ignore load errors
    } finally {
      setStatBlockLoading(false);
    }
  }, [bestiaryClient]);

  const overlayClass = `stat-block-overlay${statBlockOverlayClass ? ` ${statBlockOverlayClass}` : ''}`;

  return (
    <>
      <main className={`bestiaries-main${mainClassName ? ` ${mainClassName}` : ''}`}>
        <div className="bestiaries-toolbar">
          <input
            type="search"
            className="bestiary-search-bar"
            placeholder="Search by name…"
            value={nameInput}
            onChange={handleNameInputChange}
            aria-label="Search creatures by name"
            autoFocus={searchAutoFocus}
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
          {toolbarExtras}
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
                  <th>{firstColumn.header}</th>
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
                        {firstColumn.cell(creature)}
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

      {statBlockLoading && (
        <div className={overlayClass}>
          <div className="stat-block-modal stat-block-modal--loading">Loading…</div>
        </div>
      )}

      {statBlockData && (
        <div className={overlayClass} onClick={() => setStatBlockData(null)}>
          <div className="stat-block-modal" onClick={(e) => e.stopPropagation()}>
            <button className="stat-block-close" onClick={() => setStatBlockData(null)} aria-label="Close">✕</button>
            <CreatureStatBlock data={statBlockData} />
          </div>
        </div>
      )}
    </>
  );
};

export default CreatureBrowser;
