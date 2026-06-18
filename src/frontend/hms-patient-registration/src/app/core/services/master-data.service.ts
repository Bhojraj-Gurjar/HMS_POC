import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, of, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

export interface MasterDataItem {
  code: string;
  displayName: string;
  parentCode: string | null;
}

interface MasterDataItemApi {
  code: string;
  displayName: string;
  parentCode: string | null;
}

@Injectable({ providedIn: 'root' })
export class MasterDataService {
  private readonly http = inject(HttpClient);
  private readonly cache = new Map<string, MasterDataItem[]>();

  getByType(type: string, parentCode?: string): Observable<MasterDataItem[]> {
    const key = `${type}:${parentCode ?? ''}`;
    const cached = this.cache.get(key);
    if (cached) {
      return of(cached);
    }

    const url = `${environment.apiUrl}/masterdata/${type}`;
    const params = parentCode ? { parentCode } : undefined;
    return this.http.get<ApiResponse<MasterDataItemApi[]>>(url, { params }).pipe(
      map((response) => response.data ?? []),
      tap((items) => this.cache.set(key, items)),
    );
  }

  clearCache(type?: string): void {
    if (!type) {
      this.cache.clear();
      return;
    }
    for (const key of [...this.cache.keys()]) {
      if (key.startsWith(`${type}:`)) {
        this.cache.delete(key);
      }
    }
  }
}
