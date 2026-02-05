using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoramentoArquivos.Domain.Entities;

namespace MonitoramentoArquivos.Application.Models
{
    public class IngestionResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public List<FileReceipt> Records { get; set; } = new();

        public int TotalLines { get; set; }
        public int ParsedLines { get; set; }
        public int ErrorLines { get; set; }
    }
}