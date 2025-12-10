import { Component, signal, OnInit } from '@angular/core';
import { inject } from "@angular/core";
import { AuthService } from "../../core/auth.service";

@Component({
  selector: 'app-header',
  templateUrl: './header.html',
  styleUrls: ['./header.css'],
})
export class HeaderComponent implements OnInit {
  private readonly authService = inject(AuthService);

  readonly title = signal<string>('Manga Whisper');
  readonly isAdmin = this.authService.hasAdminRole;
  readonly adminName = signal<string>('');

  ngOnInit() {
    if (this.isAdmin()) {
      this.adminName.set(this.authService?.userData()?.fullName || 'Admin');
    }
  }
}
