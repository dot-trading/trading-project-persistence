namespace TradingProject.Persistence.Api.Stubs.Models;

public record UpdateTradeRequest(
    string? Status = null,
    double? ClosePrice = null,
    double? Pnl = null,
    double? PnlPct = null,
    double? TakeProfit = null,
    double? StopLoss = null,
    string? BinanceOrderId = null);
