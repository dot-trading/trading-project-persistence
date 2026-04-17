using Cortex.Mediator.Queries;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.Queries;

public class GetTotalPnlQuery : IQuery<double>;

public class GetTotalPnlQueryHandler(IDatabaseService databaseService)
    : IQueryHandler<GetTotalPnlQuery, double>
{
    public Task<double> Handle(GetTotalPnlQuery query, CancellationToken cancellationToken)
        => Task.FromResult(databaseService.GetTotalPnl());
}
