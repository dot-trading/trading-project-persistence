using TradingProject.Persistence.Application.Common.Enums;
using System.Text.Json.Serialization;

namespace TradingProject.Persistence.Application.Common.Models;

public record PnlSummary(
    PnlSummaryItem? Today = null,
    PnlSummaryItem? Yesterday = null,
    PnlSummaryItem? ThisWeek = null,
    PnlSummaryItem? ThisMonth = null,
    PnlSummaryItem? ThisYear = null,
    PnlSummaryItem? Total = null);

public record PnlSummaryItem(decimal Value, PnlSummaryType PnlSummaryType);

public class OpportunityData
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;
    
    [JsonPropertyName("score")]
    public int Score { get; set; }
    
    [JsonPropertyName("signal")]
    public string Signal { get; set; } = "SIGNAL";
    
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;
    
    [JsonPropertyName("price")]
    public double Price { get; set; }
    
    [JsonPropertyName("isApproved")]
    public bool IsApproved { get; set; } = true;
    
    [JsonPropertyName("validationReason")]
    public string? ValidationReason { get; set; }
}

public class PortfolioData
{
    [JsonPropertyName("freeUsdt")]
    public double FreeUsdt { get; set; }
    
    [JsonPropertyName("totalUsdt")]
    public double TotalUsdt { get; set; }
    
    [JsonPropertyName("openPositions")]
    public List<PortfolioPositionData> OpenPositions { get; set; } = new();
}

public class PortfolioPositionData
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;
    
    [JsonPropertyName("usdtValue")]
    public double UsdtValue { get; set; }
}

public record Stats(
    double PnlDay, double PnlWeek, double PnlMonth, double PnlTotal,
    int CountDay, int CountWeek, int CountMonth, int CountTotal,
    int Wins, double WinRate);



public record OpenPosition(
    int Id,
    string Symbol, string Side,
    double Entry, double Quantity, double UsdtValue,
    double? StopLoss, double? TakeProfit, int? AiScore,
    DateTime CreatedAt);

public record ClosedTrade(
    string Symbol, string Side,
    double Entry, double ClosePrice,
    double Pnl, double PnlPct,
    int? AiScore, DateTime OpenedAt, DateTime ClosedAt);