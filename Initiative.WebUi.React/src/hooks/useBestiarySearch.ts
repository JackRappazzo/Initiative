import { useEffect, useState, useMemo, useRef } from 'react';
import { BestiaryClient, BestiaryListItem, CreatureListItem, CreatureSortBy } from '../api/bestiaryClient';

const PAGE_SIZE = 20;
const DEBOUNCE_MS = 300;

export interface SortState { col: CreatureSortBy; desc: boolean; }

export interface UseBestiarySearchResult {
  // Sidebar state
  bestiaries: BestiaryListItem[];
  selectedIds: Set<string>;
  bestiariesLoading: boolean;
  bestiariesError: string | null;
  toggleBestiary: (id: string) => void;
  selectAll: () => void;
  clearAll: () => void;
  refreshBestiaries: () => void;

  // Search / sort state
  nameInput: string;
  sort: SortState;
  handleNameInputChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  handleSortClick: (col: CreatureSortBy) => void;
  sortIndicator: (col: CreatureSortBy) => string;

  // Creature results
  creatures: CreatureListItem[];
  totalCount: number;
  totalPages: number;
  currentPage: number;
  creaturesLoading: boolean;
  creaturesError: string | null;
  handlePageChange: (page: number) => void;
}

export const useBestiarySearch = (): UseBestiarySearchResult => {
  const bestiaryClient = useMemo(() => new BestiaryClient(), []);

  // Sidebar / filter state
  const [bestiaries, setBestiaries] = useState<BestiaryListItem[]>([]);
  const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set());
  const [bestiariesLoading, setBestiariesLoading] = useState(true);
  const [bestiariesError, setBestiariesError] = useState<string | null>(null);
  const [bestiaryRefreshToken, setBestiaryRefreshToken] = useState(0);

  // Search / creature state
  const [nameInput, setNameInput] = useState('');
  const [nameFilter, setNameFilter] = useState('');
  const [sort, setSort] = useState<SortState>({ col: 'Name', desc: false });
  const [creatures, setCreatures] = useState<CreatureListItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [creaturesLoading, setCreaturesLoading] = useState(false);
  const [creaturesError, setCreaturesError] = useState<string | null>(null);

  const debounceTimer = useRef<ReturnType<typeof setTimeout> | null>(null);

  // Load bestiaries on mount and on refresh
  useEffect(() => {
    setBestiariesLoading(true);
    bestiaryClient.getAvailableBestiaries()
      .then((data) => {
        setBestiaries(data);
        setSelectedIds(new Set(data.map((b) => b.id)));
      })
      .catch(() => setBestiariesError('Failed to load bestiaries'))
      .finally(() => setBestiariesLoading(false));
  }, [bestiaryClient, bestiaryRefreshToken]);

  // Load creatures whenever filters or page changes
  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));

  useEffect(() => {
    if (bestiariesLoading) return;
    let cancelled = false;
    const run = async () => {
      setCreaturesLoading(true);
      setCreaturesError(null);
      try {
        const skip = (currentPage - 1) * PAGE_SIZE;
        const idArray = Array.from(selectedIds);
        const params = {
          bestiaryIds: idArray,
          name: nameFilter || undefined,
          sortBy: sort.col,
          sortDescending: sort.desc ? true : undefined,
          pageSize: PAGE_SIZE,
          skip,
        };
        const { creatures: results, totalCount: count } = await bestiaryClient.searchCreatures(params);
        if (!cancelled) { setCreatures(results); setTotalCount(count); }
      } catch {
        if (!cancelled) setCreaturesError('Failed to load creatures');
      } finally {
        if (!cancelled) setCreaturesLoading(false);
      }
    };
    run();
    return () => { cancelled = true; };
  }, [bestiariesLoading, selectedIds, nameFilter, sort, currentPage, bestiaryClient]);

  // Debounce name input → nameFilter
  const handleNameInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setNameInput(value);
    if (debounceTimer.current) clearTimeout(debounceTimer.current);
    debounceTimer.current = setTimeout(() => {
      setNameFilter(value);
      setCurrentPage(1);
    }, DEBOUNCE_MS);
  };

  // Sort handler
  const handleSortClick = (col: CreatureSortBy) => {
    setSort((prev) => ({ col, desc: prev.col === col ? !prev.desc : false }));
    if (currentPage !== 1) setCurrentPage(1);
  };

  const sortIndicator = (col: CreatureSortBy): string => {
    if (sort.col !== col) return '';
    return sort.desc ? ' ▼' : ' ▲';
  };

  // Sidebar handlers
  const toggleBestiary = (id: string) => {
    setSelectedIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) { next.delete(id); } else { next.add(id); }
      return next;
    });
    setCurrentPage(1);
  };

  const selectAll = () => {
    setSelectedIds(new Set(bestiaries.map((b) => b.id)));
    setCurrentPage(1);
  };

  const clearAll = () => {
    setSelectedIds(new Set());
    setCurrentPage(1);
  };

  const refreshBestiaries = () => {
    setBestiaryRefreshToken(t => t + 1);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  return {
    bestiaries,
    selectedIds,
    bestiariesLoading,
    bestiariesError,
    toggleBestiary,
    selectAll,
    clearAll,
    refreshBestiaries,
    nameInput,
    sort,
    handleNameInputChange,
    handleSortClick,
    sortIndicator,
    creatures,
    totalCount,
    totalPages,
    currentPage,
    creaturesLoading,
    creaturesError,
    handlePageChange,
  };
};
