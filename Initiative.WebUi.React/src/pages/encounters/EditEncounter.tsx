import React, { useEffect, useRef, useState, useCallback, useMemo } from 'react';
import { useParams } from 'react-router-dom';
import { EncounterClient, FetchEncounterResponse } from '../../api/encounterClient';
import { CreatureListItem, BestiaryClient, FiveEToolsRawData } from '../../api/bestiaryClient';
import { EncounterState } from '../../types';
import { useCreatureManagement, useLobbyConnection } from '../../hooks';
import { EditableCreatureList, EncounterHeader, EncounterStatus } from '../../components';
import BestiaryPicker from '../../components/bestiaries/BestiaryPicker';
import PartyPicker from '../../components/encounters/PartyPicker';
import CreatureStatBlock from '../../components/bestiaries/CreatureStatBlock';
import { PartyMember } from '../../api/partyClient';
import { useUser } from '../../contexts/UserContext';
import { isTaleSpire } from '../../utils/talespire';
import { calculateEncounterDifficulty, challengeRatingToXp } from '../../utils/encounterDifficulty';

import './EditEncounter.css';

const HEALTH_STATUS_SET = new Set(['healthy', 'hurt', 'bloodied']);

const EditEncounter: React.FC = () => {
  const { encounterId } = useParams<{ encounterId: string }>();
  const encounterClient = useMemo(() => new EncounterClient(), []);
  const bestiaryClient = useMemo(() => new BestiaryClient(), []);
  const { userInfo } = useUser();
  
  // Get room code from UserContext
  const roomCode = userInfo?.roomCode;

  console.log('[EditEncounter] Room code from UserContext:', roomCode);
  console.log('[EditEncounter] User info:', userInfo);

  // Use the lobby connection hook - handles all the complex logic
  const { lobbyClient } = useLobbyConnection({
    roomCode,
    autoConnect: true,
    autoJoinRoom: true
  });

  const [encounter, setEncounter] = useState<FetchEncounterResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [editingName, setEditingName] = useState(false);
  const [showBestiaryPicker, setShowBestiaryPicker] = useState(false);
  const [showPartyPicker, setShowPartyPicker] = useState(false);
  const [newName, setNewName] = useState('');
  const [showDifficulty, setShowDifficulty] = useState(true);
  const [encounterState, setEncounterState] = useState<EncounterState>({
    currentTurn: 0,
    turnNumber: 1,
    viewersAllowed: false
  });
  const [activeStatBlock, setActiveStatBlock] = useState<FiveEToolsRawData | null>(null);
  const [showStatBlock, setShowStatBlock] = useState(true);
  const [partyMembers, setPartyMembers] = useState<PartyMember[]>([]);
  const [creatureCrById, setCreatureCrById] = useState<Record<string, string | null>>({});
  const encounterUiPrefsStorageKey = useMemo(
    () => (encounterId ? `encounterUiPrefs:${encounterId}` : null),
    [encounterId]
  );
  const turnStateLoaded = useRef(false);

  const {
    creatures,
    error,
    setError,
    updateCreature: originalUpdateCreature,
    addCreatureFromBestiary: originalAddCreatureFromBestiary,
    removeCreature: originalRemoveCreature,
    sortByInitiative: originalSortByInitiative,
    setCreatureList
  } = useCreatureManagement(encounterId, encounterClient, bestiaryClient);

  // Method to send lobby state
  const sendLobbyState = useCallback(async (
    creatureList: typeof creatures, 
    currentTurn: number, 
    turnNumber: number,
    viewersAllowed: boolean = encounterState.viewersAllowed
  ) => {
    if (!lobbyClient) {
      console.log('[EditEncounter] Not sending lobby state - no lobby client');
      return;
    }

    try {
      const getHealthStatus = (creature: (typeof creatureList)[number]): 'Healthy' | 'Hurt' | 'Bloodied' | '' => {
        if (creature.isPlayer) {
          return '';
        }

        const maxHP = Math.max(creature.maxHP ?? 0, 0);
        const currentHP = Math.max(creature.currentHP ?? 0, 0);

        if (maxHP <= 0) {
          return 'Healthy';
        }

        const hpRatio = currentHP / maxHP;
        if (hpRatio <= 0.5) {
          return 'Bloodied';
        }

        if (currentHP < maxHP) {
          return 'Hurt';
        }

        return 'Healthy';
      };

      const formatCreatureForLobby = (creature: (typeof creatureList)[number]) => {
        const displayName = (creature.displayName || 'Unnamed Creature').trim() || 'Unnamed Creature';
        const statuses = (creature.statuses ?? [])
          .map((status) => status.trim())
          .filter((status) => Boolean(status) && !HEALTH_STATUS_SET.has(status.toLowerCase()));

        return {
          displayName,
          statuses,
          healthStatus: getHealthStatus(creature),
          isPlayer: creature.isPlayer,
          isHidden: creature.isHidden ?? false,
        };
      };

      const lobbyCreatures = creatureList
        .filter(creature => !creature.isHidden) // Filter out hidden creatures
        .map(formatCreatureForLobby);
      const lobbyMode = viewersAllowed ? 'InProgress' : 'Waiting';
      
      console.log('[EditEncounter] Sending lobby state:', {
        creatures: lobbyCreatures,
        currentCreatureIndex: currentTurn,
        currentTurn: turnNumber,
        mode: lobbyMode
      });

      await lobbyClient.setLobbyState(
        lobbyCreatures,
        currentTurn,
        turnNumber,
        lobbyMode
      );
      
      console.log('[EditEncounter] Lobby state sent successfully');
    } catch (err) {
      console.error('[EditEncounter] Failed to send lobby state:', err);
    }
  }, [encounterState.viewersAllowed, lobbyClient]);

  // Enhanced creature management functions that send lobby state updates
  const updateCreature = useCallback((index: number, creature: any) => {
    originalUpdateCreature(index, creature);

    const normalizeStatuses = (statuses?: string[]) =>
      (statuses ?? [])
        .map((status) => status.trim())
        .filter((status) => Boolean(status) && !HEALTH_STATUS_SET.has(status.toLowerCase()));

    const haveStatusesChanged = (currentStatuses?: string[], updatedStatuses?: string[]) => {
      const current = normalizeStatuses(currentStatuses);
      const updated = normalizeStatuses(updatedStatuses);

      if (current.length !== updated.length) {
        return true;
      }

      return current.some((status, statusIndex) => status !== updated[statusIndex]);
    };
    
    // Send lobby state if display-affecting fields changed and viewers are allowed
    const oldCreature = creatures[index];
    if (encounterState.viewersAllowed && oldCreature && 
        (
          oldCreature.displayName !== creature.displayName ||
          oldCreature.isHidden !== creature.isHidden ||
          oldCreature.isPlayer !== creature.isPlayer ||
          oldCreature.currentHP !== creature.currentHP ||
          oldCreature.maxHP !== creature.maxHP ||
          haveStatusesChanged(oldCreature.statuses, creature.statuses)
        )) {
      const updatedCreatures = [...creatures];
      updatedCreatures[index] = creature;
      
      setTimeout(() => {
        sendLobbyState(updatedCreatures, encounterState.currentTurn, encounterState.turnNumber, encounterState.viewersAllowed);
      }, 50);
    }
  }, [originalUpdateCreature, creatures, encounterState.viewersAllowed, encounterState.currentTurn, encounterState.turnNumber, sendLobbyState]);

  const handleAddFromBestiary = useCallback(async (creature: CreatureListItem) => {
    await originalAddCreatureFromBestiary(creature);
    
    // Send lobby state after creature is added (if viewers are allowed)
    if (encounterState.viewersAllowed) {
      setTimeout(() => {
        sendLobbyState(creatures, encounterState.currentTurn, encounterState.turnNumber, encounterState.viewersAllowed);
      }, 100);
    }
  }, [originalAddCreatureFromBestiary, encounterState.viewersAllowed, encounterState.currentTurn, encounterState.turnNumber, creatures, sendLobbyState]);

  const removeCreature = useCallback(async (index: number) => {
    await originalRemoveCreature(index);
    
    const newCreatures = creatures.filter((_, i) => i !== index);
    let newCurrentTurn = encounterState.currentTurn;
    
    // Adjust current turn if needed
    if (encounterState.currentTurn >= newCreatures.length && newCreatures.length > 0) {
      newCurrentTurn = 0;
    } else if (encounterState.currentTurn > index) {
      newCurrentTurn = encounterState.currentTurn - 1;
    }
    
    if (newCurrentTurn !== encounterState.currentTurn) {
      setEncounterState(prev => ({ ...prev, currentTurn: newCurrentTurn }));
    }

    if (encounterState.viewersAllowed) {
      sendLobbyState(newCreatures, newCurrentTurn, encounterState.turnNumber, encounterState.viewersAllowed);
    }
  }, [originalRemoveCreature, encounterState.viewersAllowed, encounterState.currentTurn, encounterState.turnNumber, creatures, sendLobbyState]);

  const sortByInitiative = useCallback(async () => {
    await originalSortByInitiative();
    
    if (encounterState.viewersAllowed) {
      const sortedCreatures = [...creatures].sort((a, b) => b.initiative - a.initiative);
      sendLobbyState(sortedCreatures, encounterState.currentTurn, encounterState.turnNumber, encounterState.viewersAllowed);
    }
  }, [originalSortByInitiative, encounterState.viewersAllowed, encounterState.currentTurn, encounterState.turnNumber, creatures, sendLobbyState]);

  // Handle creature list changes from EditableCreatureList
  const handleCreaturesChange = useCallback(async (newCreatures: typeof creatures) => {
    // Update local state first
    setCreatureList(newCreatures);
    
    // Save to repository
    try {
      if (!encounterId) return;
      await encounterClient.setCreatures(encounterId, newCreatures);
    } catch (err) {
      console.error('[EditEncounter] Failed to save creatures after reorder:', err);
      setError('Failed to save creature order');
    }
    
    // Send lobby state after drag and drop reorder (if viewers are allowed)
    if (encounterState.viewersAllowed) {
      console.log('[EditEncounter] Sending lobby state after creature reorder');
      setTimeout(() => {
        sendLobbyState(newCreatures, encounterState.currentTurn, encounterState.turnNumber, encounterState.viewersAllowed);
      }, 100);
    }
  }, [setCreatureList, encounterId, encounterClient, setError, encounterState.viewersAllowed, encounterState.currentTurn, encounterState.turnNumber, sendLobbyState]);

  const rollAllInitiative = useCallback(() => {
    const rolled = creatures.map(c => {
      if (c.isPlayer) return c;
      const roll = Math.floor(Math.random() * 20) + 1;
      const modifier = c.initiativeModifier ?? 0;
      return { ...c, initiative: roll + modifier };
    });
    const sorted = [...rolled].sort((a, b) => b.initiative - a.initiative);
    handleCreaturesChange(sorted);
  }, [creatures, handleCreaturesChange]);

  const handleChooseParty = useCallback((party: import('../../api/partyClient').Party) => {
    setShowPartyPicker(false);
    setPartyMembers(party.members);

    if (encounterId) {
      encounterClient
        .setPartyLevels(encounterId, party.members.map((member) => member.level))
        .catch(() => setError('Failed to save party levels'));
    }

    // Remove existing player characters, keep non-player creatures
    const nonPlayers = creatures.filter((c) => !c.isPlayer);
    const partyCreatures = party.members.map((m) => ({
      isPlayer: true,
      displayName: m.name,
      creatureName: undefined,
      creatureId: undefined,
      initiative: 0,
      initiativeModifier: 0,
      maxHP: 0,
      currentHP: 0,
      ac: 0,
      isEditing: false,
    }));
    handleCreaturesChange([...nonPlayers, ...partyCreatures]);
  }, [creatures, handleCreaturesChange, encounterId, encounterClient, setError]);

  const loadEncounter = useCallback(async () => {
    if (!encounterId) return;

    try {
      const data = await encounterClient.getEncounter(encounterId);
      setEncounter(data);
      setNewName(data.displayName);
      setCreatureList(data.creatures.map(c => ({ ...c, isEditing: false })));
      setPartyMembers((data.partyLevels ?? []).map((level, index) => ({
        name: `Party Member ${index + 1}`,
        level: Math.max(1, Math.min(20, Math.floor(level))),
      })));
      setEncounterState({
        currentTurn: data.turnIndex ?? 0,
        turnNumber: data.turnCount ?? 1,
        viewersAllowed: data.viewersAllowed ?? false
      });
      turnStateLoaded.current = true;
      setError(null);
    } catch (err) {
      setError('Failed to load encounter');
      console.error('Error loading encounter:', err);
    } finally {
      setLoading(false);
    }
  }, [encounterId, encounterClient, setCreatureList, setError]);

  useEffect(() => {
    loadEncounter();
  }, [loadEncounter]);

  useEffect(() => {
    if (!encounterUiPrefsStorageKey || typeof window === 'undefined') {
      return;
    }

    try {
      const raw = window.localStorage.getItem(encounterUiPrefsStorageKey);
      if (!raw) {
        return;
      }

      const parsed = JSON.parse(raw) as { showDifficulty?: boolean };
      if (typeof parsed.showDifficulty === 'boolean') {
        setShowDifficulty(parsed.showDifficulty);
      }
    } catch {
      // Ignore malformed persisted encounter UI preferences.
    }
  }, [encounterUiPrefsStorageKey]);

  useEffect(() => {
    if (!encounterUiPrefsStorageKey || typeof window === 'undefined') {
      return;
    }

    const payload = {
      showDifficulty,
    };

    window.localStorage.setItem(encounterUiPrefsStorageKey, JSON.stringify(payload));
  }, [encounterUiPrefsStorageKey, showDifficulty]);

  // Fetch stat block for the current creature whenever the active turn changes
  useEffect(() => {
    const current = creatures[encounterState.currentTurn];
    if (!current?.creatureId || current.isPlayer) {
      setActiveStatBlock(null);
      return;
    }
    let cancelled = false;
    bestiaryClient.getCreatureById(current.creatureId)
      .then(detail => { if (!cancelled) setActiveStatBlock(detail.rawData); })
      .catch(() => { if (!cancelled) setActiveStatBlock(null); });
    return () => { cancelled = true; };
  }, [encounterState.currentTurn, creatures, bestiaryClient]);

  useEffect(() => {
    const monsterIds = Array.from(
      new Set(
        creatures
          .filter((creature) => !creature.isPlayer && !!creature.creatureId)
          .map((creature) => creature.creatureId as string)
      )
    );

    const missingIds = monsterIds.filter((id) => creatureCrById[id] === undefined);
    if (missingIds.length === 0) {
      return;
    }

    let cancelled = false;

    Promise.all(
      missingIds.map(async (id) => {
        try {
          const detail = await bestiaryClient.getCreatureById(id);
          const rawCr = detail.rawData.cr;

          if (typeof rawCr === 'string') {
            return [id, rawCr] as const;
          }

          if (rawCr && typeof rawCr.cr === 'string') {
            return [id, rawCr.cr] as const;
          }

          return [id, null] as const;
        } catch {
          return [id, null] as const;
        }
      })
    ).then((entries) => {
      if (cancelled) {
        return;
      }

      setCreatureCrById((previous) => {
        const next = { ...previous };

        for (const [id, cr] of entries) {
          if (next[id] === undefined) {
            next[id] = cr;
          }
        }

        return next;
      });
    });

    return () => {
      cancelled = true;
    };
  }, [creatures, bestiaryClient, creatureCrById]);

  const partyLevels = useMemo(() => partyMembers.map((member) => member.level), [partyMembers]);

  const monsterXpValues = useMemo(
    () =>
      creatures
        .filter((creature) => !creature.isPlayer)
        .map((creature) => {
          if (!creature.creatureId) {
            return 0;
          }

          return challengeRatingToXp(creatureCrById[creature.creatureId]);
        }),
    [creatures, creatureCrById]
  );

  const unknownMonsterCount = useMemo(
    () =>
      creatures.filter((creature) => {
        if (creature.isPlayer) {
          return false;
        }

        if (!creature.creatureId) {
          return true;
        }

        const cr = creatureCrById[creature.creatureId];
        return !cr || challengeRatingToXp(cr) === 0;
      }).length,
    [creatures, creatureCrById]
  );

  const encounterDifficulty = useMemo(() => {
    if (partyLevels.length === 0) {
      return null;
    }

    return calculateEncounterDifficulty(partyLevels, monsterXpValues);
  }, [partyLevels, monsterXpValues]);

  const handleNameEdit = async () => {
    if (!encounter || !encounterId || !newName.trim()) return;
    
    try {
      await encounterClient.renameEncounter(encounterId, newName.trim());
      setEncounter({ ...encounter, displayName: newName.trim() });
      setEditingName(false);
      setError(null);
    } catch (err) {
      setError('Failed to update encounter name');
      console.error('Error updating name:', err);
    }
  };

  const toggleViewersAllowed = async () => {
    if (!encounterId) return;
    const newValue = !encounterState.viewersAllowed;
    setEncounterState(prev => ({ ...prev, viewersAllowed: newValue }));
    try {
      await encounterClient.setViewersAllowed(encounterId, newValue);
      sendLobbyState(creatures, encounterState.currentTurn, encounterState.turnNumber, newValue);
    } catch (err) {
      console.error('[EditEncounter] Failed to persist viewersAllowed:', err);
    }
  };

  // Persist turn state and send lobby state when turn changes
  useEffect(() => {
    if (!turnStateLoaded.current) return;
    if (encounterId) {
      encounterClient.setTurnState(encounterId, encounterState.currentTurn, encounterState.turnNumber).catch(err =>
        console.error('[EditEncounter] Failed to persist turn state:', err)
      );
    }
    if (lobbyClient && creatures.length > 0 && encounterState.viewersAllowed) {
      sendLobbyState(creatures, encounterState.currentTurn, encounterState.turnNumber, encounterState.viewersAllowed);
    }
  }, [encounterState.currentTurn, encounterState.turnNumber]); // eslint-disable-line react-hooks/exhaustive-deps

  // Send lobby state when viewersAllowed changes
  useEffect(() => {
    if (lobbyClient && creatures.length > 0) {
      sendLobbyState(creatures, encounterState.currentTurn, encounterState.turnNumber, encounterState.viewersAllowed);
    }
  }, [encounterState.viewersAllowed]); // eslint-disable-line react-hooks/exhaustive-deps

  const nextTurn = () => {
    if (creatures.length === 0) return;

    setEncounterState(prev => {
      let next = prev.currentTurn + 1;
      let nextTurnNumber = prev.turnNumber;

      if (next >= creatures.length) {
        next = 0;
        nextTurnNumber++;
      }

      return { ...prev, currentTurn: next, turnNumber: nextTurnNumber };
    });
  };

  const prevTurn = () => {
    if (creatures.length === 0) return;

    setEncounterState(prev => {
      let pt = prev.currentTurn - 1;
      let ptNumber = prev.turnNumber;

      if (pt < 0) {
        pt = creatures.length - 1;
        ptNumber = Math.max(1, ptNumber - 1);
      }

      return { ...prev, currentTurn: pt, turnNumber: ptNumber };
    });
  };

  if (loading) {
    return <div className="edit-encounter-container">Loading encounter...</div>;
  }

  if (!encounter) {
    return <div className="edit-encounter-container">Encounter not found</div>;
  }

  return (
    <div className="edit-encounter-container">
      {isTaleSpire() && (
        <div className="talespire-badge">TaleSpire Connected</div>
      )}
      <div className="edit-encounter-header">
        <EncounterHeader
          displayName={encounter.displayName}
          editingName={editingName}
          newName={newName}
          onNameChange={setNewName}
          onStartEdit={() => setEditingName(true)}
          onSaveEdit={handleNameEdit}
          onCancelEdit={() => {
            setEditingName(false);
            setNewName(encounter.displayName);
          }}
          onKeyDown={(e) => {
            if (e.key === 'Enter') {
              handleNameEdit();
            } else if (e.key === 'Escape') {
              setEditingName(false);
              setNewName(encounter.displayName);
            }
          }}
        />
      </div>

      {error && (
        <div className="error-message" style={{ color: 'red', marginBottom: '1rem' }}>
          {error}
        </div>
      )}

      <div className="statblock-toggle-row">
        <button
          className="control-button secondary"
          onClick={() => setShowStatBlock(prev => !prev)}
        >
          {showStatBlock ? 'Hide Stat Block' : 'Show Stat Block'}
        </button>
      </div>

      <div className={`encounter-body ${showStatBlock ? '' : 'encounter-body-full'}`}>
        <div className="encounter-left">
          <EncounterStatus
            encounterState={encounterState}
            creatures={creatures}
            onToggleViewersAllowed={toggleViewersAllowed}
            onNextTurn={nextTurn}
            onPrevTurn={prevTurn}
            showDifficulty={showDifficulty}
            onToggleShowDifficulty={() => setShowDifficulty((previous) => !previous)}
            encounterDifficulty={encounterDifficulty}
            partyMemberCount={partyMembers.length}
            unknownMonsterCount={unknownMonsterCount}
          />

          <div className="creature-list">
            <div className="creature-list-header">
              <button 
                className="control-button secondary sort-initiative-button"
                onClick={sortByInitiative}
                title="Sort by Initiative (Descending)"
              >
                Sort by Initiative
              </button>
            </div>
            
            <EditableCreatureList
              creatures={creatures}
              highlightedCreatureIndex={encounterState.currentTurn}
              onCreaturesChange={handleCreaturesChange}
              onCreatureUpdate={updateCreature}
              onCreatureRemove={removeCreature}
              onRollAllInitiative={rollAllInitiative}
            />
          </div>

          <button className="add-creature-button" onClick={() => setShowBestiaryPicker(true)}>
            Add Creature
          </button>
          <button className="add-creature-button" onClick={() => setShowPartyPicker(true)}>
            Choose Party
          </button>
        </div>

        {showStatBlock && (
          <div className="encounter-statblock-panel">
            {activeStatBlock
              ? <CreatureStatBlock data={activeStatBlock} />
              : <div className="encounter-statblock-empty">No stat block available</div>}
          </div>
        )}
      </div>

      {showBestiaryPicker && (
        <BestiaryPicker
          onCreatureSelect={handleAddFromBestiary}
          onClose={() => setShowBestiaryPicker(false)}
        />
      )}
      {showPartyPicker && (
        <PartyPicker
          onPartySelect={handleChooseParty}
          onClose={() => setShowPartyPicker(false)}
        />
      )}
    </div>
  );
};

export default EditEncounter;
