import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminLoginService } from './admin-login.service';
import { LoginResponseDto } from './admin-login.model';
import { AuthService } from '../shared/core/auth.service';

@Component({
  selector: 'admin-login',
  templateUrl: './admin-login.html',
  styleUrl: './admin-login.css',
  imports: [ReactiveFormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminLoginComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly adminLoginService = inject(AdminLoginService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly submitting = signal(false);
  readonly loginError = signal<string | null>(null);
  private readonly submitted = signal(false);

  readonly loginForm = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    rememberMe: [true],
  });

  constructor() {
    if (this.authService.isAuthenticated() && this.authService.hasAdminRole()) {
      this.router.navigate(['/admin-panel']);
    }
  }

  showError(field: 'email' | 'password'): boolean {
    const control = this.loginForm.controls[field];
    return control.invalid && (control.touched || this.submitted());
  }

  onLogin(): void {
    this.submitted.set(true);
    this.loginError.set(null);

    if (this.loginForm.invalid) {
      return;
    }

    this.submitting.set(true);

    this.adminLoginService.login(this.loginForm.getRawValue()).subscribe({
      next: (response: LoginResponseDto) => {
        this.authService.saveAuthData(response);
        this.router.navigate(['/']);
      },
      error: (error: HttpErrorResponse) => {
        this.submitting.set(false);
        this.loginError.set(
          error.status === 0
            ? 'The server could not be reached. Check that the API is running and try again.'
            : (error.error?.message ?? 'Sign-in failed. Try again.')
        );
      },
    });
  }
}
