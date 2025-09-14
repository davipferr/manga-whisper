export interface Chapter {
  number: number;
  title: string;
  date: string;
  status: 'Released' | 'Upcoming';
}

export interface ChapterInfo {
  number: number;
  title: string;
  date: string;
  status: 'Released' | 'Upcoming';
  statusLabel: string;
}
