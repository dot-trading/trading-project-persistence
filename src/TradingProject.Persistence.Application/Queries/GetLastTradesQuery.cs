using Cortex.Mediator.Queries;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.Queries;

public class GetLastTradesQuery(int limit = 5) : IQuery<List<ClosedTrade>>
{
    public int Limit { get; } = limit;
}

public class GetLastTradesQueryHandler(IDatabaseService databaseService)
    : IQueryHandler<GetLastTradesQuery, List<ClosedTrade>>
{
    public Task<List<ClosedTrade>> Handle(GetLastTradesQuery query, CancellationToken cancellationToken)
        => Task.FromResult(databaseService.GetLastTrades(query.Limit));
}
