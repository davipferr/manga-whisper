import { Routes } from '@angular/router';
import { OnePieceTrackerComponent } from './one-piece-tracker/one-piece-tracker.component';

export const routes: Routes = [
  { path: '', component: OnePieceTrackerComponent },
  { path: '**', redirectTo: '' }
];
