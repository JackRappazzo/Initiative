import React, { useState, useRef, useEffect } from 'react';

// Combined list of XPHB conditions and statuses
const PREDEFINED_STATUSES = [
  'Blinded',
  'Charmed',
  'Concentration',
  'Deafened',
  'Exhaustion',
  'Frightened',
  'Grappled',
  'Incapacitated',
  'Invisible',
  'Paralyzed',
  'Petrified',
  'Poisoned',
  'Prone',
  'Restrained',
  'Stunned',
  'Unconscious',
];

interface StatusTypeaheadProps {
  existingStatuses: string[];
  onStatusSelected: (status: string) => void;
  onDismiss: () => void;
}

export const StatusTypeahead: React.FC<StatusTypeaheadProps> = ({
  existingStatuses,
  onStatusSelected,
  onDismiss,
}) => {
  const [input, setInput] = useState('');
  const [filteredOptions, setFilteredOptions] = useState<string[]>(PREDEFINED_STATUSES);
  const [selectedIndex, setSelectedIndex] = useState(0);
  const inputRef = useRef<HTMLInputElement>(null);
  const listRef = useRef<HTMLUListElement>(null);

  // Filter options based on input and existing statuses
  useEffect(() => {
    const lowerInput = input.toLowerCase();
    const filtered = PREDEFINED_STATUSES.filter((status) => {
      // Skip if already in existing statuses (case-insensitive)
      if (existingStatuses.some((s) => s.toLowerCase() === status.toLowerCase())) {
        return false;
      }
      // Match on prefix or substring
      return status.toLowerCase().includes(lowerInput);
    });

    setFilteredOptions(filtered);
    setSelectedIndex(0); // Reset selection when options change
  }, [input, existingStatuses]);

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    switch (e.key) {
      case 'ArrowDown':
        e.preventDefault();
        setSelectedIndex((prev) => (prev + 1) % Math.max(filteredOptions.length, 1));
        break;
      case 'ArrowUp':
        e.preventDefault();
        setSelectedIndex((prev) =>
          prev === 0 ? Math.max(filteredOptions.length - 1, 0) : prev - 1
        );
        break;
      case 'Enter':
        e.preventDefault();
        handleSelect();
        break;
      case 'Escape':
        e.preventDefault();
        onDismiss();
        break;
    }
  };

  const handleSelect = () => {
    // If there are filtered options and one is selected, use that
    if (filteredOptions.length > 0 && selectedIndex < filteredOptions.length) {
      onStatusSelected(filteredOptions[selectedIndex]);
      return;
    }

    // Otherwise use custom input
    const trimmed = input.trim();
    if (trimmed) {
      onStatusSelected(trimmed);
    }
  };

  useEffect(() => {
    inputRef.current?.focus();
  }, []);

  // Scroll selected item into view
  useEffect(() => {
    if (listRef.current && selectedIndex >= 0) {
      const items = listRef.current.querySelectorAll('li');
      const selectedItem = items[selectedIndex];
      if (selectedItem) {
        selectedItem.scrollIntoView({ block: 'nearest' });
      }
    }
  }, [selectedIndex, filteredOptions]);

  return (
    <div className="status-typeahead-overlay">
      <div className="status-typeahead-modal">
        <div className="status-typeahead-header">
          <h3>Add Status or Condition</h3>
          <button
            className="status-typeahead-close"
            onClick={onDismiss}
            title="Close"
          >
            ✕
          </button>
        </div>

        <div className="status-typeahead-input-wrapper">
          <input
            ref={inputRef}
            type="text"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder="Type to filter or enter custom status"
            className="status-typeahead-input"
            aria-label="Status input"
          />
        </div>

        {filteredOptions.length > 0 && (
          <div className="status-typeahead-options">
            <ul ref={listRef} className="status-typeahead-list">
              {filteredOptions.map((status, index) => (
                <li
                  key={status}
                  className={`status-typeahead-item ${index === selectedIndex ? 'selected' : ''}`}
                  onClick={() => {
                    setSelectedIndex(index);
                    onStatusSelected(status);
                  }}
                >
                  {status}
                </li>
              ))}
            </ul>
          </div>
        )}

        <div className="status-typeahead-footer">
          <button
            className="status-typeahead-button cancel"
            onClick={onDismiss}
          >
            Cancel
          </button>
          <button
            className="status-typeahead-button primary"
            onClick={handleSelect}
            disabled={!input.trim() && filteredOptions.length === 0}
            title={
              input.trim()
                ? `Add "${input.trim()}"`
                : filteredOptions.length > 0
                  ? `Add "${filteredOptions[selectedIndex]}"`
                  : 'Enter a status or select from list'
            }
          >
            Add
          </button>
        </div>
      </div>
    </div>
  );
};
