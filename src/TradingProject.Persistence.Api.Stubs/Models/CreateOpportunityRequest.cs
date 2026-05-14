namespace TradingProject.Persistence.Api.Stubs.Models;

public record CreateOpportunityRequest(
    string Symbol,
    int Score,
    string Signal,
    string Reason,
    double Price,
    double? TargetPct = null,
    double? StopLossPct = null,
    bool IsApproved = true,
    string? ValidationReason = null);
