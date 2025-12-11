import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequestDto, LoginResponseDto } from '../../admin-login/admin-login.model';

@Injectable({
  providedIn: 'root'
})
export class AuthApiService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/auth`;

  login(request: LoginRequestDto): Observable<LoginResponseDto> {
    return this.http.post<LoginResponseDto>(`${this.apiUrl}/login`, request);
  }

  logout(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/logout`, {});
  }
}
