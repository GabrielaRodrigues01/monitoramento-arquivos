using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoramentoArquivos.Domain.Entities;

namespace MonitoramentoArquivos.Infrastructure.Repositories
{
    public interface IFileReceiptRepository
    {
        Task AddRangeAsync(IEnumerable<FileReceipt> receipts);
        Task<bool> ExistsByHashAsync(string fileHash);
        Task SaveChangesAsync();
    }
}