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
