namespace TradingProject.Persistence.Api.Stubs.Models;

public record PortfolioSnapshotResponse(
    int Id,
    double Total,
    double Free,
    int PositionsCount,
    double DailyPnl,
    double TotalPnl,
    DateTime CreatedAt);
