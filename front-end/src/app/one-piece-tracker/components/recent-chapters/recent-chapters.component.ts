import { Component, input, inject } from '@angular/core';
import { Chapter } from '../../models/chapter.model';
import { ChapterService } from '../../services/chapter.service';

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
          <div class="list-group list-group-flush">
            @for (chapter of chapters(); track chapter.number) {
              <div class="list-group-item px-0 py-3 border-0 border-bottom">
                <div class="d-flex justify-content-between align-items-start">
                  <div class="flex-grow-1">
                    <div class="d-flex align-items-center gap-3 mb-3">
                      <div class="chapter-title-pill">
                        Chapter {{ chapter.number }} - {{ chapter.title }}
                      </div>
                      <div class="status-pill">Released</div>
                    </div>
                  </div>
                </div>
              </div>
            }
          </div>
        }
      </div>
    </div>
  `,
  styleUrl: './recent-chapters.component.css',
})
export class RecentChaptersComponent {
  chapters = input<Chapter[]>([]);
  public chapterService = inject(ChapterService);
}
