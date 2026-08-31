import React, { useState } from 'react';
import { useDndBeyondSession } from '../contexts/DndBeyondContext';
import './SettingsPage.css';

const isTokenExpired = (token: string): boolean => {
  try {
    const payloadBase64 = token.split('.')[1];
    const payload = JSON.parse(atob(payloadBase64));
    return typeof payload.exp === 'number' && payload.exp * 1000 <= Date.now();
  } catch {
    return false;
  }
};

const SettingsPage: React.FC = () => {
  const { token, setToken, clearToken } = useDndBeyondSession();
  const [draft, setDraft] = useState('');

  const saveToken = () => {
    const trimmed = draft.trim();
    if (!trimmed) return;
    setToken(trimmed);
    setDraft('');
  };

  return (
    <div className="settings-container">
      <h1>Settings</h1>

      <section className="settings-section">
        <h2>D&D Beyond</h2>
        <p className="settings-hint">
          Paste your D&D Beyond session (cobalt) token. It is stored only in this browser
          and is never saved to the Initiative database.
        </p>

        {token && (
          <p className={`settings-status ${isTokenExpired(token) ? 'error' : 'success'}`}>
            {isTokenExpired(token)
              ? 'Stored token appears to have expired.'
              : 'D&D Beyond token stored.'}
          </p>
        )}

        <textarea
          className="settings-token-input"
          value={draft}
          onChange={(e) => setDraft(e.target.value)}
          placeholder="Paste D&D Beyond session token"
          rows={4}
          spellCheck={false}
        />

        <div className="settings-actions">
          <button className="btn-primary" onClick={saveToken} disabled={!draft.trim()}>
            Save Token
          </button>
          {token && (
            <button className="btn-secondary" onClick={clearToken}>
              Clear Token
            </button>
          )}
        </div>
      </section>
    </div>
  );
};

export default SettingsPage;
