import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

export interface MasterDataItem {
  code: string;
  label: string;
  parentCode: string | null;
}

@Injectable({ providedIn: 'root' })
export class MasterDataService {
  private readonly http = inject(HttpClient);
  private readonly cache = new Map<string, MasterDataItem[]>();

  getByType(type: string, parentCode?: string): Observable<ApiResponse<MasterDataItem[]>> {
    const url = `${environment.apiUrl}/masterdata/${type}`;
    const params = parentCode ? { parentCode } : undefined;
    return this.http.get<ApiResponse<MasterDataItem[]>>(url, { params });
  }

  getCached(type: string, parentCode?: string): Observable<MasterDataItem[]> {
    const key = `${type}:${parentCode ?? ''}`;
    const cached = this.cache.get(key);
    if (cached) {
      return new Observable((subscriber) => {
        subscriber.next(cached);
        subscriber.complete();
      });
    }
    return new Observable((subscriber) => {
      this.getByType(type, parentCode).subscribe({
        next: (response) => {
          const items = response.data ?? [];
          this.cache.set(key, items);
          subscriber.next(items);
          subscriber.complete();
        },
        error: (err) => subscriber.error(err),
      });
    });
  }
}
