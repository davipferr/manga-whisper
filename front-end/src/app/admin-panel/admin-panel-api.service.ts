import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ChaptersListResponseDto } from '../one-piece-tracker/models/api.model';
import { AdminCheckerListResponseDto, AdminCheckerResultDto, UpdateCheckerRequestDto } from './admin-panel.model';

@Injectable({
  providedIn: 'root'
})
export class AdminPanelApiService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  getCheckers(): Observable<AdminCheckerListResponseDto> {
    return this.http.get<AdminCheckerListResponseDto>(`${this.apiUrl}/admin/checkers`);
  }

  updateChecker(id: number, request: UpdateCheckerRequestDto): Observable<AdminCheckerResultDto> {
    return this.http.patch<AdminCheckerResultDto>(`${this.apiUrl}/admin/checkers/${id}`, request);
  }

  getChapters(mangaTitle: string, page: number, pageSize: number): Observable<ChaptersListResponseDto> {
    const params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize)
      .set('mangaTitle', mangaTitle);

    return this.http.get<ChaptersListResponseDto>(`${this.apiUrl}/chapters`, { params });
  }
}
