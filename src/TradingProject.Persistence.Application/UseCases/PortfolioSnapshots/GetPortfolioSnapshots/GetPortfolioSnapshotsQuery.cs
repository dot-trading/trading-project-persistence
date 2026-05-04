using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.UseCases.PortfolioSnapshots.GetPortfolioSnapshots;

public record GetPortfolioSnapshotsQuery(int Limit = 50, int Page = 1) : IQuery<List<PortfolioSnapshotResponse>>;

public class GetPortfolioSnapshotsQueryHandler(ITradingDbContext context, IMapper mapper)
    : IQueryHandler<GetPortfolioSnapshotsQuery, List<PortfolioSnapshotResponse>>
{
    public async Task<List<PortfolioSnapshotResponse>> Handle(GetPortfolioSnapshotsQuery query, CancellationToken ct)
    {
        return await context.PortfolioSnapshots
            .OrderByDescending(s => s.CreatedAt)
            .Skip((query.Page - 1) * query.Limit)
            .Take(query.Limit)
            .ProjectTo<PortfolioSnapshotResponse>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}
