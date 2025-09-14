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
                <div class="d-flex align-items-center gap-3 mb-3">
                  <div class="chapter-title-pill">
                    Chapter {{ chapter.number }} - {{ chapter.title }}
                  </div>
                  <div class="status-pill">
                    {{ chapter.status }}
                  </div>
                </div>

                <p class="text-muted mb-0 ms-1">{{ chapter.date }}</p>
              </div>
            </div>
          </div>
          }
        </div>
      </div>
    </div>
  `,
  styleUrl: './recent-chapters.component.css',
})
export class RecentChaptersComponent {
  chapters = input.required<Chapter[]>();
}
