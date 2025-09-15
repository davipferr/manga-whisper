import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, of, tap } from 'rxjs';
import { Chapter, ChapterInfo } from '../models/chapter.model';
import { ChaptersListResponseDto } from '../models/api.model';

@Injectable({
  providedIn: 'root'
})
export class ChapterService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5000/api/chapters';
  
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
          console.error('Error loading chapters:', error);
          this.error.set('Failed to load chapters from server');
          // Fallback to hardcoded data
          return of({
            success: false,
            chapters: [
              { number: 1097, title: 'Ginny', date: '19/11/2023' },
              { number: 1096, title: 'Kumachi', date: '12/11/2023' },
              { number: 1095, title: 'A World Not Worth Living In', date: '05/11/2023' }
            ],
            errorMessage: 'Using fallback data'
          } as ChaptersListResponseDto);
        }),
        tap(response => {
          this.isLoading.set(false);
          if (response.success) {
            this.error.set(null);
            const chapters: Chapter[] = response.chapters.map(dto => ({
              number: dto.number,
              title: dto.title,
              date: dto.date
            }));
            this.recentChaptersData.set(chapters);
          } else {
            // Even if success is false, we still want to show the fallback chapters
            const chapters: Chapter[] = response.chapters.map(dto => ({
              number: dto.number,
              title: dto.title,
              date: dto.date
            }));
            this.recentChaptersData.set(chapters);
            this.error.set(response.errorMessage || 'Unknown error occurred');
          }
        })
      )
      .subscribe();
  }

  reloadChapters(): void {
    this.loadChaptersFromApi();
  }

  getLatestChapter(): ChapterInfo {
    const chapters = this.recentChaptersData();
    if (chapters.length > 0) {
      const latest = chapters[0];
      return {
        number: latest.number,
        title: latest.title,
        date: latest.date,
      };
    }
    return {
      number: 1098,
      title: 'Bonney\'s Birth',
      date: '26/11/2023',
    };
  }

  getNextChapter(): ChapterInfo {
    const latest = this.getLatestChapter();
    return {
      number: latest.number + 1,
      title: 'To Be Announced',
      date: 'TBA',
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
