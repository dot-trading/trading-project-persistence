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

    [HttpPost("trades/open")]
    public async Task<IActionResult> LogTradeOpen([FromBody] OpenPosition trade, CancellationToken ct)
    {
        await mediator.SendCommandAsync(new LogTradeOpenCommand(trade), ct);
        return Ok();
    }

    [HttpPost("trades/close")]
    public async Task<IActionResult> LogTradeClose([FromBody] LogTradeCloseRequest req, CancellationToken ct)
    {
        await mediator.SendCommandAsync(new LogTradeCloseCommand(req.TradeId, req.ClosePrice, req.PnlUsdt, req.PnlPct, req.Reason), ct);
        return Ok();
    }

    [HttpPost("trades/takeprofit")]
    public async Task<IActionResult> UpdateTakeProfit([FromBody] UpdateTakeProfitRequest req, CancellationToken ct)
    {
        await mediator.SendCommandAsync(new UpdateTakeProfitCommand(req.TradeId, req.TakeProfit), ct);
        return Ok();
    }

    [HttpPost("opportunities")]
    public async Task<IActionResult> LogOpportunity([FromBody] OpportunityData opportunity, CancellationToken ct)
    {
        await mediator.SendCommandAsync(new LogOpportunityCommand(opportunity), ct);
        return Ok();
    }

    [HttpPost("portfolio/snapshot")]
    public async Task<IActionResult> LogPortfolioSnapshot([FromBody] PortfolioData portfolio, CancellationToken ct)
    {
        await mediator.SendCommandAsync(new LogPortfolioSnapshotCommand(portfolio), ct);
        return Ok();
    }
}

public record LogTradeCloseRequest(int TradeId, double ClosePrice, double PnlUsdt, double PnlPct, string Reason);
public record UpdateTakeProfitRequest(int TradeId, double TakeProfit);
