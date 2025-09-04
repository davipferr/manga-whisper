import { Component, input, output } from '@angular/core';
import { Chapter } from '../../models/chapter.model';

@Component({
  selector: 'app-recent-chapters',
  template: `
    <div class="recent-chapters">
      <div class="section-header">
        <h2>Recent Chapters</h2>
        <p>Keep track of the latest One Piece manga releases</p>
      </div>

      <div class="chapters-list">
        @for (chapter of chapters(); track chapter.number) {
          <div class="chapter-item">
            <div class="chapter-details">
              <div class="chapter-title-section">
                <span class="chapter-number">Chapter {{ chapter.number }}</span>
                <span class="chapter-status">{{ chapter.status }}</span>
              </div>
              <h4 class="chapter-name">{{ chapter.title }}</h4>
              <span class="chapter-date">{{ chapter.date }}</span>
            </div>
            <div class="chapter-actions">
              <button class="read-btn" (click)="onReadChapter(chapter)">📖</button>
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .recent-chapters {
      background: rgba(255, 255, 255, 0.95);
      border-radius: 16px;
      padding: 30px;
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
    }

    .section-header {
      margin-bottom: 25px;
    }

    .section-header h2 {
      color: #333;
      font-size: 20px;
      font-weight: 600;
      margin-bottom: 5px;
    }

    .section-header p {
      color: #666;
      font-size: 14px;
    }

    .chapters-list {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }

    .chapter-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 15px 0;
      border-bottom: 1px solid #f0f0f0;
    }

    .chapter-item:last-child {
      border-bottom: none;
    }

    .chapter-details {
      flex: 1;
    }

    .chapter-title-section {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-bottom: 5px;
    }

    .chapter-number {
      color: #333;
      font-weight: 600;
      font-size: 14px;
    }

    .chapter-status {
      background: #e8f5e8;
      color: #4caf50;
      padding: 4px 8px;
      border-radius: 12px;
      font-size: 11px;
      font-weight: 500;
    }

    .chapter-name {
      color: #333;
      font-size: 16px;
      font-weight: 500;
      margin: 5px 0;
    }

    .chapter-date {
      color: #999;
      font-size: 12px;
    }

    .read-btn {
      background: #f8f9ff;
      border: none;
      width: 36px;
      height: 36px;
      border-radius: 8px;
      cursor: pointer;
      font-size: 16px;
      transition: all 0.2s ease;
    }

    .read-btn:hover {
      background: #eef0ff;
      transform: scale(1.05);
    }
  `]
})
export class RecentChaptersComponent {
  chapters = input.required<Chapter[]>();
  chapterRead = output<Chapter>();

  onReadChapter(chapter: Chapter): void {
    this.chapterRead.emit(chapter);
  }
}
