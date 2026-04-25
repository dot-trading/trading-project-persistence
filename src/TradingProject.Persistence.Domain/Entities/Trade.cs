namespace TradingProject.Persistence.Domain.Entities;

public class Trade
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Side { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double Price { get; set; }
    public double Quantity { get; set; }
    public double Value { get; set; }
    public double? StopLoss { get; set; }
    public double? TakeProfit { get; set; }
    public int? AiScore { get; set; }
    public double? ClosePrice { get; set; }
    public double? Pnl { get; set; }
    public double? PnlPct { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CloseAt { get; set; }
}
