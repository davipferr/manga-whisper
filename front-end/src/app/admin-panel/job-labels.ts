import { JobRunStatus, ScrapingJobType } from './admin-panel.model';

export const JOB_TYPE_TEXT: Record<ScrapingJobType, string> = {
  CheckNext: 'Check next chapter',
  Backfill: 'Backfill',
};

export const JOB_TYPE_HINT: Record<ScrapingJobType, string> = {
  CheckNext: 'Looks once for the chapter after the last known one.',
  Backfill: 'Saves every chapter after the last known one, until one is missing.',
};

export const RUN_STATUS_TEXT: Record<JobRunStatus, string> = {
  Pending: 'Waiting for the worker',
  Running: 'Running',
  Succeeded: 'Done',
  Failed: 'Failed',
  Cancelled: 'Cancelled',
};
