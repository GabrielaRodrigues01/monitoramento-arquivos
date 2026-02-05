import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/api/api.service';
import { FileReceipt } from '../../core/models/file-receipt.model';

@Component({
  selector: 'app-file-receipts',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './file-receipts.component.html',
  styleUrl: './file-receipts.component.scss',
})
export class FileReceiptsComponent implements OnInit {
  loading = true;
  error: string | null = null;
  items: FileReceipt[] = [];

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.getFileReceipts().subscribe({
      next: (data: FileReceipt[]) => {
        this.items = data ?? [];
        this.loading = false;
      },
      error: (err: unknown) => {
        console.error(err);
        this.error = 'Falha ao carregar a listagem de arquivos.';
        this.loading = false;
      },
    });
  }

  statusLabel(status: number): string {
    return status === 1 ? 'Recepcionado' : 'Não Recepcionado';
  }

  statusClass(status: number): string {
    return status === 1 ? 'badge badge--ok' : 'badge badge--bad';
  }

  trackByFile(index: number, item: FileReceipt) {
    return item.fileName ?? index;
  }
}
