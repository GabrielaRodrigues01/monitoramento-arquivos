using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoramentoArquivos.Domain.Enums;

namespace MonitoramentoArquivos.Domain.Entities
{
    public class FileReceipt
    {
        public Guid Id { get; set; }

        public AcquirerType Acquirer { get; set; }
        public int RecordType { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FileHash { get; set; } = string.Empty;

        public DateTime ReceivedAt { get; set; }

        public FileReceiptStatus Status { get; set; }

        public string Establishment { get; set; } = string.Empty;

        public DateTime ProcessingDate { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public int Sequence { get; set; }

        public string BackupPath { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}