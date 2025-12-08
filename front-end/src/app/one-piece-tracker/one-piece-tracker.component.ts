import { Component, inject } from '@angular/core';
import { ChapterService } from './services/chapter.service';
import { ChapterCardComponent } from './components/chapter-card/chapter-card.component';
import { RecentChaptersComponent } from './components/recent-chapters/recent-chapters.component';

@Component({
  selector: 'app-one-piece-tracker',
  imports: [ChapterCardComponent, RecentChaptersComponent],
  template: `
    <div class="manga-container">
      <div class="container-fluid">
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
      .main-content {
        max-width: 1200px;
        margin: 0 auto;
        padding: 0 15px;
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
