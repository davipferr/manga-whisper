import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ManualCheckResponseDto } from './admin-panel.model';

@Injectable({
  providedIn: 'root'
})
export class AdminPanelService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  triggerManualChapterCheck(): Observable<ManualCheckResponseDto> {
    return this.http.post<ManualCheckResponseDto>(`${this.apiUrl}/chapters/check-now`, {});
  }
}
