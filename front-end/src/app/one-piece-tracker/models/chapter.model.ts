export interface Chapter {
  number: number;
  title: string;
  date: string;
  status: 'Released' | 'Upcoming' | 'TBA';
}

export interface ChapterInfo {
  number: number;
  title: string;
  date: string;
  status: 'Released' | 'Upcoming';
  statusLabel: string;
}
