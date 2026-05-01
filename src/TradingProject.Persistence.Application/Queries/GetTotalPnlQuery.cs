using Cortex.Mediator.Queries;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.Queries;

public record GetTotalPnlQuery(string? QuoteAsset = null) : IQuery<double>;

public class GetTotalPnlQueryHandler(IDatabaseService databaseService)
    : IQueryHandler<GetTotalPnlQuery, double>
{
    public async Task<double> Handle(GetTotalPnlQuery query, CancellationToken cancellationToken)
        => await databaseService.GetTotalPnl(query.QuoteAsset, cancellationToken);
}
