using Microsoft.EntityFrameworkCore;
using MonitoramentoArquivos.Domain.Entities;

namespace MonitoramentoArquivos.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<FileReceipt> FileReceipts => Set<FileReceipt>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
