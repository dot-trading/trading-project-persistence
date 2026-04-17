using Cortex.Mediator.Queries;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.Queries;

public class GetStatsQuery : IQuery<Stats>;

public class GetStatsQueryHandler(IDatabaseService databaseService)
    : IQueryHandler<GetStatsQuery, Stats>
{
    public Task<Stats> Handle(GetStatsQuery query, CancellationToken cancellationToken)
        => Task.FromResult(databaseService.GetStats());
}
