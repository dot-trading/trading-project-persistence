using Cortex.Mediator.Queries;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Application.Common.Models;
using TradingProject.Persistence.Application.Common.Enums;

namespace TradingProject.Persistence.Application.Queries;

public class GetPnlSummaryQuery(PnlSummaryType? type = null, string? quoteAsset = null) : IQuery<PnlSummary>
{
    public PnlSummaryType? Type { get; } = type;
    public string? QuoteAsset { get; } = quoteAsset;
}

public class GetPnlSummaryQueryHandler(IDatabaseService databaseService)
    : IQueryHandler<GetPnlSummaryQuery, PnlSummary>
{
    public async Task<PnlSummary> Handle(GetPnlSummaryQuery query, CancellationToken cancellationToken)
        => await databaseService.GetPnlSummary(query.Type, query.QuoteAsset, cancellationToken);
}
