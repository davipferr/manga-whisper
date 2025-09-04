import { Component, inject } from '@angular/core';
import { ChapterService } from './services/chapter.service';
import { HeaderComponent } from './components/header/header.component';
import { ChapterCardComponent } from './components/chapter-card/chapter-card.component';
import { NotificationsSectionComponent } from './components/notifications-section/notifications-section.component';
import { RecentChaptersComponent } from './components/recent-chapters/recent-chapters.component';
import { Chapter } from './models/chapter.model';

@Component({
  selector: 'app-one-piece-tracker',
  imports: [
    HeaderComponent,
    ChapterCardComponent,
    NotificationsSectionComponent,
    RecentChaptersComponent
  ],
  template: `
    <div class="manga-container">
      <div class="container-fluid">
        <app-header />

        <!-- Main Content -->
        <main class="main-content">
          <!-- Latest and Next Chapter Cards -->
          <div class="row g-3 mb-4">
            <div class="col-md-6">
              <app-chapter-card
                [chapterInfo]="latestChapter()"
                cardTitle="Latest Chapter" />
            </div>
            <div class="col-md-6">
              <app-chapter-card
                [chapterInfo]="nextChapter()"
                cardTitle="Next Chapter" />
            </div>
          </div>

          <app-notifications-section />

          <app-recent-chapters
            [chapters]="chapterService.recentChapters()"
            (chapterRead)="onChapterRead($event)" />
        </main>
      </div>
    </div>
  `,
  styles: [`
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

    @media (max-width: 768px) {
      .manga-container {
        padding: 15px 0;
      }
    }
  `]
})
export class OnePieceTrackerComponent {
  chapterService = inject(ChapterService);

  latestChapter() {
    return this.chapterService.getLatestChapter();
  }

  nextChapter() {
    return this.chapterService.getNextChapter();
  }

  onChapterRead(chapter: Chapter): void {
    console.log('Reading chapter:', chapter);
    // Here you could navigate to a reading page or open a modal
  }
}
