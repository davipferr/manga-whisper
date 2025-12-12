import { ChapterResponseDto } from "../one-piece-tracker/models/api.model";

export interface RoleCheckRequestDto {
  email: string;
  roleName: string;
}

export interface RoleCheckResponseDto {
  hasRole: boolean;
  message: string;
}

export interface ManualCheckResponseDto {
  success: boolean;
  message?: string;
  errorMessage?: string;
  newChapters: ChapterResponseDto[];
}

export interface StatusMessage {
  message: string;
  isError?: boolean;
  isWarning?: boolean;
}

export interface MangaCheckerResponseDto {
  id: number;
  mangaId: number;
  checkerUrl: string;
  chapterSelector: string;
  titleSelector: string;
  urlSelector: string;
  success: boolean;
  errorMessage?: string;
}

export interface MangaCheckerListResponseDto {
  mangaCheckers: MangaCheckerResponseDto[];
  success: boolean;
  errorMessage?: string;
}
