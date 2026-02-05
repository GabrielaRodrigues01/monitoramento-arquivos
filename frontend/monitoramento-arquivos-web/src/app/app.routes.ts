import { Routes } from '@angular/router';
import { ShellComponent } from './layout/shell/shell.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { FileReceiptsComponent } from './pages/file-receipts/file-receipts.component';

export const routes: Routes = [
  {
    path: '',
    component: ShellComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'arquivos', component: FileReceiptsComponent },
    ],
  },
];
