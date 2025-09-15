import { Component, inject } from '@angular/core';
import { ChapterService } from './services/chapter.service';
import { HeaderComponent } from './components/header/header.component';
import { ChapterCardComponent } from './components/chapter-card/chapter-card.component';
import { RecentChaptersComponent } from './components/recent-chapters/recent-chapters.component';

@Component({
  selector: 'app-one-piece-tracker',
  imports: [HeaderComponent, ChapterCardComponent, RecentChaptersComponent],
  template: `
    <div class="manga-container">
      <div class="container-fluid">
        <app-header />

        <!-- Loading State -->
        <!-- TODO: Change this to the recent-chapters component  -->
        @if (chapterService.loading()) {
          <div class="loading-container text-center">
            <div class="spinner-border text-light" role="status"></div>
            <p class="text-light mt-2">Loading chapters from database...</p>
          </div>
        }

        <!-- Error State -->
        <!-- TODO: Change to alert -->
        @if (chapterService.errorMessage()) {
          <div class="alert alert-warning mx-auto d-flex align-items-center justify-content-around w-50" role="alert">
            <strong>Warning:</strong> {{ chapterService.errorMessage() }}
            <button class="btn btn-link p-0 ms-2" (click)="chapterService.reloadChapters()">
              Retry
            </button>
          </div>
        }

        <!-- Main Content -->
        <main class="main-content">
          <!-- Latest and Next Chapter Cards -->
          <div class="row g-3 mb-4">
            <div class="col-md-6">
              <app-chapter-card
                [chapterInfo]="latestChapter()"
                [isLatestChapter]="true"
                cardTitle="Latest Chapter"
              />
            </div>
            <div class="col-md-6">
              <app-chapter-card
                [chapterInfo]="nextChapter()"
                [isLatestChapter]="false"
                cardTitle="Next Chapter"
              />
            </div>
          </div>

          <app-recent-chapters [chapters]="chapterService.recentChapters()" />
        </main>
      </div>
    </div>
  `,
  styles: [
    `
      .manga-container {
        min-height: 100vh;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
        padding: 20px 0;
      }

      .main-content {
        max-width: 1200px;
        margin: 0 auto;
        padding: 0 15px;
      }

      .loading-container {
        padding: 60px 0;
      }

      @media (max-width: 768px) {
        .manga-container {
          padding: 15px 0;
        }
      }
    `,
  ],
})
export class OnePieceTrackerComponent {
  chapterService = inject(ChapterService);

  latestChapter() {
    return this.chapterService.getLatestChapter();
  }

  nextChapter() {
    return this.chapterService.getNextChapter();
  }
}
