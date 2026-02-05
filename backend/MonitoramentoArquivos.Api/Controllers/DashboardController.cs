using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonitoramentoArquivos.Infrastructure.Persistence;

namespace MonitoramentoArquivos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> Summary([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var query = _db.FileReceipts.AsNoTracking().AsQueryable();

            if (from.HasValue)
                query = query.Where(x => x.ReceivedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.ReceivedAt <= to.Value);

            var total = await query.CountAsync();

            var byAcquirer = await query
                .GroupBy(x => x.Acquirer)
                .Select(g => new
                {
                    Acquirer = g.Key,
                    Total = g.Count(),
                    Received = g.Count(x => x.Status == Domain.Enums.FileReceiptStatus.Received),
                    NotReceived = g.Count(x => x.Status == Domain.Enums.FileReceiptStatus.NotReceived)
                })
                .ToListAsync();

            return Ok(new
            {
                Total = total,
                ByAcquirer = byAcquirer
            });
        }
    }
}
