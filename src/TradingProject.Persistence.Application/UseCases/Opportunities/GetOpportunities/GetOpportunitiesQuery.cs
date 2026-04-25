using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.UseCases.Opportunities.GetOpportunities;

public record GetOpportunitiesQuery(int Limit = 50, string? Symbol = null, bool? IsApproved = null)
    : IQuery<List<OpportunityResponse>>;

public class GetOpportunitiesQueryHandler(ITradingDbContext context)
    : IQueryHandler<GetOpportunitiesQuery, List<OpportunityResponse>>
{
    public async Task<List<OpportunityResponse>> Handle(GetOpportunitiesQuery query, CancellationToken ct)
    {
        var q = context.Opportunities.AsQueryable();
        if (query.Symbol is not null) q = q.Where(o => o.Symbol == query.Symbol);
        if (query.IsApproved is not null) q = q.Where(o => o.IsApproved == query.IsApproved);

        return await q
            .OrderByDescending(o => o.CreatedAt)
            .Take(query.Limit)
            .Select(o => new OpportunityResponse(
                o.Id, o.Symbol, o.Score, o.Signal, o.Reason,
                o.TargetPct, o.StopLossPct, o.Price,
                o.Acted, o.IsApproved, o.ValidationReason, o.CreatedAt))
            .ToListAsync(ct);
    }
}
