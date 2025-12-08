import { Component, ViewEncapsulation, inject } from "@angular/core";
import { UntypedFormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from "@angular/forms";

@Component({
  selector: 'admin-login',
  templateUrl: './admin-login.html',
  styleUrl: './admin-login.css',
  encapsulation: ViewEncapsulation.None,
  imports: [ReactiveFormsModule]
})
export class AdminLoginComponent {
  private readonly formBuilder = inject(UntypedFormBuilder);

  loginForm: UntypedFormGroup = this.formBuilder.group({
    username: ['', Validators.required],
    password: ['', Validators.required]
  });

  onLogin(): void {
    if (this.loginForm.valid) {
      const { username, password } = this.loginForm.value;
      console.log('Login attempt:', { username, password });

    }
  }
}
