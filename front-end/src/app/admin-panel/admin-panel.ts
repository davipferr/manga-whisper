import { Component } from "@angular/core";
import { inject } from "@angular/core";
import { AuthService } from "../shared/core/auth.service";
import { AdminPanelService } from "./admin-panel.service";
import { signal } from "@angular/core";

@Component({
  selector: 'admin-panel',
  templateUrl: './admin-panel.html',
  styleUrl: './admin-panel.css'
})
export class AdminPanelComponent {
  private readonly authService = inject(AuthService);
  private readonly adminPanelService = inject(AdminPanelService);

  readonly userData = this.authService.userData;
  readonly hasAdminRole = this.authService.hasAdminRole;
  readonly isCheckingNewChapters = signal(false);

  onCheckForNewChapters(): void {
    if (this.isCheckingNewChapters()) {
      return;
    }

    this.isCheckingNewChapters.set(true);

    this.adminPanelService.triggerManualChapterCheck().subscribe({
      next: (response) => {
        if (response.success) {
          alert(response.message || 'Chapter check completed successfully!');
        } else {
          alert(response.errorMessage || 'Failed to check for new chapters.');
        }
        this.isCheckingNewChapters.set(false);
      },
      error: (error) => {
        console.error('Error triggering manual chapter check:', error);
        alert('An error occurred while checking for new chapters.');
        this.isCheckingNewChapters.set(false);
      }
    });
  }
}
