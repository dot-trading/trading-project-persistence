using AutoMapper;
using Cortex.Mediator.Commands;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Application.UseCases.Opportunities.CreateOpportunity;

public record CreateOpportunityRequest(
    string Symbol, int Score, string Signal, string Reason, double Price,
    double? TargetPct = null, double? StopLossPct = null,
    bool IsApproved = true, string? ValidationReason = null);

public record CreateOpportunityCommand(CreateOpportunityRequest Opportunity) : ICommand<OpportunityResponse>;

public class CreateOpportunityCommandHandler(ITradingDbContext context, IMapper mapper)
    : ICommandHandler<CreateOpportunityCommand, OpportunityResponse>
{
    public async Task<OpportunityResponse> Handle(CreateOpportunityCommand command, CancellationToken ct)
    {
        var entity = mapper.Map<Opportunity>(command.Opportunity);
        
        context.Opportunities.Add(entity);
        await context.SaveChangesAsync(ct);

        return mapper.Map<OpportunityResponse>(entity);
    }
}
