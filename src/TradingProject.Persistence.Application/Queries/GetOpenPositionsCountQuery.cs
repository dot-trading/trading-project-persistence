using Cortex.Mediator.Queries;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.Queries;

public class GetOpenPositionsCountQuery : IQuery<int>;

public class GetOpenPositionsCountQueryHandler(IDatabaseService databaseService)
    : IQueryHandler<GetOpenPositionsCountQuery, int>
{
    public Task<int> Handle(GetOpenPositionsCountQuery query, CancellationToken cancellationToken)
        => Task.FromResult(databaseService.GetOpenPositionsCount());
}
