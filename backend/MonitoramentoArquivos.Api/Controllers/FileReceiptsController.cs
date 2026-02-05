using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonitoramentoArquivos.Domain.Enums;
using MonitoramentoArquivos.Infrastructure.Persistence;

namespace MonitoramentoArquivos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileReceiptsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public FileReceiptsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] AcquirerType? acquirer,
            [FromQuery] FileReceiptStatus? status,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var query = _db.FileReceipts.AsNoTracking().AsQueryable();

            if (acquirer.HasValue)
                query = query.Where(x => x.Acquirer == acquirer.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            if (from.HasValue)
                query = query.Where(x => x.ReceivedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.ReceivedAt <= to.Value);

            var data = await query
                .OrderByDescending(x => x.ReceivedAt)
                .Take(200)
                .Select(x => new
                {
                    x.Id,
                    x.Acquirer,
                    x.RecordType,
                    x.FileName,
                    x.FileHash,
                    x.Status,
                    x.Establishment,
                    x.ProcessingDate,
                    x.PeriodStart,
                    x.PeriodEnd,
                    x.Sequence,
                    x.ReceivedAt,
                    x.BackupPath,
                    x.ErrorMessage
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}
