using Cortex.Mediator.Queries;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Application.Common.Models;

namespace TradingProject.Persistence.Application.Queries;

public class GetOpenPositionsQuery : IQuery<List<OpenPosition>>;

public class GetOpenPositionsQueryHandler(IDatabaseService databaseService)
    : IQueryHandler<GetOpenPositionsQuery, List<OpenPosition>>
{
    public async Task<List<OpenPosition>> Handle(GetOpenPositionsQuery query, CancellationToken cancellationToken)
        => await databaseService.GetOpenPositions(cancellationToken);
}
