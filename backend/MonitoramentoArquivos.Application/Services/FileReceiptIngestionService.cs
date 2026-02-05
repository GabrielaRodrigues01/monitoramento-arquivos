using MonitoramentoArquivos.Application.Contracts;
using MonitoramentoArquivos.Application.Models;
using MonitoramentoArquivos.Domain.Entities;
using MonitoramentoArquivos.Domain.Enums;
using System.Security.Cryptography;
using System.Text;

namespace MonitoramentoArquivos.Application.Services
{
    public class FileReceiptIngestionService
    {
        private readonly IFileReceiptParser _parser;

        public FileReceiptIngestionService(IFileReceiptParser parser)
        {
            _parser = parser;
        }

        /// <summary>
        /// Processa conteúdo de arquivo (linhas) e devolve o resultado.
        /// Não faz I/O nem banco: somente regra e parse (fácil de testar).
        /// </summary>
        public IngestionResult ProcessLines(IEnumerable<string> lines, string fileName, string fileContentForHash)
        {
            var result = new IngestionResult();
            var fileHash = ComputeSha256(fileContentForHash);

            int total = 0;
            int parsed = 0;
            int errors = 0;

            foreach (var line in lines)
            {
                total++;

                if (string.IsNullOrWhiteSpace(line))
                {
                    errors++;
                    continue;
                }

                try
                {
                    var record = _parser.Parse(line, fileName, fileHash);
                    record.Status = FileReceiptStatus.Received;
                    record.ReceivedAt = DateTime.UtcNow;

                    result.Records.Add(record);
                    parsed++;
                }
                catch (Exception ex)
                {
                    errors++;

                    // Registra uma “linha inválida” como NotReceived para rastreabilidade
                    result.Records.Add(new FileReceipt
                    {
                        Id = Guid.NewGuid(),
                        FileName = fileName,
                        FileHash = fileHash,
                        Status = FileReceiptStatus.NotReceived,
                        ReceivedAt = DateTime.UtcNow,
                        ErrorMessage = ex.Message
                    });
                }
            }

            result.TotalLines = total;
            result.ParsedLines = parsed;
            result.ErrorLines = errors;
            result.Success = errors == 0;
            result.ErrorMessage = errors > 0 ? "Arquivo contém linhas inválidas / layout incorreto." : null;

            return result;
        }

        private static string ComputeSha256(string content)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(content);
            var hashBytes = sha.ComputeHash(bytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}