import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PaginatedList } from '../models/api-response.model';
import {
  CreatePatientRequest,
  DashboardStats,
  PatientDetail,
  PatientSummary,
} from '../models/patient.model';

@Injectable({ providedIn: 'root' })
export class PatientService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/patients`;

  search(
    searchTerm?: string,
    pageNumber = 1,
    pageSize = 20,
  ): Observable<ApiResponse<PaginatedList<PatientSummary>>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);
    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }
    return this.http.get<ApiResponse<PaginatedList<PatientSummary>>>(this.baseUrl, { params });
  }

  getById(id: string): Observable<ApiResponse<PatientDetail>> {
    return this.http.get<ApiResponse<PatientDetail>>(`${this.baseUrl}/${id}`);
  }

  create(request: CreatePatientRequest): Observable<ApiResponse<PatientDetail>> {
    return this.http.post<ApiResponse<PatientDetail>>(this.baseUrl, request);
  }

  update(id: string, request: CreatePatientRequest): Observable<ApiResponse<PatientDetail>> {
    return this.http.put<ApiResponse<PatientDetail>>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.baseUrl}/${id}`);
  }

  getDashboardStats(): Observable<ApiResponse<DashboardStats>> {
    return this.http.get<ApiResponse<DashboardStats>>(`${environment.apiUrl}/dashboard/stats`);
  }
}
