export interface ChapterResponseDto {
  number: number;
  title: string;
  extractedAt: string;
}

export interface ChaptersListResponseDto {
  chapters: ChapterResponseDto[];
  success: boolean;
  errorMessage?: string;
}
