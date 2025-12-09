import { Component, signal } from "@angular/core";
import { OnInit, inject } from "@angular/core";
import { AdminPanelService } from "./admin-panel.service";
import { Router } from "@angular/router";

@Component({
  selector: 'admin-panel',
  templateUrl: './admin-panel.html',
  styleUrl: './admin-panel.css'
})
export class AdminPanelComponent implements OnInit {
  private readonly adminPanelService = inject(AdminPanelService);
  private readonly router = inject(Router);

  readonly loading = signal<boolean>(false);

  ngOnInit(): void {
    const email = localStorage.getItem('email');
    const roleName = 'admin';

    debugger;

    this.loading.set(true);
    this.adminPanelService.checkRole({ email: email ?? '', roleName: roleName })
    .subscribe({
      next: (response) => {
        this.loading.set(false);
        if (!response.hasRole) {
          console.error('User does not have admin role:', response.message);
          this.router.navigate(['']);
        }
      },
      error: (error) => {
        this.loading.set(false);
        console.error('Error checking role:', error);
        this.router.navigate(['']);
      }
    });
  }
}
