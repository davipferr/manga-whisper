import { Injectable, signal, inject, computed } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, of, tap } from 'rxjs';
import { Chapter } from '../models/chapter.model';
import { ChaptersListResponseDto } from '../models/api.model';
import { environment } from '../../../environments/environment';
import { MangaTitleEnum } from '../../admin-panel/manga-title.enum';

const INITIAL_RETRY_SECONDS = 60;

@Injectable({
  providedIn: 'root'
})
export class ChapterService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/chapters`;

  private readonly recentChaptersData = signal<Chapter[]>([]);
  private readonly latestChapterData = signal<Chapter | null>(null);
  private readonly isLoading = signal<boolean>(false);
  private readonly error = signal<string | null>(null);
  private readonly retryDelay = signal<number>(INITIAL_RETRY_SECONDS);
  private readonly retryIn = signal<number>(INITIAL_RETRY_SECONDS);

  private readonly _currentPage = signal<number>(1);
  private readonly _totalPages = signal<number>(0);
  private readonly _totalChapters = signal<number>(0);

  readonly recentChapters = this.recentChaptersData.asReadonly();
  readonly latestChapter = this.latestChapterData.asReadonly();
  readonly nextChapterNumber = computed(() => (this.latestChapterData()?.number ?? 0) + 1);
  readonly loading = this.isLoading.asReadonly();
  readonly errorMessage = this.error.asReadonly();
  readonly retryAfter = this.retryIn.asReadonly();
  readonly currentPage = this._currentPage.asReadonly();
  readonly totalPages = this._totalPages.asReadonly();
  readonly totalChapters = this._totalChapters.asReadonly();

  private pageSize = 5;
  private retryTimer: ReturnType<typeof setTimeout> | null = null;

  setPageSize(size: number): void {
    this.pageSize = size;
  }

  fetchChapters(page: number = 1): void {
    if (this.retryTimer) {
      clearTimeout(this.retryTimer);
      this.retryTimer = null;
    }

    this.isLoading.set(true);
    this.error.set(null);

    const params = new HttpParams()
      .set('page', page)
      .set('pageSize', this.pageSize)
      .set('mangaTitle', MangaTitleEnum.OnePiece);

    this.http.get<ChaptersListResponseDto>(this.apiUrl, { params })
      .pipe(
        catchError(error => {
          console.error('Error fetching chapters:', error);

          return of({
            success: false,
            chapters: [],
            errorMessage: 'The chapter list could not be loaded.',
            totalChapters: 0
          } as ChaptersListResponseDto);
        }),
        tap(response => {
          this.isLoading.set(false);

          if (!response.success) {
            this.error.set(response.errorMessage || 'The chapter list could not be loaded.');
            this.scheduleRetry(page);
            return;
          }

          this.retryDelay.set(INITIAL_RETRY_SECONDS);

          const chapters = response.chapters.map(dto => ({
            number: dto.number,
            title: dto.title,
            url: dto.url,
            extractedAt: dto.extractedAt
          }));

          this.recentChaptersData.set(chapters);

          // Chapters come newest first, so the first item of page 1 is the latest release
          if (page === 1) {
            this.latestChapterData.set(chapters[0] ?? null);
          }

          this._currentPage.set(page);
          this._totalChapters.set(response.totalChapters);
          this._totalPages.set(Math.ceil(response.totalChapters / this.pageSize));
        })
      )
      .subscribe();
  }

  nextPage(): void {
    if (this._currentPage() < this._totalPages()) {
      this.fetchChapters(this._currentPage() + 1);
    }
  }

  previousPage(): void {
    if (this._currentPage() > 1) {
      this.fetchChapters(this._currentPage() - 1);
    }
  }

  private scheduleRetry(page: number): void {
    const seconds = this.retryDelay();
    this.retryIn.set(seconds);
    this.retryTimer = setTimeout(() => this.fetchChapters(page), seconds * 1000);
    this.retryDelay.set(Math.min(seconds * 2, 15 * 60));
  }
}
