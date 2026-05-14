namespace TradingProject.Persistence.Api.Stubs.Models;

public record CreateTradeRequest(
    string Symbol,
    string Side,
    double Price,
    double Quantity,
    double Value,
    double? StopLoss = null,
    double? TakeProfit = null,
    int? AiScore = null,
    string? BinanceOrderId = null);
