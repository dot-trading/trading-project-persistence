using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.UseCases.Trades.GetTrades;

public record GetTradesQuery(int Limit = 50, string? Status = null, string? Symbol = null)
    : IQuery<List<TradeResponse>>;

public class GetTradesQueryHandler(ITradingDbContext context)
    : IQueryHandler<GetTradesQuery, List<TradeResponse>>
{
    public async Task<List<TradeResponse>> Handle(GetTradesQuery query, CancellationToken ct)
    {
        var q = context.Trades.AsQueryable();
        if (query.Status is not null) q = q.Where(t => t.Status == query.Status);
        if (query.Symbol is not null) q = q.Where(t => t.Symbol == query.Symbol);

        return await q
            .OrderByDescending(t => t.CreatedAt)
            .Take(query.Limit)
            .Select(t => new TradeResponse(
                t.Id, t.Symbol, t.Side, t.Status,
                t.Price, t.Quantity, t.Value,
                t.StopLoss, t.TakeProfit, t.AiScore,
                t.ClosePrice, t.Pnl, t.PnlPct,
                t.CreatedAt, t.CloseAt))
            .ToListAsync(ct);
    }
}
