import { Component, signal, computed } from '@angular/core';
import { inject } from "@angular/core";
import { NavigationEnd, Router } from '@angular/router';
import { AuthService } from "../../core/auth.service";
import { startWith } from 'rxjs/internal/operators/startWith';
import { map } from 'rxjs/internal/operators/map';
import { filter } from 'rxjs/internal/operators/filter';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-header',
  templateUrl: './header.html',
  styleUrls: ['./header.css'],
})
export class HeaderComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly title = signal<string>('Manga Whisper');
  readonly isAdmin = this.authService.hasAdminRole;
  readonly adminName = computed(() => this.authService?.userData()?.fullName ?? 'Admin');

  private readonly currentUrl = toSignal(
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => this.router.url),
      startWith(this.router.url)
    ),
    { initialValue: this.router.url }
  );

  readonly currentRouteIsAdminPanel = computed<boolean>(() => this.currentUrl() === '/admin-panel');

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        console.log('Logged out successfully');
        this.router.navigate(['/admin']);
      },
      error: (err) => console.error('Logout failed', err)
    });
  }

  goToLogin(): void {
    this.router.navigate(['/admin']);
  }

  goToHome(): void {
    this.router.navigate(['/']);
  }

  goToPanel(): void {
    this.router.navigate(['/admin-panel']);
  }
}
