import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/api/api.service';
import { DashboardSummary } from '../../core/models/dashboard-summary.model';

import { BaseChartDirective, provideCharts, withDefaultRegisterables } from 'ng2-charts';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  providers: [provideCharts(withDefaultRegisterables())],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  loading = true;
  error: string | null = null;
  summary: DashboardSummary | null = null;

  barChartData: any = null;
  barChartOptions: any = { responsive: true };

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.getDashboardSummary().subscribe({
      next: (data: DashboardSummary) => {
        this.summary = data;

        this.barChartData = {
          labels: ['Arquivos'],
          datasets: [
            { label: 'Recepcionados', data: [data.receivedCount] },
            { label: 'Não Recepcionados', data: [data.notReceivedCount] }
          ]
        };

        this.loading = false;
      },
      error: (err: unknown) => {
        console.error(err);
        this.error = 'Falha ao carregar o resumo do dashboard.';
        this.loading = false;
      }
    });
  }
}
