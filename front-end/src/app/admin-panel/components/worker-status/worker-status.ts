import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, of, switchMap, timer } from 'rxjs';
import { AdminPanelApiService } from '../../admin-panel-api.service';
import { WorkerStatusResponseDto } from '../../admin-panel.model';
import { JOB_TYPE_TEXT } from '../../job-labels';

// The worker beats every 30s and records a clean shutdown at once; a short refresh shows stops and runs quickly
const REFRESH_MS = 5_000;

@Component({
  selector: 'app-worker-status',
  imports: [DatePipe],
  template: `
    <p
      class="worker-status"
      [class.is-idle]="state() === 'idle'"
      [class.is-busy]="state() === 'busy'"
      [class.is-offline]="state() === 'offline'"
      role="status"
    >
      <span class="dot" aria-hidden="true"></span>
      @switch (state()) {
        @case ('idle') {
          <span>Worker idle</span>
        }
        @case ('busy') {
          @if (status()?.currentRun; as run) {
            <span>Worker busy: {{ typeText[run.type] }} for {{ run.mangaTitle }}</span>
          }
        }
        @case ('offline') {
          <span>
            Worker not running
            @if (status()?.lastSeenAt; as lastSeen) {
              <span class="mw-muted">(last seen {{ lastSeen | date: 'd MMM, HH:mm' }})</span>
            }
          </span>
        }
        @default {
          <span class="mw-muted">Checking the worker</span>
        }
      }
    </p>
  `,
  styles: `
    .worker-status {
      display: inline-flex;
      align-items: center;
      gap: 10px;
      margin: 0;
      padding: 8px 14px;
      font-weight: 700;
      font-size: 0.9rem;
      border: 2px dashed var(--mw-tone);
      background: var(--mw-paper-raised);
    }
    .worker-status.is-idle,
    .worker-status.is-busy { border: 2px solid var(--mw-ink); }
    .worker-status.is-busy { background: var(--mw-straw); }
    .worker-status.is-offline { border-color: var(--mw-red); }
    .dot {
      width: 12px;
      height: 12px;
      border: 2px solid var(--mw-ink);
      border-radius: 50%;
      background: transparent;
    }
    .is-idle .dot,
    .is-busy .dot { background: var(--mw-ink); }
    .is-busy .dot { animation: pulse 1.2s ease-in-out infinite; }
    @keyframes pulse { 50% { opacity: 0.2; } }
    @media (prefers-reduced-motion: reduce) {
      .is-busy .dot { animation: none; }
    }
    .is-offline .dot { border-color: var(--mw-red); background: var(--mw-red); }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WorkerStatusComponent implements OnInit {
  private readonly api = inject(AdminPanelApiService);
  private readonly destroyRef = inject(DestroyRef);

  readonly typeText = JOB_TYPE_TEXT;
  readonly status = signal<WorkerStatusResponseDto | null>(null);
  readonly state = computed<'loading' | 'idle' | 'busy' | 'offline'>(() => {
    const status = this.status();
    if (!status) {
      return 'loading';
    }
    if (!status.isOnline) {
      return 'offline';
    }
    return status.currentRun ? 'busy' : 'idle';
  });

  ngOnInit(): void {
    timer(0, REFRESH_MS)
      .pipe(
        // Auth errors are handled by the page; here the status simply stays unknown until the next try
        switchMap(() => this.api.getWorkerStatus().pipe(catchError(() => of(null)))),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe((status) => this.status.set(status));
  }
}
