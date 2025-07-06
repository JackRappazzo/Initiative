import React from 'react';

interface EncounterHeaderProps {
  displayName: string;
  editingName: boolean;
  newName: string;
  onNameChange: (name: string) => void;
  onStartEdit: () => void;
  onSaveEdit: () => void;
  onCancelEdit: () => void;
  onKeyDown: (e: React.KeyboardEvent) => void;
}

export const EncounterHeader: React.FC<EncounterHeaderProps> = ({
  displayName,
  editingName,
  newName,
  onNameChange,
  onStartEdit,
  onSaveEdit,
  onCancelEdit,
  onKeyDown
}) => {
  return (
    <div className="encounter-name">
      {editingName ? (
        <>
          <input
            type="text"
            value={newName}
            onChange={(e) => onNameChange(e.target.value)}
            onKeyDown={onKeyDown}
            autoFocus
          />
          <button className="control-button primary" onClick={onSaveEdit}>Save</button>
          <button className="control-button secondary" onClick={onCancelEdit}>Cancel</button>
        </>
      ) : (
        <>
          <h1>{displayName}</h1>
          <button className="control-button secondary" onClick={onStartEdit}>
            Edit Name
          </button>
        </>
      )}
    </div>
  );
};
