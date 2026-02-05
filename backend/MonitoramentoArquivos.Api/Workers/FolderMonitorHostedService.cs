using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MonitoramentoArquivos.Application.Services;
using MonitoramentoArquivos.Infrastructure.Repositories;

namespace MonitoramentoArquivos.Api.Workers
{
    public class FolderMonitorHostedService : BackgroundService
    {
        private readonly ILogger<FolderMonitorHostedService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly FileIngestionOptions _options;

        private FileSystemWatcher? _watcher;

        private static readonly ConcurrentDictionary<string, byte> _processing = new();

        public FolderMonitorHostedService(
            ILogger<FolderMonitorHostedService> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<FileIngestionOptions> options)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _options = options.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            EnsureFolders();

            _watcher = new FileSystemWatcher(_options.InboxPath, _options.FileFilter)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.CreationTime
            };

            _watcher.Created += (_, e) =>
            {
                _ = Task.Run(() => OnCreated(e.FullPath, stoppingToken), stoppingToken);
            };

            _logger.LogInformation("Monitoramento iniciado em {Inbox}", _options.InboxPath);

            return Task.CompletedTask;
        }

        private async Task OnCreated(string fullPath, CancellationToken ct)
        {
            if (!_processing.TryAdd(fullPath, 0))
            {
                _logger.LogInformation("Ignorando evento duplicado para {Path}", fullPath);
                return;
            }

            try
            {
                await WaitFileReady(fullPath, ct);

                if (!File.Exists(fullPath))
                {
                    _logger.LogWarning("Arquivo não existe mais no inbox (provavelmente já movido): {Path}", fullPath);
                    return;
                }

                var fileName = Path.GetFileName(fullPath);
                var content = await File.ReadAllTextAsync(fullPath, ct);
                var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                _logger.LogInformation("Processando arquivo {File} ({Lines} linhas)", fileName, lines.Length);

                using var scope = _scopeFactory.CreateScope();
                var ingestionService = scope.ServiceProvider.GetRequiredService<FileReceiptIngestionService>();
                var repository = scope.ServiceProvider.GetRequiredService<IFileReceiptRepository>();

                var result = ingestionService.ProcessLines(lines, fileName, content);

                var hash = result.Records.FirstOrDefault()?.FileHash;
                if (string.IsNullOrWhiteSpace(hash))
                {
                    _logger.LogWarning("Não foi possível obter FileHash para deduplicação (arquivo {File})", fileName);
                }
                else
                {
                    var exists = await repository.ExistsByHashAsync(hash);
                    if (exists)
                    {
                        _logger.LogWarning("Arquivo duplicado (hash={Hash}). Movendo para backup.", hash);

                        MoveFileIfExists(fullPath, _options.BackupPath, fileName);
                        return;
                    }
                }

                var destinationFolder = result.Success ? _options.BackupPath : _options.RejectedPath;

                var finalPath = MoveFileIfExists(fullPath, destinationFolder, fileName);

                
                if (!string.IsNullOrWhiteSpace(finalPath))
                {
                    foreach (var r in result.Records)
                        r.BackupPath = finalPath;
                }

                await repository.AddRangeAsync(result.Records);
                await repository.SaveChangesAsync();

                _logger.LogInformation("Arquivo finalizado. Sucesso={Success}. Path={Path}", result.Success, finalPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar arquivo {Path}", fullPath);

                var fileName = Path.GetFileName(fullPath);
                MoveFileIfExists(fullPath, _options.RejectedPath, fileName);
            }
            finally
            {
                _processing.TryRemove(fullPath, out _);
            }
        }

        private void EnsureFolders()
        {
            Directory.CreateDirectory(_options.InboxPath);
            Directory.CreateDirectory(_options.BackupPath);
            Directory.CreateDirectory(_options.RejectedPath);
        }

        private static async Task WaitFileReady(string path, CancellationToken ct)
        {
            const int maxAttempts = 20;
            const int delayMs = 200;

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                ct.ThrowIfCancellationRequested();

                if (!File.Exists(path))
                {
                    await Task.Delay(delayMs, ct);
                    continue;
                }

                try
                {
                    using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    if (stream.Length >= 1) return;
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }

                await Task.Delay(delayMs, ct);
            }

            throw new IOException($"Arquivo não ficou pronto a tempo: {path}");
        }

        /// <summary>
        /// Move o arquivo para a pasta destino e retorna o caminho final.
        /// Se o arquivo não existir mais, retorna string.Empty.
        /// </summary>
        private string MoveFileIfExists(string source, string destinationFolder, string fileName)
        {
            if (!File.Exists(source))
            {
                _logger.LogWarning("Não foi possível mover: arquivo não existe mais: {Source}", source);
                return string.Empty;
            }

            var destinationPath = Path.Combine(
                destinationFolder,
                $"{DateTime.Now:yyyyMMdd_HHmmss}_{fileName}");

            File.Move(source, destinationPath, true);

            return destinationPath;
        }

        public override void Dispose()
        {
            _watcher?.Dispose();
            base.Dispose();
        }
    }
}
