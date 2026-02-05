namespace MonitoramentoArquivos.Api.Workers
{
    public class FileIngestionOptions
    {
        public string InboxPath { get; set; } = string.Empty;
        public string BackupPath { get; set; } = string.Empty;
        public string RejectedPath { get; set; } = string.Empty;
        public string FileFilter { get; set; } = "*.txt";
    }
}