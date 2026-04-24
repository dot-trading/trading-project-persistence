namespace TradingProject.Persistence.Domain.Entities;

public class Opportunity
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Signal { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public double? TargetPct { get; set; }
    public double? StopLossPct { get; set; }
    public double Price { get; set; }
    public bool Acted { get; set; }
    public bool IsApproved { get; set; }
    public string? ValidationReason { get; set; }
    public DateTime CreatedAt { get; set; }

    public Opportunity() { }
    
    public Opportunity(string symbol, int score, string signal, string reason, double price)
    {
        Symbol = symbol;
        Score = score;
        Signal = signal;
        Reason = reason;
        Price = price;
        CreatedAt = DateTime.UtcNow;
    }
}
