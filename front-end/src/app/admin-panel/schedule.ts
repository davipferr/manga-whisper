// Jobs are stored as standard 5-field cron expressions (minute hour day-of-month month day-of-week),
// evaluated in BRT by the API and the worker. The panel builds them from days + hours + minute so
// nobody has to type cron syntax.

export interface WeeklySchedule {
  // 0 = Sunday ... 6 = Saturday
  days: number[];
  hours: number[];
  minute: number;
}

export const WEEK_DAYS = [
  { value: 0, short: 'Sun', name: 'Sunday' },
  { value: 1, short: 'Mon', name: 'Monday' },
  { value: 2, short: 'Tue', name: 'Tuesday' },
  { value: 3, short: 'Wed', name: 'Wednesday' },
  { value: 4, short: 'Thu', name: 'Thursday' },
  { value: 5, short: 'Fri', name: 'Friday' },
  { value: 6, short: 'Sat', name: 'Saturday' },
] as const;

const sortedUnique = (values: number[]) => [...new Set(values)].sort((a, b) => a - b);

export function toCron(schedule: WeeklySchedule): string {
  return `${schedule.minute} ${sortedUnique(schedule.hours).join(',')} * * ${sortedUnique(schedule.days).join(',')}`;
}

/** Reads back an expression created by {@link toCron}; null for anything else */
export function fromCron(cron: string): WeeklySchedule | null {
  const match = /^(\d{1,2}) ([\d,]+) \* \* ([\d,]+)$/.exec(cron.trim());
  if (!match) {
    return null;
  }

  const minute = Number(match[1]);
  const hours = match[2].split(',').map(Number);
  const days = match[3].split(',').map(Number).map((d) => (d === 7 ? 0 : d));

  const valid = minute <= 59 && hours.every((h) => h <= 23) && days.every((d) => d <= 6);
  return valid ? { days: sortedUnique(days), hours: sortedUnique(hours), minute } : null;
}

/** "1, 2 and 3" */
function joinWords(words: string[]): string {
  return words.length <= 1 ? (words[0] ?? '') : `${words.slice(0, -1).join(', ')} and ${words[words.length - 1]}`;
}

/** Parses "12, 16,20" into [12, 16, 20]; null when any value is not an hour (0-23) */
export function parseHours(text: string): number[] | null {
  const parts = text.split(',').map((p) => p.trim()).filter((p) => p !== '');
  if (parts.length === 0 || parts.some((p) => !/^\d{1,2}$/.test(p) || Number(p) > 23)) {
    return null;
  }
  return sortedUnique(parts.map(Number));
}

/** "Thursday and Friday at 12:00, 16:00 and 20:00" */
export function describeCron(cron: string): string {
  const schedule = fromCron(cron);
  if (!schedule) {
    return cron;
  }

  const days =
    schedule.days.length === 7 ? 'Every day' : joinWords(schedule.days.map((d) => WEEK_DAYS[d].name));
  const minute = String(schedule.minute).padStart(2, '0');
  const times = joinWords(schedule.hours.map((h) => `${String(h).padStart(2, '0')}:${minute}`));

  return `${days} at ${times}`;
}
