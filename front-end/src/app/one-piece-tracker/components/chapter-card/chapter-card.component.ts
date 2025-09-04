import { Component, input } from '@angular/core';
import { ChapterInfo } from '../../models/chapter.model';

@Component({
  selector: 'app-chapter-card',
  template: `
    <div class="chapter-card">
      <div class="card-header">
        <h2>{{ cardTitle() }}</h2>
        <span class="status-badge" [class]="statusClass()">{{ chapterInfo().statusLabel }}</span>
      </div>
      <div class="chapter-info">
        <span class="chapter-number">Chapter {{ chapterInfo().number }}</span>
        <h3 class="chapter-title">{{ chapterInfo().title }}</h3>
        <div class="chapter-date">
          <i class="calendar-icon">📅</i>
          {{ dateLabel() }} {{ chapterInfo().date }}
        </div>
      </div>
    </div>
  `,
  styles: [`
    .chapter-card {
      background: rgba(255, 255, 255, 0.95);
      border-radius: 16px;
      padding: 25px;
      box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
    }

    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
    }

    .card-header h2 {
      color: #333;
      font-size: 18px;
      font-weight: 600;
    }

    .status-badge {
      padding: 6px 12px;
      border-radius: 20px;
      font-size: 12px;
      font-weight: 500;
    }

    .status-badge.released {
      background: #ff4444;
      color: white;
    }

    .status-badge.upcoming {
      background: #ffa500;
      color: white;
    }

    .chapter-info {
      display: flex;
      flex-direction: column;
      gap: 10px;
    }

    .chapter-number {
      color: #666;
      font-size: 14px;
      font-weight: 500;
    }

    .chapter-title {
      color: #333;
      font-size: 20px;
      font-weight: 600;
      margin: 0;
    }

    .chapter-date {
      display: flex;
      align-items: center;
      gap: 8px;
      color: #666;
      font-size: 14px;
    }
  `]
})
export class ChapterCardComponent {
  chapterInfo = input.required<ChapterInfo>();
  cardTitle = input.required<string>();

  statusClass() {
    return this.chapterInfo().status.toLowerCase();
  }

  dateLabel() {
    return this.chapterInfo().status === 'Released' ? 'Released on' : 'Expected on';
  }
}
