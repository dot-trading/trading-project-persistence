using Asp.Versioning;
using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using TradingProject.Persistence.Application.Queries;
using TradingProject.Persistence.Application.Commands;
using TradingProject.Persistence.Application.Common.Models;
using TradingProject.Persistence.Application.UseCases.Opportunities.CreateOpportunity;

namespace TradingProject.Persistence.Api.Controllers.V1;

[Obsolete("This controller is deprecated. Please use the specialized controllers (Trades, Opportunities, PortfolioSnapshots) instead.")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tradingdata")]
public class TradingDataController(IMediator mediator) : ControllerBase
{
    [HttpGet("positions/open/count")]
    public async Task<IActionResult> GetOpenPositionsCount(CancellationToken ct)
        => Ok(await mediator.SendQueryAsync(new GetOpenPositionsCountQuery(), ct));

    [HttpGet("pnl/daily")]
    public async Task<IActionResult> GetDailyPnl([FromQuery] string? quoteAsset, CancellationToken ct)
        => Ok(await mediator.SendQueryAsync(new GetDailyPnlQuery(quoteAsset), ct));

    [HttpGet("pnl/total")]
    public async Task<IActionResult> GetTotalPnl([FromQuery] string? quoteAsset, CancellationToken ct)
        => Ok(await mediator.SendQueryAsync(new GetTotalPnlQuery(quoteAsset), ct));

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats([FromQuery] string? quoteAsset, CancellationToken ct)
        => Ok(await mediator.SendQueryAsync(new GetStatsQuery(quoteAsset), ct));

    [HttpGet("pnl/summary")]
    public async Task<IActionResult> GetPnlSummary([FromQuery] string? quoteAsset, CancellationToken ct)
        => Ok(await mediator.SendQueryAsync(new GetPnlSummaryQuery(quoteAsset: quoteAsset), ct));

    [HttpGet("positions/open")]
    public async Task<IActionResult> GetOpenPositions(CancellationToken ct)
    {
        var positions = await mediator.SendQueryAsync(new GetOpenPositionsQuery(), ct);
        return Ok(positions.Select(p => new OpenPositionResponse(p.Id, p.Symbol, p.Side, p.Entry, p.Quantity, p.Value, p.StopLoss, p.TakeProfit, p.AiScore, p.CreatedAt)));
    }

    [HttpGet("trades/last")]
    public async Task<IActionResult> GetLastTrades(CancellationToken ct, [FromQuery] int limit = 5)
        => Ok(await mediator.SendQueryAsync(new GetLastTradesQuery(limit), ct));

    [HttpPost("trades/open")]
    public async Task<IActionResult> LogTradeOpen([FromBody] LogTradeOpenRequest req, CancellationToken ct)
    {
        var trade = new OpenPosition(req.Id, req.Symbol, req.Side, req.Entry, req.Quantity, req.Value, req.StopLoss, req.TakeProfit, req.AiScore, req.CreatedAt);
        await mediator.SendCommandAsync(new LogTradeOpenCommand(trade), ct);
        return Ok();
    }

    [HttpPost("trades/close")]
    public async Task<IActionResult> LogTradeClose([FromBody] LogTradeCloseRequest req, CancellationToken ct)
    {
        await mediator.SendCommandAsync(new LogTradeCloseCommand(req.TradeId, req.ClosePrice, req.Pnl, req.PnlPct, req.Reason), ct);
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
        await mediator.SendCommandAsync(
            new CreateOpportunityCommand(
                new CreateOpportunityRequest(
                    opportunity.Symbol, opportunity.Score, opportunity.Signal, opportunity.Reason, opportunity.Price,
                    TargetPct: null, StopLossPct: null,
                    opportunity.IsApproved, opportunity.ValidationReason)), ct);
        return Ok();
    }

    [HttpPost("portfolio/snapshot")]
    public async Task<IActionResult> LogPortfolioSnapshot([FromBody] PortfolioSnapshotRequest req, CancellationToken ct)
    {
        var portfolio = new PortfolioData
        {
            Free = req.Free,
            Total = req.Total,
            DailyPnl = req.DailyPnl,
            TotalPnl = req.TotalPnl,
            OpenPositions = req.OpenPositions
                .Select(p => new PortfolioPositionData { Symbol = p.Symbol, Value = p.Value })
                .ToList()
        };
        await mediator.SendCommandAsync(new LogPortfolioSnapshotCommand(portfolio), ct);
        return Ok();
    }
}

public record LogTradeOpenRequest(int Id, string Symbol, string Side, double Entry, double Quantity, double Value, double? StopLoss, double? TakeProfit, int? AiScore, DateTime CreatedAt);
public record LogTradeCloseRequest(int TradeId, double ClosePrice, double Pnl, double PnlPct, string Reason);
public record UpdateTakeProfitRequest(int TradeId, double TakeProfit);
public record OpenPositionResponse(int Id, string Symbol, string Side, double Entry, double Quantity, double Value, double? StopLoss, double? TakeProfit, int? AiScore, DateTime CreatedAt);

public class PortfolioSnapshotRequest
{
    [JsonPropertyName("free")]
    public double Free { get; set; }

    [JsonPropertyName("total")]
    public double Total { get; set; }

    [JsonPropertyName("dailyPnl")]
    public double DailyPnl { get; set; }

    [JsonPropertyName("totalPnl")]
    public double TotalPnl { get; set; }

    [JsonPropertyName("openPositions")]
    public List<PortfolioPositionRequest> OpenPositions { get; set; } = new();
}

public class PortfolioPositionRequest
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public double Value { get; set; }
}
