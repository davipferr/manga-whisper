import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, of, tap } from 'rxjs';
import { Chapter } from '../models/chapter.model';
import { ChaptersListResponseDto } from '../models/api.model';

@Injectable({
  providedIn: 'root'
})
export class ChapterService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5065/api/chapters';

  private readonly recentChaptersData = signal<Chapter[]>([]);
  private readonly isLoading = signal<boolean>(false);
  private readonly error = signal<string | null>(null);

  readonly recentChapters = this.recentChaptersData.asReadonly();
  readonly loading = this.isLoading.asReadonly();
  readonly errorMessage = this.error.asReadonly();

  constructor() {
    this.loadChaptersFromApi();
  }

  private loadChaptersFromApi(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.http.get<ChaptersListResponseDto>(this.apiUrl)
      .pipe(
        catchError(error => {
          console.error('Error fetching chapters:', error);

          return of({
            success: false,
            chapters: [],
            errorMessage: 'There was an error fetching the chapters.'
          } as ChaptersListResponseDto);
        }),
        tap(response => {
          this.isLoading.set(false);

          if (!response.success) {
            this.error.set(response.errorMessage || 'Unknown error occurred');
            return;
          }

          this.error.set(null);

          const chapters: Chapter[] = response.chapters.map(dto => ({
            number: dto.number,
            title: dto.title,
            extractedAt: dto.extractedAt
          }));

          this.recentChaptersData.set(chapters);
        })
      )
      .subscribe();
  }

  reloadChapters(): void {
    this.loadChaptersFromApi();
  }

  getLatestChapter(): Chapter {
    const chapters = this.recentChaptersData();

    if (chapters.length > 0) {
      return chapters[0];
    }

    return {
      number: 0,
      title: 'No latest chapter',
      extractedAt: 'N/A',
    };
  }

  getNextChapter(): Chapter {
    const latest = this.getLatestChapter();

    return {
      number: latest.number + 1,
      title: 'To Be Announced',
      extractedAt: 'TBA',
    };
  }

  addChapter(chapter: Chapter): void {
    this.recentChaptersData.update(chapters => [chapter, ...chapters]);
  }

  updateChapter(number: number, updates: Partial<Chapter>): void {
    this.recentChaptersData.update(chapters =>
      chapters.map(chapter =>
        chapter.number === number ? { ...chapter, ...updates } : chapter
      )
    );
  }
}
