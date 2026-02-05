using Microsoft.EntityFrameworkCore;
using MonitoramentoArquivos.Domain.Entities;
using MonitoramentoArquivos.Infrastructure.Persistence;

namespace MonitoramentoArquivos.Infrastructure.Repositories
{
    public class FileReceiptRepository : IFileReceiptRepository
    {
        private readonly AppDbContext _context;

        public FileReceiptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<FileReceipt> receipts)
        {
            await _context.FileReceipts.AddRangeAsync(receipts);
        }

        public async Task<bool> ExistsByHashAsync(string fileHash)
        {
            return await _context.FileReceipts
                .AnyAsync(x => x.FileHash == fileHash);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}