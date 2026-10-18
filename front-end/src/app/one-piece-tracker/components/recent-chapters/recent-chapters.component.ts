import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { Chapter } from '../../models/chapter.model';
import { ChapterService } from '../../services/chapter.service';

@Component({
  selector: 'app-recent-chapters',
  templateUrl: './recent-chapters.component.html',
  styleUrl: './recent-chapters.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RecentChaptersComponent {
  chapters = input<Chapter[]>([]);
  isAdmin = input<boolean>(false);

  protected readonly chapterService = inject(ChapterService);

  retryNow(): void {
    this.chapterService.fetchChapters(this.chapterService.currentPage());
  }
}
