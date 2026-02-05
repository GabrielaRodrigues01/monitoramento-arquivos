using MonitoramentoArquivos.Application.Contracts;
using MonitoramentoArquivos.Application.Exceptions;
using MonitoramentoArquivos.Domain.Entities;
using MonitoramentoArquivos.Domain.Enums;
using System.Globalization;

namespace MonitoramentoArquivos.Application.Services
{
    
    public class FileReceiptLineParser : IFileReceiptParser
    {
        public FileReceipt Parse(string line, string fileName, string fileHash)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new LayoutParseException("Linha vazia.");

            var recordTypeStr = Slice1Based(line, 1, 1);
            if (!int.TryParse(recordTypeStr, out var recordType) || (recordType != 0 && recordType != 1))
                throw new LayoutParseException($"Tipo de Registro inválido: '{recordTypeStr}'.");

            return recordType switch
            {
                0 => ParseUfCard(line, fileName, fileHash),
                1 => ParseFagammonCard(line, fileName, fileHash),
                _ => throw new LayoutParseException($"Tipo de Registro não suportado: {recordType}.")
            };
        }

        
        private static FileReceipt ParseUfCard(string line, string fileName, string fileHash)
        {
            EnsureMinLength(line, 50, "UfCard");

            var estabelecimento = Slice1Based(line, 2, 11);
            var dataProc = ParseDate(Slice1Based(line, 12, 19), "Data Processamento");
            var periodoIni = ParseDate(Slice1Based(line, 20, 27), "Período Inicial");
            var periodoFim = ParseDate(Slice1Based(line, 28, 35), "Período Final");
            var sequencia = ParseInt(Slice1Based(line, 36, 42), "Sequência");
            var empresa = Slice1Based(line, 43, 50).Trim();

            if (!empresa.Equals("UfCard", StringComparison.OrdinalIgnoreCase))
                throw new LayoutParseException($"Empresa esperada 'UfCard', mas veio '{empresa}'.");

            return new FileReceipt
            {
                Id = Guid.NewGuid(),
                Acquirer = AcquirerType.UfCard,
                RecordType = 0,
                FileName = fileName,
                FileHash = fileHash,
                Establishment = estabelecimento,
                ProcessingDate = dataProc,
                PeriodStart = periodoIni,
                PeriodEnd = periodoFim,
                Sequence = sequencia,
            };
        }

        
        private static FileReceipt ParseFagammonCard(string line, string fileName, string fileHash)
        {
            EnsureMinLength(line, 36, "FagammonCard");

            var dataProc = ParseDate(Slice1Based(line, 2, 9), "Data Processamento");
            var estabelecimento = Slice1Based(line, 10, 17);
            var empresa = Slice1Based(line, 18, 29).Trim();
            var sequencia = ParseInt(Slice1Based(line, 30, 36), "Sequência");

            if (!empresa.Equals("FagammonCard", StringComparison.OrdinalIgnoreCase))
                throw new LayoutParseException($"Empresa esperada 'FagammonCard', mas veio '{empresa}'.");

            return new FileReceipt
            {
                Id = Guid.NewGuid(),
                Acquirer = AcquirerType.FagammonCard,
                RecordType = 1,
                FileName = fileName,
                FileHash = fileHash,
                Establishment = estabelecimento,
                ProcessingDate = dataProc,
                PeriodStart = dataProc, 
                PeriodEnd = dataProc,   
                Sequence = sequencia,
            };
        }

        private static string Slice1Based(string input, int startInclusive, int endInclusive)
        {
            var startZero = startInclusive - 1;
            var length = endInclusive - startInclusive + 1;

            if (startZero < 0 || length <= 0)
                throw new LayoutParseException("Parâmetros de corte inválidos.");

            if (input.Length < startZero + length)
                throw new LayoutParseException($"Linha menor que o esperado. Tamanho={input.Length}, esperado >= {startZero + length}.");

            return input.Substring(startZero, length);
        }

        private static void EnsureMinLength(string input, int minLength, string layoutName)
        {
            if (input.Length < minLength)
                throw new LayoutParseException($"Layout {layoutName} inválido: tamanho da linha {input.Length} menor que {minLength}.");
        }

        private static DateTime ParseDate(string yyyymmdd, string fieldName)
        {
            if (!DateTime.TryParseExact(yyyymmdd, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                throw new LayoutParseException($"{fieldName} inválida: '{yyyymmdd}'.");
            return dt;
        }

        private static int ParseInt(string raw, string fieldName)
        {
            var trimmed = raw.Trim();
            if (!int.TryParse(trimmed, out var value))
                throw new LayoutParseException($"{fieldName} inválido: '{raw}'.");
            return value;
        }
    }
}