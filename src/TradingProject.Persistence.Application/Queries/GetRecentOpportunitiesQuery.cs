using Cortex.Mediator.Queries;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Application.Common.Models;

namespace TradingProject.Persistence.Application.Queries;

public record GetRecentOpportunitiesQuery(int Hours) : IQuery<List<OpportunityData>>;

public class GetRecentOpportunitiesQueryHandler(IDatabaseService databaseService) 
    : IQueryHandler<GetRecentOpportunitiesQuery, List<OpportunityData>>
{
    public async Task<List<OpportunityData>> Handle(GetRecentOpportunitiesQuery query, CancellationToken ct)
    {
        return await databaseService.GetRecentOpportunities(query.Hours, ct);
    }
}
