namespace TradingProject.Persistence.Application.UseCases.PortfolioSnapshots;

public record PortfolioSnapshotResponse(
    int Id, double Total, double Free,
    int PositionsCount, double DailyPnl, double TotalPnl, DateTime CreatedAt);
