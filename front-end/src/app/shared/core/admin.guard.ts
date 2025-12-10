import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { AuthService } from './auth.service';

export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    router.navigate(['/admin']);
    return false;
  }

  if (!authService.hasAdminRole()) {
    router.navigate(['']);
    return false;
  }

  return true;
};
