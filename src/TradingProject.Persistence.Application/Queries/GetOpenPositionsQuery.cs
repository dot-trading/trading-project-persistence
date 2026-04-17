using Cortex.Mediator.Queries;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.Queries;

public class GetOpenPositionsQuery : IQuery<List<OpenPosition>>;

public class GetOpenPositionsQueryHandler(IDatabaseService databaseService)
    : IQueryHandler<GetOpenPositionsQuery, List<OpenPosition>>
{
    public Task<List<OpenPosition>> Handle(GetOpenPositionsQuery query, CancellationToken cancellationToken)
        => Task.FromResult(databaseService.GetOpenPositions());
}
