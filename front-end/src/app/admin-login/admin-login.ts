import { Component, ViewEncapsulation, inject } from "@angular/core";
import { UntypedFormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from "@angular/forms";
import { AdminLoginService } from "./admin-login.service";
import { LoginRequestDto, LoginResponseDto, UnauthorizedResponseDto  } from './admin-login.model';
import { Router } from "@angular/router";
import { AuthService } from "../shared/core/auth.service";

@Component({
  selector: 'admin-login',
  templateUrl: './admin-login.html',
  styleUrl: './admin-login.css',
  encapsulation: ViewEncapsulation.None,
  imports: [ReactiveFormsModule]
})
export class AdminLoginComponent {
  private readonly formBuilder = inject(UntypedFormBuilder);
  private readonly adminLoginService = inject(AdminLoginService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  loginForm: UntypedFormGroup = this.formBuilder.group({
    email: ['', Validators.required],
    password: ['', Validators.required]
  });

  onLogin(): void {
    if (!this.loginForm.valid) {
      return;
    }

    const { email, password } = this.loginForm.value;

    const loginRequest: LoginRequestDto = {
      email,
      password
    };

    this.adminLoginService.login(loginRequest).subscribe({
      next: (response: LoginResponseDto) => {
        this.authService.saveAuthData(response);
        this.router.navigate(['/admin-panel']);
      },
      error: (error: UnauthorizedResponseDto) => {
        console.error('Login failed:', error.message);
      }
    });
  }
}
