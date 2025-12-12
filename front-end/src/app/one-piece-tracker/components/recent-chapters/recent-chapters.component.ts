import { Component, input, inject } from '@angular/core';
import { Chapter } from '../../models/chapter.model';
import { ChapterService } from '../../services/chapter.service';
import { AuthService } from 'src/app/shared/core/auth.service';

@Component({
  selector: 'app-recent-chapters',
  standalone: true,
  template: `
    <div class="container-fluid d-flex justify-content-center">
      <div class="recent-chapters">
        <div class="section-header mb-4">
          <h2 class="mb-1">Recent Chapters</h2>
          <p class="text-muted mb-0">Keep track of the latest One Piece manga releases</p>
        </div>

        <!-- Loading State -->
        @if (chapterService.loading()) {
          <div class="loading-container text-center">
            <div class="spinner-border text-black" role="status"></div>
            <p class="text-black mt-2">Loading chapters from database...</p>
          </div>
        }

        <!-- Error State -->
        @if (chapterService.errorMessage()) {
          <div class="alert alert-warning mx-auto">
            <p class="d-flex align-items-center justify-content-evenly">
              <strong>Warning:</strong> {{ chapterService.errorMessage() }}
            </p>
            <p class="text-center mb-0">
              <strong>The request will be retried in {{ chapterService.retryAfter }} seconds</strong>
            </p>
          </div>
        }

        <!-- Chapters List -->
        @if (!chapterService.loading() && !chapterService.errorMessage()) {
          <div class="list-group list-group-flush list-group-min-height">
            @for (chapter of chapters(); track chapter.number) {
              <div class="list-group-item px-0 py-3 border-0 border-bottom">
                <div class="d-flex justify-content-between align-items-start">
                  <div class="flex-grow-1">
                    <div class="d-flex align-items-center gap-3 mb-3">
                      <div class="chapter-title-pill">
                        Chapter {{ chapter.number }} - {{ chapter.title ? chapter.title : 'No Title' }}
                      </div>
                      <div class="status-pill">Released</div>
                      @if (isAdmin()) {
                        <button class="see-chapter-pill"> See chapter </button>
                      }
                    </div>
                  </div>
                </div>
              </div>
            }
          </div>
        }

        <!-- Pagination Controls -->
        <div class="pagination-controls text-center mt-4">
          <div>
            <button class="btn btn-primary me-2 w-25" (click)="chapterService.previousPage()" [disabled]="chapterService.currentPage() === 1">
              Previous
            </button>
            <span class="fs-6 fw-bold">Page {{ chapterService.currentPage() }}</span>
            <button class="btn btn-primary ms-2 w-25" (click)="chapterService.nextPage()" [disabled]="chapterService.currentPage() === chapterService.totalPages() || chapterService.totalPages() === 0">
              Next
            </button>
          </div>
          <div class="mt-2 fw-bold fs-5">
            <span> Total Pages: {{ chapterService.totalPages() }} </span>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrl: './recent-chapters.component.css',
})
export class RecentChaptersComponent {
  chapters = input<Chapter[]>([]);
  chapterService = inject(ChapterService);
  private readonly authService = inject(AuthService);

  readonly isAdmin = this.authService.hasAdminRole;

  ngOnInit(): void {
    this.chapterService.setPageSize(3);
    this.chapterService.fetchChapters();
  }
}
