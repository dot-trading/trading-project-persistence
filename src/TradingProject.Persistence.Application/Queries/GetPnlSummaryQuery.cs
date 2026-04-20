using Cortex.Mediator.Queries;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Application.Common.Models;

namespace TradingProject.Persistence.Application.Queries;

public class GetPnlSummaryQuery : IQuery<PnlSummary>;

public class GetPnlSummaryQueryHandler(IDatabaseService databaseService)
    : IQueryHandler<GetPnlSummaryQuery, PnlSummary>
{
    public Task<PnlSummary> Handle(GetPnlSummaryQuery query, CancellationToken cancellationToken)
        => Task.FromResult(databaseService.GetPnlSummary());
}
