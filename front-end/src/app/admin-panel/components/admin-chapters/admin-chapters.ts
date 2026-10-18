import { ChangeDetectionStrategy, Component, effect, inject, input, signal, untracked } from '@angular/core';
import { AdminPanelApiService } from '../../admin-panel-api.service';
import { ChapterResponseDto } from '../../../one-piece-tracker/models/api.model';

const PAGE_SIZE = 10;

@Component({
  selector: 'app-admin-chapters',
  templateUrl: './admin-chapters.html',
  styleUrl: './admin-chapters.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminChaptersComponent {
  private readonly api = inject(AdminPanelApiService);

  mangaTitle = input.required<string>();

  readonly chapters = signal<ChapterResponseDto[]>([]);
  readonly page = signal(1);
  readonly totalPages = signal(0);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  constructor() {
    // Start from the first page whenever another manga is picked
    effect(() => {
      this.mangaTitle();
      untracked(() => this.load(1));
    });
  }

  load(page: number): void {
    if (!this.mangaTitle()) {
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    this.api.getChapters(this.mangaTitle(), page, PAGE_SIZE).subscribe({
      next: (response) => {
        this.loading.set(false);
        this.chapters.set(response.chapters);
        this.page.set(page);
        this.totalPages.set(Math.ceil(response.totalChapters / PAGE_SIZE));
      },
      error: () => {
        this.loading.set(false);
        this.error.set('The saved chapters could not be loaded.');
      },
    });
  }
}
