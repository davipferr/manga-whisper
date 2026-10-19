import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, of, switchMap, timer } from 'rxjs';
import { AdminPanelApiService } from '../../admin-panel-api.service';
import { WorkerStatusResponseDto } from '../../admin-panel.model';

// The worker writes its heartbeat every 30s
const REFRESH_MS = 30_000;

@Component({
  selector: 'app-worker-status',
  imports: [DatePipe],
  template: `
    <p class="worker-status" [class.is-online]="state() === 'online'" [class.is-offline]="state() === 'offline'" role="status">
      <span class="dot" aria-hidden="true"></span>
      @switch (state()) {
        @case ('online') {
          <span>Worker running</span>
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
    .worker-status.is-online { border: 2px solid var(--mw-ink); }
    .worker-status.is-offline { border-color: var(--mw-red); }
    .dot {
      width: 12px;
      height: 12px;
      border: 2px solid var(--mw-ink);
      border-radius: 50%;
      background: transparent;
    }
    .is-online .dot { background: var(--mw-ink); }
    .is-offline .dot { border-color: var(--mw-red); background: var(--mw-red); }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WorkerStatusComponent implements OnInit {
  private readonly api = inject(AdminPanelApiService);
  private readonly destroyRef = inject(DestroyRef);

  readonly status = signal<WorkerStatusResponseDto | null>(null);
  readonly state = computed<'loading' | 'online' | 'offline'>(() => {
    const status = this.status();
    if (!status) {
      return 'loading';
    }
    return status.isOnline ? 'online' : 'offline';
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
