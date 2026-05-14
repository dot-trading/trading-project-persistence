namespace TradingProject.Persistence.Api.Stubs.Models;

public record CreatePortfolioSnapshotRequest(
    double Total,
    double Free,
    int PositionsCount,
    double DailyPnl = 0,
    double TotalPnl = 0);
