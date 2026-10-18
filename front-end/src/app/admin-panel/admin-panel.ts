import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from '../shared/core/auth.service';
import { AdminPanelApiService } from './admin-panel-api.service';
import { AdminChecker } from './admin-panel.model';
import { CheckerCardComponent } from './components/checker-card/checker-card';
import { AdminChaptersComponent } from './components/admin-chapters/admin-chapters';

@Component({
  selector: 'admin-panel',
  templateUrl: './admin-panel.html',
  styleUrl: './admin-panel.css',
  imports: [CheckerCardComponent, AdminChaptersComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminPanelComponent implements OnInit {
  private readonly api = inject(AdminPanelApiService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly checkers = signal<AdminChecker[]>([]);
  readonly loading = signal(false);
  readonly loadError = signal<string | null>(null);

  readonly mangaTitles = computed(() => [...new Set(this.checkers().map((c) => c.mangaTitle))]);
  private readonly pickedManga = signal<string | null>(null);
  readonly selectedManga = computed(() => this.pickedManga() ?? this.mangaTitles()[0] ?? '');

  ngOnInit(): void {
    this.loadCheckers();
  }

  loadCheckers(): void {
    this.loading.set(true);
    this.loadError.set(null);

    this.api.getCheckers().subscribe({
      next: (response) => {
        this.loading.set(false);
        this.checkers.set(response.checkers);
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);

        if (error.status === 401 || error.status === 403) {
          this.onUnauthorized();
          return;
        }

        this.loadError.set(
          error.status === 0
            ? 'The API could not be reached. Check that it is running, then try again.'
            : (error.error?.errorMessage ?? 'The checkers could not be loaded.')
        );
      },
    });
  }

  onCheckerUpdated(updated: AdminChecker): void {
    this.checkers.update((list) => list.map((c) => (c.id === updated.id ? updated : c)));
  }

  onUnauthorized(): void {
    this.authService.clearAuthData();
    this.router.navigate(['/admin']);
  }

  selectManga(event: Event): void {
    this.pickedManga.set((event.target as HTMLSelectElement).value);
  }
}
