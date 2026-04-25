namespace TradingProject.Persistence.Application.UseCases.Opportunities;

public record OpportunityResponse(
    int Id, string Symbol, int Score, string Signal, string Reason,
    double? TargetPct, double? StopLossPct, double Price,
    bool Acted, bool IsApproved, string? ValidationReason, DateTime CreatedAt);
