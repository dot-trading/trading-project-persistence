using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.UseCases.Trades.GetTrades;

public record GetTradesQuery(int Limit = 50, int Page = 1, string? Status = null, string? Symbol = null)
    : IQuery<List<TradeResponse>>;

public class GetTradesQueryHandler(ITradingDbContext context, IMapper mapper)
    : IQueryHandler<GetTradesQuery, List<TradeResponse>>
{
    public async Task<List<TradeResponse>> Handle(GetTradesQuery query, CancellationToken ct)
    {
        var q = context.Trades.AsQueryable();
        if (query.Status is not null) q = q.Where(t => t.Status == query.Status);
        if (query.Symbol is not null) q = q.Where(t => t.Symbol == query.Symbol);

        return await q
            .OrderByDescending(t => t.CreatedAt)
            .Skip((query.Page - 1) * query.Limit)
            .Take(query.Limit)
            .ProjectTo<TradeResponse>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}
