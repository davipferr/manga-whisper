import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { catchError, Observable, of } from 'rxjs';
import { LoginRequestDto, AuthenticationResponse, LoginResponseDto } from './admin-login.model';

@Injectable({
  providedIn: 'root'
})
export class AdminLoginService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/auth/login`;

  login(request: LoginRequestDto): Observable<LoginResponseDto> {
    return this.http.post<LoginResponseDto>(this.apiUrl, request);
  }
}
