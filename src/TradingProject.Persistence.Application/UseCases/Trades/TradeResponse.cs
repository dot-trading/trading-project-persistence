namespace TradingProject.Persistence.Application.UseCases.Trades;

public record TradeResponse(
    int Id, string Symbol, string Side, string Status,
    double Price, double Quantity, double Value,
    double? StopLoss, double? TakeProfit, int? AiScore,
    double? ClosePrice, double? Pnl, double? PnlPct,
    DateTime CreatedAt, DateTime? CloseAt);
