using TradingProject.Persistence.Application.Common.Enums;
using TradingProject.Persistence.Application.Common.Models;

namespace TradingProject.Persistence.Application.Abstractions;

public interface IDatabaseService
{
    Task<PnlSummaryItem> GetPnlSummaryAsync(PnlSummaryType pnlSummaryType, CancellationToken ct = default);
    Task<PnlSummary> GetPnlSummary(PnlSummaryType? type = null, CancellationToken ct = default);
    Task<int> GetOpenPositionsCount(CancellationToken ct = default);
    Task<double> GetDailyPnl(CancellationToken ct = default);
    Task<double> GetTotalPnl(CancellationToken ct = default);
    Task<Stats> GetStats(CancellationToken ct = default);
    Task<List<OpenPosition>> GetOpenPositions(CancellationToken ct = default);
    Task<List<ClosedTrade>> GetLastTrades(int limit = 5, CancellationToken ct = default);
    Task LogTradeOpen(OpenPosition trade, CancellationToken ct = default);
    Task LogTradeClose(int tradeId, double closePrice, double pnlUsdt, double pnlPct, string reason, CancellationToken ct = default);
    Task UpdateTakeProfit(int tradeId, double takeProfit, CancellationToken ct = default);
    Task LogOpportunity(OpportunityData opportunity, CancellationToken ct = default);
    Task LogPortfolioSnapshot(PortfolioData portfolio, CancellationToken ct = default);
}


