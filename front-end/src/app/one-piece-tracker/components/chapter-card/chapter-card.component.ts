import { Component, input } from '@angular/core';
import { Chapter } from '../../models/chapter.model';

@Component({
  selector: 'app-chapter-card',
  template: `
    <div class="card h-100 chapter-card-container">
      <div class="card-body d-flex flex-column p-4">
        <!-- Card Header -->
        <div class="d-flex justify-content-between align-items-center">
          <h2 class="card-title fs-4 fw-semibold text-dark">
            {{ cardTitle() }}
          </h2>

          <span class="badge rounded-pill px-3 py-2 fw-medium" [class]="getStatusLabelClass()">
            {{ getStatusLabelText() }}
          </span>
        </div>

        <!-- Chapter Info -->
        <div class="flex-grow-1 d-flex flex-column justify-content-center">
          <div class="mb-3">
            <span
              class="badge bg-secondary bg-opacity-10 text-secondary rounded-pill px-3 py-2 fs-6"
            >
              Chapter {{ chapterInfo().number }}
            </span>
          </div>
          <h3 class="chapter-title fs-2 fw-bold text-dark mb-4">
            {{ chapterInfo().title ? chapterInfo().title : 'No Title' }}
          </h3>

          <div>
            <div class="d-flex align-items-center text-muted">
              <span class="fs-5">
                {{ dateLabel() }} <strong class="text-dark">{{ chapterInfo().extractedAt }}</strong>
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrl: './chapter-card.component.css',
})
export class ChapterCardComponent {
  chapterInfo = input.required<Chapter>();
  cardTitle = input.required<string>();
  isLatestChapter = input.required<boolean>();

  getStatusLabelClass() {

    if(this.isLatestChapter())
    {
      return 'status-badge-released';

    }

    return 'status-badge-upcoming';
  }

  getStatusLabelText() {

    if(this.isLatestChapter())
    {
      return 'Released';
    }

    return 'Upcoming';
  }

  dateLabel() {

    if (this.isLatestChapter())
    {
      return 'Extracted on';
    }

    return 'Expected on';
  }
}
