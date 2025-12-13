export interface ChapterResponseDto {
  mangaId: number;
  number: number;
  title: string;
  url: string
  extractedAt: string;
}

export interface ChaptersListResponseDto {
  chapters: ChapterResponseDto[];
  success: boolean;
  errorMessage?: string;
  totalChapters: number;
}
