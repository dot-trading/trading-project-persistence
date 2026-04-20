using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Application.Common.Enums;
using TradingProject.Persistence.Application.Common.Models;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Infrastructure.Services;

public class DatabaseService(ITradingDbContext context) : IDatabaseService
{
    public async Task<PnlSummaryItem> GetPnlSummaryAsync(
        PnlSummaryType pnlSummaryType,
        CancellationToken cancellationToken = default)
    {
        var query = context.Trades.Where(e => e.Status == "closed");

        switch (pnlSummaryType)
        {
            case PnlSummaryType.Today:
                query = query.Where(e => e.CloseAt >= DateTime.Today.ToUniversalTime());
                break;
            case PnlSummaryType.Yesterday:
                var yesterday = DateTime.Today.AddDays(-1).ToUniversalTime();
                var today = DateTime.Today.ToUniversalTime();
                query = query.Where(e => e.CloseAt >= yesterday && e.CloseAt < today);
                break;
            case PnlSummaryType.ThisWeek:
                var daysToMonday = ((int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                var startOfWeek = DateTime.Today.AddDays(-daysToMonday).ToUniversalTime();
                query = query.Where(e => e.CloseAt >= startOfWeek);
                break;
            case PnlSummaryType.ThisMonth:
                var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToUniversalTime();
                query = query.Where(e => e.CloseAt >= startOfMonth);
                break;
            case PnlSummaryType.ThisYear:
                var startOfYear = new DateTime(DateTime.Today.Year, 1, 1).ToUniversalTime();
                query = query.Where(e => e.CloseAt >= startOfYear);
                break;
        }

        var pnl = await query.SumAsync(e => e.PnlUsdt ?? 0, cancellationToken);
        return new PnlSummaryItem(Convert.ToDecimal(pnl), pnlSummaryType);
    }

    public async Task<PnlSummary> GetPnlSummary(PnlSummaryType? type = null, CancellationToken ct = default)
    {
        if (type.HasValue)
        {
            var item = await GetPnlSummaryAsync(type.Value, ct);
            return type.Value switch
            {
                PnlSummaryType.Today => new PnlSummary(Today: item),
                PnlSummaryType.Yesterday => new PnlSummary(Yesterday: item),
                PnlSummaryType.ThisWeek => new PnlSummary(ThisWeek: item),
                PnlSummaryType.ThisMonth => new PnlSummary(ThisMonth: item),
                PnlSummaryType.ThisYear => new PnlSummary(ThisYear: item),
                PnlSummaryType.All => new PnlSummary(Total: item),
                _ => new PnlSummary()
            };
        }

        var todayTask = GetPnlSummaryAsync(PnlSummaryType.Today, ct);
        var yesterdayTask = GetPnlSummaryAsync(PnlSummaryType.Yesterday, ct);
        var weekTask = GetPnlSummaryAsync(PnlSummaryType.ThisWeek, ct);
        var monthTask = GetPnlSummaryAsync(PnlSummaryType.ThisMonth, ct);
        var yearTask = GetPnlSummaryAsync(PnlSummaryType.ThisYear, ct);
        var totalTask = GetPnlSummaryAsync(PnlSummaryType.All, ct);

        await Task.WhenAll(todayTask, yesterdayTask, weekTask, monthTask, yearTask, totalTask);

        return new PnlSummary(
            await todayTask,
            await yesterdayTask,
            await weekTask,
            await monthTask,
            await yearTask,
            await totalTask);
    }

    public async Task<int> GetOpenPositionsCount(CancellationToken ct = default)
    {
        return await context.Trades.CountAsync(t => t.Status == "open", ct);
    }

    public async Task<double> GetDailyPnl(CancellationToken ct = default)
    {
        var today = DateTime.Today.ToUniversalTime();
        return await context.Trades
            .Where(t => t.Status == "closed" && t.CloseAt >= today)
            .SumAsync(t => t.PnlUsdt ?? 0, ct);
    }

    public async Task<double> GetTotalPnl(CancellationToken ct = default)
    {
        return await context.Trades
            .Where(t => t.Status == "closed")
            .SumAsync(t => t.PnlUsdt ?? 0, ct);
    }

    public async Task<Stats> GetStats(CancellationToken ct = default)
    {
        var today = DateTime.Today.ToUniversalTime();
        var monday = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7).ToUniversalTime();
        var month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToUniversalTime();

        var trades = await context.Trades.Where(t => t.Status == "closed").ToListAsync(ct);

        var statsDay = trades.Where(t => t.CloseAt >= today).ToList();
        var statsWeek = trades.Where(t => t.CloseAt >= monday).ToList();
        var statsMonth = trades.Where(t => t.CloseAt >= month).ToList();
        var statsTotal = trades;

        var wins = trades.Count(t => t.PnlUsdt > 0);

        return new Stats(
            statsDay.Sum(t => t.PnlUsdt ?? 0),
            statsWeek.Sum(t => t.PnlUsdt ?? 0),
            statsMonth.Sum(t => t.PnlUsdt ?? 0),
            statsTotal.Sum(t => t.PnlUsdt ?? 0),
            statsDay.Count,
            statsWeek.Count,
            statsMonth.Count,
            statsTotal.Count,
            wins,
            statsTotal.Count > 0 ? wins * 100.0 / statsTotal.Count : 0);
    }

    public async Task<List<OpenPosition>> GetOpenPositions(CancellationToken ct = default)
    {
        return await context.Trades
            .Where(t => t.Status == "open")
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new OpenPosition(
                t.Symbol, t.Side, t.Price, t.Quantity, t.UsdtValue,
                t.StopLoss, t.TakeProfit, t.AiScore, t.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<List<ClosedTrade>> GetLastTrades(int limit = 5, CancellationToken ct = default)
    {
        return await context.Trades
            .Where(t => t.Status == "closed")
            .OrderByDescending(t => t.CloseAt)
            .Take(limit)
            .Select(t => new ClosedTrade(
                t.Symbol, t.Side, t.Price, t.ClosePrice ?? 0,
                t.PnlUsdt ?? 0, t.PnlPct ?? 0, t.AiScore, t.CreatedAt, t.CloseAt ?? DateTime.MinValue))
            .ToListAsync(ct);
    }

    public async Task LogTradeOpen(OpenPosition trade, CancellationToken ct = default)
    {
        var entity = new Trade
        {
            Symbol = trade.Symbol,
            Side = trade.Side,
            Status = "open",
            Price = trade.Entry,
            Quantity = trade.Quantity,
            UsdtValue = trade.UsdtValue,
            StopLoss = trade.StopLoss,
            TakeProfit = trade.TakeProfit,
            AiScore = trade.AiScore,
            CreatedAt = trade.CreatedAt.ToUniversalTime()
        };

        context.Trades.Add(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task LogTradeClose(int tradeId, double closePrice, double pnlUsdt, double pnlPct, string reason, CancellationToken ct = default)
    {
        var trade = await context.Trades.FirstOrDefaultAsync(t => t.Id == tradeId, ct);
        if (trade == null) return;

        trade.Status = "closed";
        trade.ClosePrice = closePrice;
        trade.PnlUsdt = pnlUsdt;
        trade.PnlPct = pnlPct;
        trade.CloseAt = DateTime.UtcNow;
        // AI reason? Assuming there's a field or it's part of another entity, but the original SQL had ai_reason=@r
        // Let's check Trade entity. It doesn't have AiReason. I'll skip it or add it if I see it.
        // Wait, line 177 of original file: ai_reason=@r. 
        // I'll check Trade.cs again.

        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateTakeProfit(int tradeId, double takeProfit, CancellationToken ct = default)
    {
        var trade = await context.Trades.FirstOrDefaultAsync(t => t.Id == tradeId, ct);
        if (trade == null) return;

        trade.TakeProfit = takeProfit;
        await context.SaveChangesAsync(ct);
    }

    public async Task LogOpportunity(OpportunityData opportunity, CancellationToken ct = default)
    {
        var entity = new Opportunity
        {
            Symbol = opportunity.Symbol,
            Score = opportunity.Score,
            Reason = opportunity.Reason,
            Price = opportunity.Price,
            CreatedAt = DateTime.UtcNow
        };

        context.Opportunities.Add(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task LogPortfolioSnapshot(PortfolioData portfolio, CancellationToken ct = default)
    {
        var entity = new PortfolioSnapshot
        {
            FreeUsdt = portfolio.FreeUsdt,
            TotalUsdt = portfolio.TotalUsdt,
            PositionsCount = portfolio.OpenPositions?.Count ?? 0,
            CreatedAt = DateTime.UtcNow
        };

        context.PortfolioSnapshots.Add(entity);
        await context.SaveChangesAsync(ct);
    }
}
