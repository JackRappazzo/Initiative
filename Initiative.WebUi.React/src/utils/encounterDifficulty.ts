export type FormulaType = 'standard' | 'fleeMortals';

export type StandardDifficultyBand = 'Trivial' | 'Low' | 'Moderate' | 'High' | 'Extreme';

export type FleeMortalsDifficultyBand = 'Easy' | 'Standard' | 'Hard';

export type EncounterDifficultyBand = StandardDifficultyBand | FleeMortalsDifficultyBand;

export interface EncounterDifficultySummary {
  totalMonsterXp: number;
  thresholds: {
    low: number;
    moderate: number;
    high: number;
    extreme: number;
  };
  difficulty: EncounterDifficultyBand;
  unit: 'XP' | 'CR';
  formulaType: FormulaType;
}

const CR_TO_XP: Record<string, number> = {
  '0': 10,
  '1/8': 25,
  '1/4': 50,
  '1/2': 100,
  '1': 200,
  '2': 450,
  '3': 700,
  '4': 1100,
  '5': 1800,
  '6': 2300,
  '7': 2900,
  '8': 3900,
  '9': 5000,
  '10': 5900,
  '11': 7200,
  '12': 8400,
  '13': 10000,
  '14': 11500,
  '15': 13000,
  '16': 15000,
  '17': 18000,
  '18': 20000,
  '19': 22000,
  '20': 25000,
  '21': 33000,
  '22': 41000,
  '23': 50000,
  '24': 62000,
  '25': 75000,
  '26': 90000,
  '27': 105000,
  '28': 120000,
  '29': 135000,
  '30': 155000,
};

type PerLevelBudgets = {
  low: number;
  moderate: number;
  high: number;
  extreme: number;
};

// Flee, Mortals! per-character CR budgets, from the Encounter CR per Character table.
const FLEE_MORTALS_BUDGETS: Record<number, { easy: number; standard: number; hard: number }> = {
  1:  { easy: 1/8, standard: 1/8, hard: 1/4 },
  2:  { easy: 1/8, standard: 1/4, hard: 1/2 },
  3:  { easy: 1/4, standard: 1/2, hard: 3/4 },
  4:  { easy: 1/2, standard: 3/4, hard: 1   },
  5:  { easy: 1,   standard: 1.5, hard: 2.5 },
  6:  { easy: 1.5, standard: 2,   hard: 3   },
  7:  { easy: 2,   standard: 2.5, hard: 3.5 },
  8:  { easy: 2.5, standard: 3,   hard: 4   },
  9:  { easy: 3,   standard: 3.5, hard: 4.5 },
  10: { easy: 3.5, standard: 4,   hard: 6   },
  11: { easy: 4,   standard: 4.5, hard: 5.5 },
  12: { easy: 4.5, standard: 5,   hard: 6   },
  13: { easy: 5,   standard: 5.5, hard: 6.5 },
  14: { easy: 5.5, standard: 6,   hard: 7   },
  15: { easy: 6,   standard: 6.5, hard: 7.5 },
  16: { easy: 6.5, standard: 7,   hard: 8   },
  17: { easy: 7,   standard: 7.5, hard: 8.5 },
  18: { easy: 7.5, standard: 8,   hard: 9   },
  19: { easy: 8,   standard: 8.5, hard: 9.5 },
  20: { easy: 8.5, standard: 9,   hard: 10  },
};
const LEVEL_BUDGETS: Record<number, PerLevelBudgets> = {
  1: { low: 25, moderate: 50, high: 75, extreme: 100 },
  2: { low: 50, moderate: 100, high: 150, extreme: 200 },
  3: { low: 75, moderate: 150, high: 225, extreme: 400 },
  4: { low: 125, moderate: 250, high: 375, extreme: 500 },
  5: { low: 250, moderate: 500, high: 750, extreme: 1100 },
  6: { low: 300, moderate: 600, high: 900, extreme: 1400 },
  7: { low: 350, moderate: 750, high: 1100, extreme: 1700 },
  8: { low: 450, moderate: 900, high: 1400, extreme: 2100 },
  9: { low: 550, moderate: 1100, high: 1600, extreme: 2400 },
  10: { low: 600, moderate: 1200, high: 1900, extreme: 2800 },
  11: { low: 800, moderate: 1600, high: 2400, extreme: 3600 },
  12: { low: 1000, moderate: 2000, high: 3000, extreme: 4500 },
  13: { low: 1100, moderate: 2200, high: 3400, extreme: 5100 },
  14: { low: 1250, moderate: 2500, high: 3800, extreme: 5700 },
  15: { low: 1400, moderate: 2800, high: 4300, extreme: 6400 },
  16: { low: 1600, moderate: 3200, high: 4800, extreme: 7200 },
  17: { low: 2000, moderate: 3900, high: 5900, extreme: 8800 },
  18: { low: 2100, moderate: 4200, high: 6300, extreme: 9500 },
  19: { low: 2400, moderate: 4900, high: 7300, extreme: 10900 },
  20: { low: 2800, moderate: 5700, high: 8500, extreme: 12700 },
};

export function challengeRatingToXp(cr?: string | null): number {
  if (!cr) {
    return 0;
  }

  return CR_TO_XP[cr.trim()] ?? 0;
}

export function challengeRatingToNumber(cr?: string | null): number {
  if (!cr) {
    return 0;
  }

  const trimmed = cr.trim();

  if (trimmed.includes('/')) {
    const [numerator, denominator] = trimmed.split('/');
    const num = Number(numerator);
    const den = Number(denominator);
    if (Number.isFinite(num) && Number.isFinite(den) && den !== 0) {
      return num / den;
    }
    return 0;
  }

  const parsed = Number(trimmed);
  return Number.isFinite(parsed) ? parsed : 0;
}

export function calculateEncounterDifficulty(partyLevels: number[], monsterXpValues: number[]): EncounterDifficultySummary {
  const thresholds = partyLevels.reduce(
    (totals, level) => {
      const clampedLevel = Math.max(1, Math.min(20, Math.floor(level)));
      const budget = LEVEL_BUDGETS[clampedLevel];

      totals.low += budget.low;
      totals.moderate += budget.moderate;
      totals.high += budget.high;
      totals.extreme += budget.extreme;

      return totals;
    },
    { low: 0, moderate: 0, high: 0, extreme: 0 }
  );

  const totalMonsterXp = monsterXpValues.reduce((sum, xp) => sum + Math.max(0, xp), 0);

  let difficulty: StandardDifficultyBand = 'Trivial';

  if (totalMonsterXp >= thresholds.extreme) {
    difficulty = 'Extreme';
  } else if (totalMonsterXp >= thresholds.high) {
    difficulty = 'High';
  } else if (totalMonsterXp >= thresholds.moderate) {
    difficulty = 'Moderate';
  } else if (totalMonsterXp >= thresholds.low) {
    difficulty = 'Low';
  }

  return {
    totalMonsterXp,
    thresholds,
    difficulty,
    unit: 'XP',
    formulaType: 'standard',
  };
}

export function calculateFleeMortalsEncounterDifficulty(partyLevels: number[], monsterCrValues: number[]): EncounterDifficultySummary {
  const thresholds = partyLevels.reduce(
    (totals, level) => {
      const clampedLevel = Math.max(1, Math.min(20, Math.floor(level)));
      const budget = FLEE_MORTALS_BUDGETS[clampedLevel];

      totals.low += budget.easy;
      totals.moderate += budget.standard;
      totals.high += budget.hard;

      return totals;
    },
    { low: 0, moderate: 0, high: 0, extreme: 0 }
  );

  thresholds.extreme = thresholds.high;

  const totalMonsterCr = monsterCrValues.reduce((sum, cr) => sum + cr, 0);

  let difficulty: FleeMortalsDifficultyBand = 'Easy';

  if (totalMonsterCr >= thresholds.high) {
    difficulty = 'Hard';
  } else if (totalMonsterCr >= thresholds.moderate) {
    difficulty = 'Standard';
  } else if (totalMonsterCr >= thresholds.low) {
    difficulty = 'Easy';
  }

  return {
    totalMonsterXp: totalMonsterCr,
    thresholds,
    difficulty,
    unit: 'CR',
    formulaType: 'fleeMortals',
  };
}
