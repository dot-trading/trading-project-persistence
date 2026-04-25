using Cortex.Mediator.Commands;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Application.UseCases.Opportunities.CreateOpportunity;

public record CreateOpportunityRequest(
    string Symbol, int Score, string Signal, string Reason, double Price,
    double? TargetPct = null, double? StopLossPct = null,
    bool IsApproved = true, string? ValidationReason = null);

public record CreateOpportunityCommand(CreateOpportunityRequest Opportunity) : ICommand<OpportunityResponse>;

public class CreateOpportunityCommandHandler(ITradingDbContext context)
    : ICommandHandler<CreateOpportunityCommand, OpportunityResponse>
{
    public async Task<OpportunityResponse> Handle(CreateOpportunityCommand command, CancellationToken ct)
    {
        var req = command.Opportunity;
        var entity = new Opportunity
        {
            Symbol = req.Symbol,
            Score = req.Score,
            Signal = req.Signal,
            Reason = req.Reason,
            Price = req.Price,
            TargetPct = req.TargetPct,
            StopLossPct = req.StopLossPct,
            IsApproved = req.IsApproved,
            ValidationReason = req.ValidationReason,
            CreatedAt = DateTime.UtcNow
        };
        context.Opportunities.Add(entity);
        await context.SaveChangesAsync(ct);

        return new OpportunityResponse(
            entity.Id, entity.Symbol, entity.Score, entity.Signal, entity.Reason,
            entity.TargetPct, entity.StopLossPct, entity.Price,
            entity.Acted, entity.IsApproved, entity.ValidationReason, entity.CreatedAt);
    }
}
