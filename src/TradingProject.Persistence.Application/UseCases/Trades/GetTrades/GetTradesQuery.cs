using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Application.Common.Models;

namespace TradingProject.Persistence.Application.UseCases.Trades.GetTrades;

public record GetTradesQuery(int Limit = 50, int Page = 1, string? Status = null, string? Symbol = null)
    : IQuery<PagingList<TradeResponse>>;

public class GetTradesQueryHandler(ITradingDbContext context, IMapper mapper)
    : IQueryHandler<GetTradesQuery, PagingList<TradeResponse>>
{
    public async Task<PagingList<TradeResponse>> Handle(GetTradesQuery query, CancellationToken cancellationToken)
    {
        var q = context.Trades.AsQueryable();
        if (query.Status is not null) q = q.Where(t => t.Status == query.Status);
        if (query.Symbol is not null) q = q.Where(t => t.Symbol == query.Symbol);

        var data =
            await q.OrderByDescending(t => t.CreatedAt)
                   .Skip((query.Page - 1) * query.Limit)
                   .Take(query.Limit)
                   .ProjectTo<TradeResponse>(mapper.ConfigurationProvider)
                   .ToArrayAsync(cancellationToken);

        return new PagingList<TradeResponse>(
            data,
            query.Page,
            query.Limit,
            await q.CountAsync(cancellationToken),
            new Dictionary<string, object?>()
            {
                ["Status"] = query.Status,
                ["Symbol"] = query.Symbol,
            });
    }
}
