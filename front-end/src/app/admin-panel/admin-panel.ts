import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { WorkerStatusComponent } from './components/worker-status/worker-status';

@Component({
  selector: 'admin-panel',
  templateUrl: './admin-panel.html',
  styleUrl: './admin-panel.css',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, WorkerStatusComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminPanelComponent {}
