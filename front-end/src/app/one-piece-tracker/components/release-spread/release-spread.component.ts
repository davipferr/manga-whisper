import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { Chapter } from '../../models/chapter.model';

const ONE_WEEK_MS = 7 * 24 * 60 * 60 * 1000;

@Component({
  selector: 'app-release-spread',
  templateUrl: './release-spread.component.html',
  styleUrl: './release-spread.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReleaseSpreadComponent {
  latest = input<Chapter | null>(null);
  nextNumber = input.required<number>();
  loading = input<boolean>(false);
  isAdmin = input<boolean>(false);

  readonly isNew = computed(() => {
    const releasedAt = parseApiDate(this.latest()?.extractedAt);
    return releasedAt !== null && Date.now() - releasedAt.getTime() < ONE_WEEK_MS;
  });
}

/** The API sends dates as dd/MM/yyyy. */
function parseApiDate(value: string | undefined): Date | null {
  const match = value?.match(/^(\d{2})\/(\d{2})\/(\d{4})$/);
  if (!match) {
    return null;
  }

  const [, day, month, year] = match;
  return new Date(Number(year), Number(month) - 1, Number(day));
}
