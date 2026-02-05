using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonitoramentoArquivos.Domain.Entities;

namespace MonitoramentoArquivos.Infrastructure.Persistence.Configurations
{
    public class FileReceiptConfiguration : IEntityTypeConfiguration<FileReceipt>
    {
        public void Configure(EntityTypeBuilder<FileReceipt> builder)
        {
            builder.ToTable("FileReceipts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.FileHash)
                .HasMaxLength(64) // SHA256 hex
                .IsRequired();

            builder.HasIndex(x => x.FileHash).IsUnique();

            builder.Property(x => x.RecordType).IsRequired();

            builder.Property(x => x.Acquirer)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Establishment)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.BackupPath)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(1000);

            builder.Property(x => x.ReceivedAt).IsRequired();
            builder.Property(x => x.ProcessingDate).IsRequired();
            builder.Property(x => x.PeriodStart).IsRequired();
            builder.Property(x => x.PeriodEnd).IsRequired();
            builder.Property(x => x.Sequence).IsRequired();
        }
    }
}
