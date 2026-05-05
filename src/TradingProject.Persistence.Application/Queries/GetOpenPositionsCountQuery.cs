using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.Queries;

public class GetOpenPositionsCountQuery : IQuery<int>;

public class GetOpenPositionsCountQueryHandler(
    ITradingDbContext context)
    : IQueryHandler<GetOpenPositionsCountQuery, int>
{
    public async Task<int> Handle(GetOpenPositionsCountQuery query, CancellationToken cancellationToken)
        => await context.Trades.CountAsync(t => t.Status == "open", cancellationToken);
}
