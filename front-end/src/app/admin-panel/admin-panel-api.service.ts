import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { MangaCheckerListResponseDto, ManualCheckResponseDto } from './admin-panel.model';

@Injectable({
  providedIn: 'root'
})
export class AdminPanelApiService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  triggerManualChapterCheck(): Observable<ManualCheckResponseDto> {
    return this.http.post<ManualCheckResponseDto>(`${this.apiUrl}/chapters/check-now`, {});
  }

  triggerProcessAllAvailableChapters(checkerId: number): Observable<ManualCheckResponseDto> {
    return this.http.post<ManualCheckResponseDto>(`${this.apiUrl}/chapters/process-all-available/${checkerId}`, {});
  }

  getMangaCheckersByMangaTitle(mangaTitle: string): Observable<MangaCheckerListResponseDto> {
    return this.http.get<MangaCheckerListResponseDto>(`${this.apiUrl}/mangacheckers/get-checker-by-manga-title/${encodeURIComponent(mangaTitle)}`);
  }
}
