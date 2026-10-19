import { Routes } from '@angular/router';
import { OnePieceTrackerComponent } from './one-piece-tracker/one-piece-tracker.component';
import { AdminLoginComponent } from './admin-login/admin-login';
import { AdminPanelComponent } from './admin-panel/admin-panel';
import { AdminCheckersComponent } from './admin-panel/pages/admin-checkers/admin-checkers';
import { AdminJobsComponent } from './admin-panel/pages/admin-jobs/admin-jobs';
import { adminGuard } from './shared/core/admin.guard';

export const routes: Routes = [
  { path: '', component: OnePieceTrackerComponent },
  { path: 'admin', component: AdminLoginComponent },
  {
    path: 'admin-panel',
    component: AdminPanelComponent,
    canActivate: [adminGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'checkers' },
      { path: 'checkers', component: AdminCheckersComponent },
      { path: 'jobs', component: AdminJobsComponent },
    ],
  },
  { path: '**', redirectTo: '' },
];
