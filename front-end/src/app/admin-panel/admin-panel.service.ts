import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { RoleCheckRequestDto, RoleCheckResponseDto } from './admin-panel.model';

@Injectable({
  providedIn: 'root'
})
export class AdminPanelService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/auth/check-role`;

  checkRole(request: RoleCheckRequestDto): Observable<RoleCheckResponseDto> {
    return this.http.post<RoleCheckResponseDto>(this.apiUrl, request);
  }
}
