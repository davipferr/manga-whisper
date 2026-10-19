import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { filter, map } from 'rxjs';
import { AuthService } from '../../core/auth.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink],
  templateUrl: './header.html',
  styleUrl: './header.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  private readonly currentUrl = toSignal(
    this.router.events.pipe(
      filter((event) => event instanceof NavigationEnd),
      map((event) => event.urlAfterRedirects)
    ),
    { initialValue: this.router.url }
  );

  readonly isAdmin = this.authService.hasAdminRole;
  readonly adminName = computed(() => this.authService.userData()?.fullName ?? this.authService.userData()?.email);
  readonly inAdminPanel = computed(() => this.currentUrl().startsWith('/admin-panel'));
  readonly onLoginPage = computed(() => this.currentUrl().split('?')[0] === '/admin');

  logout(): void {
    this.authService.logout().subscribe({
      next: () => this.router.navigate(['/']),
      error: (err) => {
        console.error('Logout failed', err);
        this.router.navigate(['/']);
      },
    });
  }
}
