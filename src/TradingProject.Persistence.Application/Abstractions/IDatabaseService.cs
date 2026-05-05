using TradingProject.Persistence.Application.Common.Enums;
using TradingProject.Persistence.Application.Common.Models;

namespace TradingProject.Persistence.Application.Abstractions;

public interface IDatabaseService
{
    Task<PnlSummaryItem> GetPnlSummaryAsync(PnlSummaryType pnlSummaryType, string? quoteAsset = null, CancellationToken ct = default);
    Task<PnlSummary> GetPnlSummary(PnlSummaryType? type = null, string? quoteAsset = null, CancellationToken ct = default);
    Task<double> GetDailyPnl(string? quoteAsset = null, CancellationToken ct = default);
    Task<double> GetTotalPnl(string? quoteAsset = null, CancellationToken ct = default);
    Task<Stats> GetStats(string? quoteAsset = null, CancellationToken ct = default);
    Task<List<ClosedTrade>> GetLastTrades(int limit = 5, CancellationToken ct = default);
    Task LogTradeOpen(OpenPosition trade, CancellationToken ct = default);
    Task LogTradeClose(int tradeId, double closePrice, double pnlUsdt, double pnlPct, string reason, CancellationToken ct = default);
    Task UpdateTakeProfit(int tradeId, double takeProfit, CancellationToken ct = default);
    Task LogOpportunity(OpportunityData opportunity, CancellationToken ct = default);
}
