import React from 'react';
import { BestiaryListItem } from '../../api/bestiaryClient';

interface BestiarySidebarProps {
  bestiaries: BestiaryListItem[];
  selectedIds: Set<string>;
  bestiariesLoading: boolean;
  bestiariesError: string | null;
  toggleBestiary: (id: string) => void;
  selectOnly: (id: string) => void;
  selectAll: () => void;
  clearAll: () => void;
  className?: string;
  headerSlot?: React.ReactNode;
  extraItemActions?: (bestiary: BestiaryListItem) => React.ReactNode;
}

const BestiarySidebar: React.FC<BestiarySidebarProps> = ({
  bestiaries,
  selectedIds,
  bestiariesLoading,
  bestiariesError,
  toggleBestiary,
  selectOnly,
  selectAll,
  clearAll,
  className,
  headerSlot,
  extraItemActions,
}) => {
  return (
    <aside className={`bestiaries-sidebar${className ? ` ${className}` : ''}`}>
      <div className="sidebar-header">
        <span className="sidebar-title">Bestiaries</span>
        <div className="sidebar-controls">
          <button className="sidebar-ctrl-btn" onClick={selectAll} disabled={bestiariesLoading}>All</button>
          <span className="sidebar-ctrl-sep">·</span>
          <button className="sidebar-ctrl-btn" onClick={clearAll} disabled={bestiariesLoading}>None</button>
        </div>
      </div>

      {bestiariesError && <p className="sidebar-error">{bestiariesError}</p>}

      {headerSlot}

      {bestiariesLoading ? (
        <p className="sidebar-loading">Loading...</p>
      ) : (
        <ul className="sidebar-list">
          {bestiaries.map((b) => (
            <li key={b.id} className="sidebar-item" onDoubleClick={() => selectOnly(b.id)}>
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
                {extraItemActions?.(b)}
              </label>
            </li>
          ))}
        </ul>
      )}
    </aside>
  );
};

export default BestiarySidebar;
