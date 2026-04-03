import React from 'react';
import { CreatureListItem } from '../../api/bestiaryClient';
import BestiarySidebar from './BestiarySidebar';
import CreatureBrowser from './CreatureBrowser';
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
    toggleBestiary, selectOnly, selectAll, clearAll,
    nameInput, creatureTypeFilter, sort, handleNameInputChange, handleCreatureTypeChange, handleSortClick, sortIndicator,
    creatures, totalCount, totalPages, currentPage,
    creaturesLoading, creaturesError, handlePageChange,
  } = useBestiarySearch();

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
            <BestiarySidebar
              bestiaries={bestiaries}
              selectedIds={selectedIds}
              bestiariesLoading={bestiariesLoading}
              bestiariesError={bestiariesError}
              toggleBestiary={toggleBestiary}
              selectOnly={selectOnly}
              selectAll={selectAll}
              clearAll={clearAll}
              className="bestiary-picker-sidebar"
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
              handlePageChange={handlePageChange}
              searchAutoFocus
              mainClassName="bestiary-picker-main"
              statBlockOverlayClass="picker-stat-block-overlay"
              firstColumn={{
                header: null,
                cell: (creature) => (
                  <button
                    className="control-button secondary"
                    onClick={(e) => { e.stopPropagation(); onCreatureSelect(creature); }}
                    aria-label={`Add ${creature.name} to encounter`}
                    title="Add to encounter"
                  >
                    +
                  </button>
                ),
              }}
            />
          </div>
        </div>
      </div>
    </>
  );
};

export default BestiaryPicker;
