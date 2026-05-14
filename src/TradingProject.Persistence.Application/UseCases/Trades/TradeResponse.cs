namespace TradingProject.Persistence.Application.UseCases.Trades;

public record TradeResponse(
    int Id, string Symbol, string QuoteAsset, string Side, string Status,
    double Price, double Quantity, double Value,
    double? StopLoss, double? TakeProfit, int? AiScore,
    string? BinanceOrderId,
    double? ClosePrice, double? Pnl, double? PnlPct,
    DateTime CreatedAt, DateTime? CloseAt);
