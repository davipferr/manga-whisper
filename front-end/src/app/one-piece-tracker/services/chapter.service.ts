import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, of, tap } from 'rxjs';
import { Chapter } from '../models/chapter.model';
import { ChaptersListResponseDto } from '../models/api.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ChapterService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/chapters`;

  private readonly recentChaptersData = signal<Chapter[]>([]);
  private readonly isLoading = signal<boolean>(false);
  private readonly error = signal<string | null>(null);
  private readonly retryDelay = signal<number>(60);
  private readonly pageSize = 5;

  private readonly _currentPage = signal<number>(1);
  private readonly _totalPages = signal<number>(1);

  readonly recentChapters = this.recentChaptersData.asReadonly();
  readonly loading = this.isLoading.asReadonly();
  readonly errorMessage = this.error.asReadonly();
  readonly currentPage = this._currentPage.asReadonly();
  readonly totalPages = this._totalPages.asReadonly();

  constructor() {
    this.fetchChapters();
  }

  get retryAfter(): number {
    return this.retryDelay();
  }

  private scheduleRetry(): void {
    setTimeout(() => {
      this.fetchChapters();
    }, this.retryDelay() * 1000);
  }

  private fetchChapters(page: number = 1): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.http.get<ChaptersListResponseDto>(`${this.apiUrl}?page=${page}&pageSize=${this.pageSize}`)
      .pipe(
        catchError(error => {
          console.error('Error fetching chapters:', error);

          this.error.set('Failed to fetch chapters.');

          this.retryDelay.update(delay => delay * 2);
          this.scheduleRetry();

          return of({
            success: false,
            chapters: [],
            errorMessage: 'Failed to fetch chapters.',
            totalChapters: 0
          } as ChaptersListResponseDto);
        }),
        tap(response => {
          this.isLoading.set(false);

          if (!response.success) {
            this.error.set(response.errorMessage || 'Unknown error occurred');
            return;
          }

          this.error.set(null);

          this.recentChaptersData.set(response.chapters.map(dto => ({
            number: dto.number,
            title: dto.title,
            extractedAt: dto.extractedAt
          })));

          this._currentPage.set(page);
          this._totalPages.set(Math.ceil(response.totalChapters / this.pageSize));
        })
      )
      .subscribe();
  }

  nextPage(): void {
    this.fetchChapters(this._currentPage() + 1);
  }

  previousPage(): void {
    if (this._currentPage() > 1) {
      this.fetchChapters(this._currentPage() - 1);
    }
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
