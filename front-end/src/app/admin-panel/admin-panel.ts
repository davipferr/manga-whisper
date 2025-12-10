import { Component } from "@angular/core";
import { inject } from "@angular/core";
import { AuthService } from "../shared/core/auth.service";

@Component({
  selector: 'admin-panel',
  templateUrl: './admin-panel.html',
  styleUrl: './admin-panel.css'
})
export class AdminPanelComponent {
  private readonly authService = inject(AuthService);

  readonly userData = this.authService.userData;
  readonly hasAdminRole = this.authService.hasAdminRole;
}
