using TradingProject.Persistence.Application.Common.Enums;

namespace TradingProject.Persistence.Application.Common.Models;

public record PnlSummary(
    PnlSummaryItem? Today = null,
    PnlSummaryItem? Yesterday = null,
    PnlSummaryItem? ThisWeek = null,
    PnlSummaryItem? ThisMonth = null,
    PnlSummaryItem? ThisYear = null,
    PnlSummaryItem? Total = null);

public record PnlSummaryItem(decimal Value, PnlSummaryType PnlSummaryType);

public record OpportunityData(string Symbol, int Score, string Reason, double Price);
public record PortfolioData(double FreeUsdt, double TotalUsdt, List<PortfolioPositionData> OpenPositions);
public record PortfolioPositionData(string Symbol, double UsdtValue);

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