import React, { useRef, useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { BestiaryClient, BestiaryListItem } from '../../api/bestiaryClient';
import BestiarySidebar from '../../components/bestiaries/BestiarySidebar';
import CreatureBrowser from '../../components/bestiaries/CreatureBrowser';
import { useBestiarySearch } from '../../hooks';
import './ListBestiaries.css';

const ListBestiaries: React.FC = () => {
  const navigate = useNavigate();
  const bestiaryClient = useMemo(() => new BestiaryClient(), []);

  const {
    bestiaries, selectedIds, bestiariesLoading, bestiariesError,
    toggleBestiary, selectOnly, selectAll, clearAll, refreshBestiaries,
    nameInput, creatureTypeFilter, sort, handleNameInputChange, handleCreatureTypeChange, handleSortClick, sortIndicator,
    creatures, totalCount, totalPages, currentPage,
    creaturesLoading, creaturesError, handlePageChange,
  } = useBestiarySearch();

  const [newBestiaryName, setNewBestiaryName] = useState('');
  const [creatingBestiary, setCreatingBestiary] = useState(false);
  const [showNewInput, setShowNewInput] = useState(false);
  const [createError, setCreateError] = useState<string | null>(null);
  const creatingRef = useRef(false);

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

  const onPageChange = (page: number) => {
    handlePageChange(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const newBestiarySlot = showNewInput ? (
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
  );

  // ── Render ────────────────────────────────────────────────────────────────
  return (
    <div className="bestiaries-layout">
      <BestiarySidebar
        bestiaries={bestiaries}
        selectedIds={selectedIds}
        bestiariesLoading={bestiariesLoading}
        bestiariesError={bestiariesError}
        toggleBestiary={toggleBestiary}
        selectOnly={selectOnly}
        selectAll={selectAll}
        clearAll={clearAll}
        headerSlot={newBestiarySlot}
        extraItemActions={(b: BestiaryListItem) => b.ownerId ? (
          <button
            className="sidebar-edit-btn"
            onClick={e => { e.preventDefault(); navigate(`/bestiaries/${b.id}/edit`); }}
          >Edit</button>
        ) : null}
      />
      <CreatureBrowser
        creatures={creatures}
        totalCount={totalCount}
        totalPages={totalPages}
        currentPage={currentPage}
        creaturesLoading={creaturesLoading}
        creaturesError={creaturesError}
        nameInput={nameInput}
        creatureTypeFilter={creatureTypeFilter}
        sort={sort}
        handleNameInputChange={handleNameInputChange}
        handleCreatureTypeChange={handleCreatureTypeChange}
        handleSortClick={handleSortClick}
        sortIndicator={sortIndicator}
        handlePageChange={onPageChange}
        toolbarExtras={<h1 className="bestiaries-title">Creatures</h1>}
        firstColumn={{
          header: <span className="creature-th-link" />,
          cell: (creature) => (
            <button
              className="creature-page-btn"
              onClick={(e) => { e.stopPropagation(); navigate(`/bestiaries/creatures/${creature.id}`); }}
              title={`Open ${creature.name} page`}
              aria-label={`Open ${creature.name} full page`}
            >↗</button>
          ),
        }}
      />
    </div>
  );
};

export default ListBestiaries;
