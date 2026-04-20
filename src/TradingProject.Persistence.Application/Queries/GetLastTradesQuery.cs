using Cortex.Mediator.Queries;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Application.Common.Models;

namespace TradingProject.Persistence.Application.Queries;

public class GetLastTradesQuery(int limit = 5) : IQuery<List<ClosedTrade>>
{
    public int Limit { get; } = limit;
}

public class GetLastTradesQueryHandler(IDatabaseService databaseService)
    : IQueryHandler<GetLastTradesQuery, List<ClosedTrade>>
{
    public async Task<List<ClosedTrade>> Handle(GetLastTradesQuery query, CancellationToken cancellationToken)
        => await databaseService.GetLastTrades(query.Limit, cancellationToken);
}
