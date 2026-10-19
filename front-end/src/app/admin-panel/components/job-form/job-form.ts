import { ChangeDetectionStrategy, Component, computed, effect, inject, input, output, signal, untracked } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { AdminPanelApiService } from '../../admin-panel-api.service';
import { AdminChecker, SaveScrapingJobRequestDto, ScrapingJob, ScrapingJobType } from '../../admin-panel.model';
import { JOB_TYPE_HINT, JOB_TYPE_TEXT } from '../../job-labels';
import { WEEK_DAYS, describeCron, fromCron, parseHours, toCron } from '../../schedule';

// One Piece usually comes out on Thursday, at an hour nobody knows in advance
const DEFAULT_DAYS = [4];
const DEFAULT_HOURS = '12, 16, 20';

@Component({
  selector: 'app-job-form',
  imports: [ReactiveFormsModule],
  templateUrl: './job-form.html',
  styleUrl: './job-form.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class JobFormComponent {
  private readonly api = inject(AdminPanelApiService);

  checkers = input.required<AdminChecker[]>();
  // Null to create a new job
  job = input<ScrapingJob | null>(null);
  saved = output<ScrapingJob>();
  cancelled = output<void>();
  unauthorized = output<void>();

  readonly weekDays = WEEK_DAYS;
  readonly jobTypes: ScrapingJobType[] = ['CheckNext', 'Backfill'];
  readonly typeText = JOB_TYPE_TEXT;
  readonly typeHint = JOB_TYPE_HINT;

  readonly saving = signal(false);
  readonly error = signal('');

  readonly form = inject(FormBuilder).nonNullable.group({
    mangaCheckerId: [0, [Validators.required, Validators.min(1)]],
    type: ['CheckNext' as ScrapingJobType, Validators.required],
    days: [DEFAULT_DAYS],
    hours: [DEFAULT_HOURS, Validators.required],
    minute: [0, [Validators.required, Validators.min(0), Validators.max(59)]],
    isEnabled: [true],
  });

  private readonly formValue = toSignal(this.form.valueChanges, { initialValue: this.form.getRawValue() });

  readonly parsedHours = computed(() => parseHours(this.formValue().hours ?? ''));
  readonly cron = computed(() => {
    const { days, minute } = this.formValue();
    const hours = this.parsedHours();
    if (!hours || !days?.length || minute === undefined || minute === null || minute < 0 || minute > 59) {
      return null;
    }
    return toCron({ days, hours, minute: Number(minute) });
  });
  readonly preview = computed(() => {
    const cron = this.cron();
    return cron ? describeCron(cron) : null;
  });

  readonly isEdit = computed(() => this.job() !== null);

  constructor() {
    // Fill the form from the job being edited (or the defaults for a new one)
    effect(() => {
      const job = this.job();
      const checkers = this.checkers();

      untracked(() => {
        const schedule = job ? fromCron(job.cronExpression) : null;
        this.form.reset({
          mangaCheckerId: job?.mangaCheckerId ?? checkers[0]?.id ?? 0,
          type: job?.type ?? 'CheckNext',
          days: schedule?.days ?? DEFAULT_DAYS,
          hours: schedule ? schedule.hours.join(', ') : DEFAULT_HOURS,
          minute: schedule?.minute ?? 0,
          isEnabled: job?.isEnabled ?? true,
        });
        this.error.set('');
      });
    });
  }

  isDaySelected(day: number): boolean {
    return (this.formValue().days ?? []).includes(day);
  }

  toggleDay(day: number): void {
    const days = this.form.controls.days.value;
    this.form.controls.days.setValue(days.includes(day) ? days.filter((d) => d !== day) : [...days, day]);
    this.form.controls.days.markAsDirty();
  }

  submit(): void {
    const cron = this.cron();
    if (this.form.invalid || !cron) {
      this.form.markAllAsTouched();
      this.error.set('Pick at least one day, hours between 0 and 23 and a minute between 0 and 59.');
      return;
    }

    const value = this.form.getRawValue();
    const request: SaveScrapingJobRequestDto = {
      mangaCheckerId: Number(value.mangaCheckerId),
      type: value.type,
      cronExpression: cron,
      isEnabled: value.isEnabled,
    };

    const job = this.job();
    const save$ = job ? this.api.updateJob(job.id, request) : this.api.createJob(request);

    this.saving.set(true);
    this.error.set('');

    save$.subscribe({
      next: (result) => {
        this.saving.set(false);
        if (result.job) {
          this.saved.emit(result.job);
        }
      },
      error: (error: HttpErrorResponse) => {
        this.saving.set(false);

        if (error.status === 401 || error.status === 403) {
          this.unauthorized.emit();
          return;
        }

        this.error.set(error.error?.errorMessage ?? 'The job was not saved. Try again.');
      },
    });
  }
}
