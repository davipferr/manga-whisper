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
