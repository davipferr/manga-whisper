import { Component, signal } from '@angular/core';
import { OnePieceTrackerComponent } from './one-piece-tracker/one-piece-tracker.component';

@Component({
  selector: 'app-root',
  imports: [OnePieceTrackerComponent],
  template: '<app-one-piece-tracker></app-one-piece-tracker>',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('Manga Whisper');
}
