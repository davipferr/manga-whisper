import { ChangeDetectionStrategy, Component, computed, effect, inject, input, output, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { AdminPanelApiService } from '../../admin-panel-api.service';
import { AdminChecker, CheckerStatus, ScrapingJobType, UpdateCheckerRequestDto } from '../../admin-panel.model';
import { JOB_TYPE_HINT, JOB_TYPE_TEXT } from '../../job-labels';

const STATUS_TEXT: Record<CheckerStatus, string> = {
  Idle: 'Ready',
  Checking: 'Checking now',
  Error: 'Last check failed',
  Stopped: 'Paused',
};

@Component({
  selector: 'app-checker-card',
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './checker-card.html',
  styleUrl: './checker-card.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CheckerCardComponent {
  private readonly api = inject(AdminPanelApiService);

  checker = input.required<AdminChecker>();
  updated = output<AdminChecker>();
  unauthorized = output<void>();

  readonly jobTypes: ScrapingJobType[] = ['CheckNext', 'Backfill'];
  readonly typeText = JOB_TYPE_TEXT;
  readonly typeHint = JOB_TYPE_HINT;

  readonly saving = signal(false);
  readonly feedback = signal('');
  readonly feedbackIsError = signal(false);

  readonly chapterForm = inject(FormBuilder).nonNullable.group({
    lastKnownChapter: [0, [Validators.required, Validators.min(0), Validators.max(100000)]],
  });

  private readonly typedChapter = toSignal(this.chapterForm.controls.lastKnownChapter.valueChanges, {
    initialValue: 0,
  });

  readonly statusText = computed(() => STATUS_TEXT[this.checker().status] ?? this.checker().status);
  readonly nextToCheck = computed(() => {
    const typed = Number(this.typedChapter());
    return (Number.isFinite(typed) ? typed : this.checker().lastKnownChapter) + 1;
  });

  constructor() {
    // Keep the input in sync with the server value whenever the checker changes
    effect(() => {
      this.chapterForm.reset({ lastKnownChapter: this.checker().lastKnownChapter });
    });
  }

  toggleActive(): void {
    const isActive = !this.checker().isActive;
    this.save({ isActive }, isActive ? 'Checking resumed.' : 'Checking paused.');
  }

  saveLastKnownChapter(): void {
    if (this.chapterForm.invalid) {
      return;
    }

    const lastKnownChapter = this.chapterForm.getRawValue().lastKnownChapter;
    this.save({ lastKnownChapter }, `Saved. The checker looks for chapter ${lastKnownChapter + 1} next.`);
  }

  runNow(type: ScrapingJobType): void {
    this.saving.set(true);
    this.feedback.set('');

    this.api.requestRun(this.checker().id, type).subscribe({
      next: () => {
        this.saving.set(false);
        this.feedbackIsError.set(false);
        this.feedback.set(
          `${JOB_TYPE_TEXT[type]} queued from chapter ${this.checker().lastKnownChapter + 1}. ` +
            'The worker starts it within 30 seconds; follow it in the run history on the Scheduled jobs tab.'
        );
      },
      error: (error: HttpErrorResponse) => {
        this.saving.set(false);

        if (error.status === 401 || error.status === 403) {
          this.unauthorized.emit();
          return;
        }

        this.feedbackIsError.set(true);
        this.feedback.set(error.error?.errorMessage ?? 'The run was not requested. Try again.');
      },
    });
  }

  private save(request: UpdateCheckerRequestDto, successMessage: string): void {
    this.saving.set(true);
    this.feedback.set('');

    this.api.updateChecker(this.checker().id, request).subscribe({
      next: (result) => {
        this.saving.set(false);
        if (result.checker) {
          this.updated.emit(result.checker);
        }
        this.feedbackIsError.set(false);
        this.feedback.set(successMessage);
      },
      error: (error: HttpErrorResponse) => {
        this.saving.set(false);

        if (error.status === 401 || error.status === 403) {
          this.unauthorized.emit();
          return;
        }

        this.feedbackIsError.set(true);
        this.feedback.set(error.error?.errorMessage ?? 'The change was not saved. Try again.');
      },
    });
  }
}
