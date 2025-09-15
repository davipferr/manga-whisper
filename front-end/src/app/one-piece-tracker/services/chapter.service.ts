import { Injectable, signal } from '@angular/core';
import { Chapter, ChapterInfo } from '../models/chapter.model';

@Injectable({
  providedIn: 'root'
})
export class ChapterService {
  private readonly recentChaptersData = signal<Chapter[]>([
    {
      number: 1097,
      title: 'Ginny',
      date: '19/11/2023',
    },
    {
      number: 1096,
      title: 'Kumachi',
      date: '12/11/2023',
    },
    {
      number: 1095,
      title: 'A World Not Worth Living In',
      date: '05/11/2023',
    }
  ]);

  readonly recentChapters = this.recentChaptersData.asReadonly();

  getLatestChapter(): ChapterInfo {
    return {
      number: 1098,
      title: 'Bonney\'s Birth',
      date: '26/11/2023',
    };
  }

  getNextChapter(): ChapterInfo {
    return {
      number: 1099,
      title: 'To Be Announced',
      date: '03/12/2023',
    };
  }

  addChapter(chapter: Chapter): void {
    this.recentChaptersData.update(chapters => [chapter, ...chapters]);
  }

  updateChapter(number: number, updates: Partial<Chapter>): void {
    this.recentChaptersData.update(chapters =>
      chapters.map(chapter =>
        chapter.number === number ? { ...chapter, ...updates } : chapter
      )
    );
  }
}
