import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  // Don't add token to login endpoint
  if (req.url.includes('/auth/login') || !token) {
    return next(req);
  }

  const cloned = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });

  return next(cloned).pipe(
    catchError((error: unknown) => {
      // The token expired or was revoked: forget the session so the UI stops showing admin features
      if (error instanceof HttpErrorResponse && error.status === 401) {
        authService.clearAuthData();
      }
      return throwError(() => error);
    })
  );
};
