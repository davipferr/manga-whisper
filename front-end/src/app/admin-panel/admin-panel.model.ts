// Values of MangaWhisper.Common.Enums.MangaCheckerStatus, sent as strings by the API
export type CheckerStatus = 'Stopped' | 'Idle' | 'Checking' | 'Error';

export interface AdminChecker {
  id: number;
  mangaId: number;
  mangaTitle: string;
  siteIdentifier: string;
  isActive: boolean;
  status: CheckerStatus;
  lastKnownChapter: number;
  lastCheckedAt: string | null;
  storedChapters: number;
}

export interface AdminCheckerListResponseDto {
  checkers: AdminChecker[];
  success: boolean;
  errorMessage?: string;
}

export interface AdminCheckerResultDto {
  checker?: AdminChecker;
  success: boolean;
  notFound: boolean;
  errorMessage?: string;
}

export interface UpdateCheckerRequestDto {
  isActive?: boolean;
  lastKnownChapter?: number;
}

// Values of MangaWhisper.Common.Enums.ScrapingJobType / JobRunStatus, sent as strings by the API
export type ScrapingJobType = 'CheckNext' | 'Backfill';
export type JobRunStatus = 'Pending' | 'Running' | 'Succeeded' | 'Failed' | 'Cancelled';

export interface ScrapingJob {
  id: number;
  mangaCheckerId: number;
  mangaTitle: string;
  type: ScrapingJobType;
  cronExpression: string;
  timeZone: string;
  isEnabled: boolean;
  nextRunAt: string | null;
  lastRunAt: string | null;
  createdAt: string;
}

export interface ScrapingJobListResponseDto {
  jobs: ScrapingJob[];
  success: boolean;
  errorMessage?: string;
}

export interface ScrapingJobResultDto {
  job?: ScrapingJob;
  success: boolean;
  notFound: boolean;
  invalid: boolean;
  errorMessage?: string;
}

export interface SaveScrapingJobRequestDto {
  mangaCheckerId: number;
  type: ScrapingJobType;
  cronExpression: string;
  isEnabled: boolean;
}

export interface JobRun {
  id: number;
  mangaCheckerId: number;
  mangaTitle: string;
  scrapingJobId: number | null;
  type: ScrapingJobType;
  status: JobRunStatus;
  requestedAt: string;
  startedAt: string | null;
  finishedAt: string | null;
  chaptersFound: number;
  errorMessage: string | null;
}

export interface JobRunListResponseDto {
  runs: JobRun[];
  success: boolean;
  errorMessage?: string;
}

export interface JobRunResultDto {
  run?: JobRun;
  success: boolean;
  notFound: boolean;
  conflict: boolean;
  errorMessage?: string;
}

export interface WorkerStatusResponseDto {
  isOnline: boolean;
  lastSeenAt: string | null;
  startedAt: string | null;
  success: boolean;
  errorMessage?: string;
}
