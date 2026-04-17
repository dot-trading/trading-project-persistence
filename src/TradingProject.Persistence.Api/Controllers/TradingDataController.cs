using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using TradingProject.Persistence.Application.Queries;

namespace TradingProject.Persistence.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TradingDataController(IMediator mediator) : ControllerBase
{
    [HttpGet("positions/open/count")]
    public async Task<IActionResult> GetOpenPositionsCount(CancellationToken ct)
        => Ok(await mediator.SendQueryAsync(new GetOpenPositionsCountQuery(), ct));

    [HttpGet("pnl/daily")]
    public async Task<IActionResult> GetDailyPnl(CancellationToken ct)
        => Ok(await mediator.SendQueryAsync(new GetDailyPnlQuery(), ct));

    [HttpGet("pnl/total")]
    public async Task<IActionResult> GetTotalPnl(CancellationToken ct)
        => Ok(await mediator.SendQueryAsync(new GetTotalPnlQuery(), ct));

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken ct)
        => Ok(await mediator.SendQueryAsync(new GetStatsQuery(), ct));

    [HttpGet("pnl/summary")]
    public async Task<IActionResult> GetPnlSummary(CancellationToken ct)
        => Ok(await mediator.SendQueryAsync(new GetPnlSummaryQuery(), ct));

    [HttpGet("positions/open")]
    public async Task<IActionResult> GetOpenPositions(CancellationToken ct)
        => Ok(await mediator.SendQueryAsync(new GetOpenPositionsQuery(), ct));

    [HttpGet("trades/last")]
    public async Task<IActionResult> GetLastTrades(CancellationToken ct, [FromQuery] int limit = 5)
        => Ok(await mediator.SendQueryAsync(new GetLastTradesQuery(limit), ct));
}
