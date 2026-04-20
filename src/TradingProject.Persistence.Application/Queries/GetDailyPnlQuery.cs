using Cortex.Mediator.Queries;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.Queries;

public class GetDailyPnlQuery : IQuery<double>;

public class GetDailyPnlQueryHandler(IDatabaseService databaseService)
    : IQueryHandler<GetDailyPnlQuery, double>
{
    public async Task<double> Handle(GetDailyPnlQuery query, CancellationToken cancellationToken)
        => await databaseService.GetDailyPnl(cancellationToken);
}
