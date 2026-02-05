export interface FileReceipt {
  id: string;
  acquirer: number;
  recordType: number;
  fileName: string;
  fileHash: string;
  status: number; 
  establishment: string;
  processingDate: string;
  periodStart: string;
  periodEnd: string;
  sequence: number;
  receivedAt: string;
  backupPath: string;
  errorMessage: string | null;
}
