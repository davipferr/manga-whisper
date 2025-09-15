export interface ChapterResponseDto {
  number: number;
  title: string;
  date: string;
}

export interface ChaptersListResponseDto {
  chapters: ChapterResponseDto[];
  success: boolean;
  errorMessage?: string;
}