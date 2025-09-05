import { Component, input, output } from '@angular/core';
import { Chapter } from '../../models/chapter.model';

@Component({
  selector: 'app-recent-chapters',
  template: `
    <div class="container-fluid d-flex justify-content-center">
      <div class="recent-chapters">
        <div class="section-header mb-4">
          <h2 class="mb-1">Recent Chapters</h2>
          <p class="text-muted mb-0">Keep track of the latest One Piece manga releases</p>
        </div>

        <div class="list-group list-group-flush">
          @for (chapter of chapters(); track chapter.number) {
          <div class="list-group-item px-0 py-3 border-0 border-bottom">
            <div class="d-flex justify-content-between align-items-start">
              <div class="flex-grow-1">
                <div class="d-flex align-items-center gap-2 mb-2">
                  <h6 class="fw-medium text-dark bg-primary rounded px-2 py-1 mb-0">
                    Chapter {{ chapter.number }} - {{ chapter.title }}
                  </h6>
                  <span
                    class="badge bg-success-subtle text-success px-2 border rounded border-success-subtle"
                    >
                    {{ chapter.status }}
                  </span>
                </div>

                <p class="">{{ chapter.date }}</p>
              </div>
            </div>
          </div>
          }
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      .recent-chapters {
        background: rgba(255, 255, 255, 0.95);
        border-radius: 16px;
        padding: 30px;
        box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
        max-width: 600px;
        width: 100%;
      }

      .section-header h2 {
        color: #333;
        font-size: 20px;
        font-weight: 600;
      }

      .list-group-item {
        background-color: transparent !important;
      }

      .list-group-item:last-child {
        border-bottom: none !important;
      }

      .btn-outline-primary:hover {
        transform: scale(1.05);
        transition: transform 0.2s ease;
      }
    `,
  ],
})
export class RecentChaptersComponent {
  chapters = input.required<Chapter[]>();
  chapterRead = output<Chapter>();
}
