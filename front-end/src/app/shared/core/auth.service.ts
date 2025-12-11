import { Injectable, signal, computed, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginResponseDto } from '../../admin-login/admin-login.model';
import { AuthApiService } from './auth-api.service';

interface DecodedToken {
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress': string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role'?: string | string[];
  sub: string;
  jti: string;
  exp: number;
  iss: string;
  aud: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly authApi = inject(AuthApiService);

  private readonly tokenKey = 'adminToken';
  private readonly userDataKey = 'userData';

  private readonly _userData = signal<LoginResponseDto | null>(this.loadUserData());
  readonly userData = this._userData.asReadonly();
  readonly isAuthenticated = computed(() => this._userData() !== null && this.isTokenValid());
  readonly userRoles = computed(() => this._userData()?.roles ?? []);
  readonly hasAdminRole = computed(() => this.userRoles().some((role: string) => role.toUpperCase() === 'ADMIN'));

  saveAuthData(loginResponse: LoginResponseDto): void {
    localStorage.setItem(this.tokenKey, loginResponse.token);
    localStorage.setItem(this.userDataKey, JSON.stringify(loginResponse));
    this._userData.set(loginResponse);
  }

  clearAuthData(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userDataKey);
    this._userData.set(null);
  }

  logout(): Observable<{ message: string }> {
    this.clearAuthData();

    return this.authApi.logout();
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private loadUserData(): LoginResponseDto | null {
    const userData = localStorage.getItem(this.userDataKey);
    if (!userData) {
      return null;
    }

    try {
      const parsed = JSON.parse(userData) as LoginResponseDto;
      // Verify token is still valid
      if (!this.isTokenValid()) {
        this.clearAuthData();
        return null;
      }
      return parsed;
    } catch {
      return null;
    }
  }

  private isTokenValid(): boolean {
    const token = this.getToken();
    if (!token) {
      return false;
    }

    try {
      const decoded = this.decodeToken(token);
      const currentTime = Math.floor(Date.now() / 1000);
      return decoded.exp > currentTime;
    } catch {
      return false;
    }
  }

  private decodeToken(token: string): DecodedToken {
    const parts = token.split('.');
    if (parts.length !== 3) {
      throw new Error('Invalid token format');
    }

    const payload = parts[1];
    const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
    return JSON.parse(decoded);
  }

  hasRole(roleName: string): boolean {
    return this.userRoles().some((role: string) => role.toUpperCase() === roleName.toUpperCase());
  }
}
