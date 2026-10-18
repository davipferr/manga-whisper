import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { ChapterService } from './services/chapter.service';
import { ReleaseSpreadComponent } from './components/release-spread/release-spread.component';
import { RecentChaptersComponent } from './components/recent-chapters/recent-chapters.component';
import { AuthService } from '../shared/core/auth.service';

@Component({
  selector: 'app-one-piece-tracker',
  imports: [ReleaseSpreadComponent, RecentChaptersComponent],
  templateUrl: './one-piece-tracker.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OnePieceTrackerComponent implements OnInit {
  protected readonly chapterService = inject(ChapterService);
  protected readonly isAdmin = inject(AuthService).hasAdminRole;

  ngOnInit(): void {
    this.chapterService.setPageSize(8);
    this.chapterService.fetchChapters();
  }
}
