import { Component } from "@angular/core";
import { inject } from "@angular/core";
import { AuthService } from "../shared/core/auth.service";
import { AdminPanelApiService } from "./admin-panel-api.service";
import { AdminPanelService } from "./admin-panel.service";
import { signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { StatusMessage } from "./admin-panel.model";
import { MangaTitleEnum } from "./manga-title.enum";
import { switchMap } from "rxjs";

@Component({
  selector: 'admin-panel',
  templateUrl: './admin-panel.html',
  styleUrl: './admin-panel.css',
  imports: [CommonModule]
})
export class AdminPanelComponent {
  private readonly authService = inject(AuthService);
  private readonly adminPanelApiService = inject(AdminPanelApiService);
  private readonly adminPanelService = inject(AdminPanelService);

  readonly userData = this.authService.userData;
  readonly hasAdminRole = this.authService.hasAdminRole;
  readonly isCheckingNewChapters = signal(false);
  readonly statusMessage = signal<StatusMessage>({ message: '' });
  readonly mangaTitleEnum = MangaTitleEnum;

  onCheckForNewChapters(): void {
    if (this.isCheckingNewChapters()) {
      return;
    }

    this.isCheckingNewChapters.set(true);

    let warningMessage = this.adminPanelService.setWarningMessage('Manual chapter check started...')
    this.statusMessage.set(warningMessage);

    this.adminPanelApiService.triggerManualChapterCheck().subscribe({
      next: (response) => {
        let message = this.adminPanelService.handleSetStatusMessage(response);
        this.statusMessage.set(message);
        this.isCheckingNewChapters.set(false);
      },
      error: (error) => {
        let message = this.adminPanelService.handleSetStatusMessage(error);
        this.statusMessage.set(message);
        this.isCheckingNewChapters.set(false);
      }
    });
  }

  ProcessAllAvailableChapters(): void {
    if (this.isCheckingNewChapters()) {
      return;
    }

    this.isCheckingNewChapters.set(true);

    let warningMessage = this.adminPanelService.setWarningMessage('Manual all chapters available check started...')
    this.statusMessage.set(warningMessage);

    this.adminPanelApiService.getMangaCheckersByMangaTitle(this.mangaTitleEnum.OnePiece).pipe(
      switchMap(checkerResponse => {
        if (checkerResponse.mangaCheckers.length === 0) {
          let message: StatusMessage = {
            message: 'No manga checker found for the selected manga title.',
            isError: true
          };
          this.statusMessage.set(message);
          this.isCheckingNewChapters.set(false);
          throw new Error('No manga checker found');
        }
        let checkerId = checkerResponse.mangaCheckers[0].id;
        return this.adminPanelApiService.triggerProcessAllAvailableChapters(checkerId);
      })
    ).subscribe({
      next: (response) => {
        let message = this.adminPanelService.handleSetStatusMessage(response);
        this.statusMessage.set(message);
        this.isCheckingNewChapters.set(false);
      },
      error: (error) => {
        let message = this.adminPanelService.handleSetStatusMessage(error);
        this.statusMessage.set(message);
        this.isCheckingNewChapters.set(false);
      }
    });
  }
}
