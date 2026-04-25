using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.UseCases.PortfolioSnapshots.GetPortfolioSnapshots;

public record GetPortfolioSnapshotsQuery(int Limit = 50) : IQuery<List<PortfolioSnapshotResponse>>;

public class GetPortfolioSnapshotsQueryHandler(ITradingDbContext context)
    : IQueryHandler<GetPortfolioSnapshotsQuery, List<PortfolioSnapshotResponse>>
{
    public async Task<List<PortfolioSnapshotResponse>> Handle(GetPortfolioSnapshotsQuery query, CancellationToken ct)
    {
        return await context.PortfolioSnapshots
            .OrderByDescending(s => s.CreatedAt)
            .Take(query.Limit)
            .Select(s => new PortfolioSnapshotResponse(
                s.Id, s.Total, s.Free,
                s.PositionsCount, s.DailyPnl, s.TotalPnl, s.CreatedAt))
            .ToListAsync(ct);
    }
}
