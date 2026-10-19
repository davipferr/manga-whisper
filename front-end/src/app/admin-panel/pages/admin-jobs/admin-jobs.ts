import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { timer } from 'rxjs';
import { AuthService } from '../../../shared/core/auth.service';
import { AdminPanelApiService } from '../../admin-panel-api.service';
import { AdminChecker, JobRun, ScrapingJob } from '../../admin-panel.model';
import { JobFormComponent } from '../../components/job-form/job-form';
import { JOB_TYPE_TEXT, RUN_STATUS_TEXT } from '../../job-labels';
import { describeCron } from '../../schedule';

// Run history refresh, so "Run now" requests show their progress without reloading
const RUNS_REFRESH_MS = 15_000;

@Component({
  selector: 'app-admin-jobs',
  templateUrl: './admin-jobs.html',
  styleUrl: './admin-jobs.css',
  imports: [DatePipe, JobFormComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminJobsComponent implements OnInit {
  private readonly api = inject(AdminPanelApiService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly typeText = JOB_TYPE_TEXT;
  readonly runStatusText = RUN_STATUS_TEXT;
  readonly describeCron = describeCron;

  readonly checkers = signal<AdminChecker[]>([]);
  readonly jobs = signal<ScrapingJob[]>([]);
  readonly runs = signal<JobRun[]>([]);
  readonly loading = signal(false);
  readonly loadError = signal<string | null>(null);
  readonly runsError = signal<string | null>(null);

  // 'new' while creating, the job while editing, null when the form is closed
  readonly editing = signal<ScrapingJob | 'new' | null>(null);
  readonly busyJobId = signal<number | null>(null);
  readonly confirmDeleteId = signal<number | null>(null);
  readonly feedback = signal('');
  readonly feedbackIsError = signal(false);

  ngOnInit(): void {
    this.loadJobs();

    timer(0, RUNS_REFRESH_MS)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.loadRuns());
  }

  loadJobs(): void {
    this.loading.set(true);
    this.loadError.set(null);

    this.api.getCheckers().subscribe({
      next: (response) => this.checkers.set(response.checkers),
      error: (error: HttpErrorResponse) => this.handleLoadError(error),
    });

    this.api.getJobs().subscribe({
      next: (response) => {
        this.loading.set(false);
        this.jobs.set(response.jobs);
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);
        this.handleLoadError(error);
      },
    });
  }

  loadRuns(): void {
    this.api.getRuns().subscribe({
      next: (response) => {
        this.runsError.set(null);
        this.runs.set(response.runs);
      },
      error: (error: HttpErrorResponse) => {
        if (error.status === 401 || error.status === 403) {
          this.onUnauthorized();
          return;
        }
        this.runsError.set('The run history could not be loaded.');
      },
    });
  }

  openNew(): void {
    this.feedback.set('');
    this.editing.set('new');
  }

  edit(job: ScrapingJob): void {
    this.feedback.set('');
    this.editing.set(job);
  }

  editedJob(): ScrapingJob | null {
    const editing = this.editing();
    return editing === 'new' ? null : editing;
  }

  onSaved(saved: ScrapingJob): void {
    const exists = this.jobs().some((j) => j.id === saved.id);
    this.jobs.update((list) => (exists ? list.map((j) => (j.id === saved.id ? saved : j)) : [...list, saved]));
    this.editing.set(null);
    this.showFeedback(exists ? 'Job saved.' : 'Job created.', false);
  }

  toggleEnabled(job: ScrapingJob): void {
    this.busyJobId.set(job.id);
    this.feedback.set('');

    this.api
      .updateJob(job.id, {
        mangaCheckerId: job.mangaCheckerId,
        type: job.type,
        cronExpression: job.cronExpression,
        isEnabled: !job.isEnabled,
      })
      .subscribe({
        next: (result) => {
          this.busyJobId.set(null);
          if (result.job) {
            const updated = result.job;
            this.jobs.update((list) => list.map((j) => (j.id === updated.id ? updated : j)));
            this.showFeedback(updated.isEnabled ? 'Job enabled.' : 'Job disabled.', false);
          }
        },
        error: (error: HttpErrorResponse) => this.handleActionError(error, 'The job was not changed. Try again.'),
      });
  }

  delete(job: ScrapingJob): void {
    if (this.confirmDeleteId() !== job.id) {
      this.confirmDeleteId.set(job.id);
      return;
    }

    this.busyJobId.set(job.id);
    this.feedback.set('');

    this.api.deleteJob(job.id).subscribe({
      next: () => {
        this.busyJobId.set(null);
        this.confirmDeleteId.set(null);
        this.jobs.update((list) => list.filter((j) => j.id !== job.id));
        if (this.editedJob()?.id === job.id) {
          this.editing.set(null);
        }
        this.showFeedback('Job deleted. Its past runs stay in the history.', false);
      },
      error: (error: HttpErrorResponse) => {
        this.confirmDeleteId.set(null);
        this.handleActionError(error, 'The job was not deleted. Try again.');
      },
    });
  }

  onUnauthorized(): void {
    this.authService.clearAuthData();
    this.router.navigate(['/admin']);
  }

  private handleLoadError(error: HttpErrorResponse): void {
    if (error.status === 401 || error.status === 403) {
      this.onUnauthorized();
      return;
    }

    this.loadError.set(
      error.status === 0
        ? 'The API could not be reached. Check that it is running, then try again.'
        : (error.error?.errorMessage ?? 'The jobs could not be loaded.')
    );
  }

  private handleActionError(error: HttpErrorResponse, fallback: string): void {
    this.busyJobId.set(null);

    if (error.status === 401 || error.status === 403) {
      this.onUnauthorized();
      return;
    }

    this.showFeedback(error.error?.errorMessage ?? fallback, true);
  }

  private showFeedback(message: string, isError: boolean): void {
    this.feedbackIsError.set(isError);
    this.feedback.set(message);
  }
}
