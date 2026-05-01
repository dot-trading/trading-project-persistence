using Cortex.Mediator.Queries;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Application.Common.Models;

namespace TradingProject.Persistence.Application.Queries;

public record GetStatsQuery(string? QuoteAsset = null) : IQuery<Stats>;

public class GetStatsQueryHandler(IDatabaseService databaseService)
    : IQueryHandler<GetStatsQuery, Stats>
{
    public async Task<Stats> Handle(GetStatsQuery query, CancellationToken cancellationToken)
        => await databaseService.GetStats(query.QuoteAsset, cancellationToken);
}
