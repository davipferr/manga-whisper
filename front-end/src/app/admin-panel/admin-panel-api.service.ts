import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  AdminCheckerListResponseDto,
  AdminCheckerResultDto,
  JobRunListResponseDto,
  JobRunResultDto,
  SaveScrapingJobRequestDto,
  ScrapingJobListResponseDto,
  ScrapingJobResultDto,
  ScrapingJobType,
  UpdateCheckerRequestDto,
  WorkerStatusResponseDto,
} from './admin-panel.model';

@Injectable({
  providedIn: 'root'
})
export class AdminPanelApiService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  getCheckers(): Observable<AdminCheckerListResponseDto> {
    return this.http.get<AdminCheckerListResponseDto>(`${this.apiUrl}/admin/checkers`);
  }

  updateChecker(id: number, request: UpdateCheckerRequestDto): Observable<AdminCheckerResultDto> {
    return this.http.patch<AdminCheckerResultDto>(`${this.apiUrl}/admin/checkers/${id}`, request);
  }

  // "Run now": the worker picks the run up on its next poll (up to 30s)
  requestRun(checkerId: number, type: ScrapingJobType): Observable<JobRunResultDto> {
    return this.http.post<JobRunResultDto>(`${this.apiUrl}/admin/checkers/${checkerId}/runs`, { type });
  }

  getJobs(): Observable<ScrapingJobListResponseDto> {
    return this.http.get<ScrapingJobListResponseDto>(`${this.apiUrl}/admin/jobs`);
  }

  createJob(request: SaveScrapingJobRequestDto): Observable<ScrapingJobResultDto> {
    return this.http.post<ScrapingJobResultDto>(`${this.apiUrl}/admin/jobs`, request);
  }

  updateJob(id: number, request: SaveScrapingJobRequestDto): Observable<ScrapingJobResultDto> {
    return this.http.put<ScrapingJobResultDto>(`${this.apiUrl}/admin/jobs/${id}`, request);
  }

  deleteJob(id: number): Observable<ScrapingJobResultDto> {
    return this.http.delete<ScrapingJobResultDto>(`${this.apiUrl}/admin/jobs/${id}`);
  }

  getRuns(take = 20): Observable<JobRunListResponseDto> {
    return this.http.get<JobRunListResponseDto>(`${this.apiUrl}/admin/runs`, { params: new HttpParams().set('take', take) });
  }

  getWorkerStatus(): Observable<WorkerStatusResponseDto> {
    return this.http.get<WorkerStatusResponseDto>(`${this.apiUrl}/admin/worker-status`);
  }
}
