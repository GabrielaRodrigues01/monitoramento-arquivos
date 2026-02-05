import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { FileReceipt } from '../models/file-receipt.model';
import { DashboardSummary } from '../models/dashboard-summary.model';

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private http: HttpClient) {}

  getDashboardSummary(): Observable<DashboardSummary> {
    return this.http.get<any>(`/api/Dashboard/summary`).pipe(
      map((raw) => {
        // Formato "flat" (se existir)
        if (raw && (raw.receivedCount != null || raw.notReceivedCount != null)) {
          const receivedCount = Number(raw.receivedCount ?? 0);
          const notReceivedCount = Number(raw.notReceivedCount ?? 0);

          const totalCount =
            raw.totalCount != null
              ? Number(raw.totalCount)
              : Number(receivedCount + notReceivedCount);

          return { receivedCount, notReceivedCount, totalCount };
        }

        // Formato por adquirente (seu retorno atual)
        const by: any[] = Array.isArray(raw?.byAcquirer) ? raw.byAcquirer : [];

        const receivedCount = by.reduce(
          (acc: number, x: any) => acc + Number(x?.received ?? 0),
          0
        );

        const notReceivedCount = by.reduce(
          (acc: number, x: any) => acc + Number(x?.notReceived ?? 0),
          0
        );

        const totalCount =
          raw?.total != null
            ? Number(raw.total)
            : Number(receivedCount + notReceivedCount);

        return { receivedCount, notReceivedCount, totalCount };
      })
    );
  }

  getFileReceipts(): Observable<FileReceipt[]> {
    return this.http.get<FileReceipt[]>(`/api/FileReceipts`);
  }
}
