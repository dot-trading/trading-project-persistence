using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Application.Common.Enums;
using TradingProject.Persistence.Application.Common.Models;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Infrastructure.Services;

public class DatabaseService(IServiceScopeFactory scopeFactory, ILogger<DatabaseService> logger) : IDatabaseService
{
    static DatabaseService()
    {
        // Just to be sure the class is loaded
    }

    public async Task<PnlSummaryItem> GetPnlSummaryAsync(
        PnlSummaryType pnlSummaryType,
        string? quoteAsset = null,
        CancellationToken cancellationToken = default)
    {
        var summary = await GetPnlSummary(pnlSummaryType, quoteAsset, cancellationToken);
        return pnlSummaryType switch
        {
            PnlSummaryType.Today => summary.Today!,
            PnlSummaryType.Yesterday => summary.Yesterday!,
            PnlSummaryType.ThisWeek => summary.ThisWeek!,
            PnlSummaryType.ThisMonth => summary.ThisMonth!,
            PnlSummaryType.ThisYear => summary.ThisYear!,
            PnlSummaryType.All => summary.Total!,
            _ => throw new ArgumentOutOfRangeException(nameof(pnlSummaryType))
        };
    }

    public async Task<PnlSummary> GetPnlSummary(PnlSummaryType? type = null, string? quoteAsset = null, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITradingDbContext>();
        
        var query = context.Trades.AsNoTracking().Where(t => t.Status == "closed");
        if (!string.IsNullOrEmpty(quoteAsset))
            query = query.Where(t => t.Symbol.EndsWith(quoteAsset));

        var trades = await query.ToListAsync(ct);
        
        var today = DateTime.Today.ToUniversalTime();
        var yesterdayStart = DateTime.Today.AddDays(-1).ToUniversalTime();
        var weekStart = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7).ToUniversalTime();
        var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToUniversalTime();
        var yearStart = new DateTime(DateTime.Today.Year, 1, 1).ToUniversalTime();

        var todaySum = trades.Where(t => t.CloseAt >= today).Sum(t => t.Pnl ?? 0);
        var yesterdaySum = trades.Where(t => t.CloseAt >= yesterdayStart && t.CloseAt < today).Sum(t => t.Pnl ?? 0);
        var weekSum = trades.Where(t => t.CloseAt >= weekStart).Sum(t => t.Pnl ?? 0);
        var monthSum = trades.Where(t => t.CloseAt >= monthStart).Sum(t => t.Pnl ?? 0);
        var yearSum = trades.Where(t => t.CloseAt >= yearStart).Sum(t => t.Pnl ?? 0);
        var totalSum = trades.Sum(t => t.Pnl ?? 0);

        return new PnlSummary(
            new PnlSummaryItem((decimal)todaySum, PnlSummaryType.Today),
            new PnlSummaryItem((decimal)yesterdaySum, PnlSummaryType.Yesterday),
            new PnlSummaryItem((decimal)weekSum, PnlSummaryType.ThisWeek),
            new PnlSummaryItem((decimal)monthSum, PnlSummaryType.ThisMonth),
            new PnlSummaryItem((decimal)yearSum, PnlSummaryType.ThisYear),
            new PnlSummaryItem((decimal)totalSum, PnlSummaryType.All)
        );
    }

    public async Task<int> GetOpenPositionsCount(CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITradingDbContext>();
        return await context.Trades.CountAsync(t => t.Status == "open", ct);
    }

    public async Task<double> GetDailyPnl(string? quoteAsset = null, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITradingDbContext>();
        var today = DateTime.Today.ToUniversalTime();
        var query = context.Trades.Where(t => t.Status == "closed" && t.CloseAt >= today);
        if (!string.IsNullOrEmpty(quoteAsset))
            query = query.Where(t => t.Symbol.EndsWith(quoteAsset));
            
        return await query.SumAsync(t => t.Pnl ?? 0, ct);
    }

    public async Task<double> GetTotalPnl(string? quoteAsset = null, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITradingDbContext>();
        var query = context.Trades.Where(t => t.Status == "closed");
        if (!string.IsNullOrEmpty(quoteAsset))
            query = query.Where(t => t.Symbol.EndsWith(quoteAsset));

        return await query.SumAsync(t => t.Pnl ?? 0, ct);
    }

    public async Task<Stats> GetStats(string? quoteAsset = null, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITradingDbContext>();
        var today = DateTime.Today.ToUniversalTime();
        var monday = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7).ToUniversalTime();
        var month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToUniversalTime();

        var query = context.Trades.Where(t => t.Status == "closed");
        if (!string.IsNullOrEmpty(quoteAsset))
            query = query.Where(t => t.Symbol.EndsWith(quoteAsset));

        var trades = await query.ToListAsync(ct);

        var statsDay = trades.Where(t => t.CloseAt >= today).ToList();
        var statsWeek = trades.Where(t => t.CloseAt >= monday).ToList();
        var statsMonth = trades.Where(t => t.CloseAt >= month).ToList();
        var statsTotal = trades;

        var wins = trades.Count(t => t.Pnl > 0);

        return new Stats(
            statsDay.Sum(t => t.Pnl ?? 0),
            statsWeek.Sum(t => t.Pnl ?? 0),
            statsMonth.Sum(t => t.Pnl ?? 0),
            statsTotal.Sum(t => t.Pnl ?? 0),
            statsDay.Count,
            statsWeek.Count,
            statsMonth.Count,
            statsTotal.Count,
            wins,
            statsTotal.Count > 0 ? wins * 100.0 / statsTotal.Count : 0);
    }

    public async Task<List<OpenPosition>> GetOpenPositions(CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITradingDbContext>();
        var trades = await context.Trades
            .Where(t => t.Status == "open")
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

        return trades.Select(t => new OpenPosition(
            t.Id, t.Symbol, t.Side, t.Price, t.Quantity, t.Value,
            t.StopLoss, t.TakeProfit, t.AiScore, t.CreatedAt)).ToList();
    }

    public async Task<List<ClosedTrade>> GetLastTrades(int limit = 5, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITradingDbContext>();
        return await context.Trades
            .Where(t => t.Status == "closed")
            .OrderByDescending(t => t.CloseAt)
            .Take(limit)
            .Select(t => new ClosedTrade(
                t.Symbol, t.Side, t.Price, t.ClosePrice ?? 0,
                t.Pnl ?? 0, t.PnlPct ?? 0, t.AiScore, t.CreatedAt, t.CloseAt ?? DateTime.MinValue))
            .ToListAsync(ct);
    }

    public async Task LogTradeOpen(OpenPosition trade, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITradingDbContext>();
        var entity = new Trade
        {
            Symbol = trade.Symbol,
            Side = trade.Side,
            Status = "open",
            Price = trade.Entry,
            Quantity = trade.Quantity,
            Value = trade.UsdtValue,
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
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITradingDbContext>();
        var trade = await context.Trades.FirstOrDefaultAsync(t => t.Id == tradeId, ct);
        if (trade == null || trade.Status == "closed") return;

        trade.Status = "closed";
        trade.ClosePrice = closePrice;
        trade.Pnl = pnlUsdt;
        trade.PnlPct = pnlPct;
        trade.CloseAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateTakeProfit(int tradeId, double takeProfit, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITradingDbContext>();
        var trade = await context.Trades.FirstOrDefaultAsync(t => t.Id == tradeId, ct);
        if (trade == null) return;

        trade.TakeProfit = takeProfit;
        await context.SaveChangesAsync(ct);
    }

    public async Task LogOpportunity(OpportunityData opportunity, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITradingDbContext>();
        var entity = new Opportunity
        {
            Symbol = opportunity.Symbol,
            Score = opportunity.Score,
            Signal = opportunity.Signal ?? "NEUTRAL",
            Reason = opportunity.Reason,
            Price = opportunity.Price,
            IsApproved = opportunity.IsApproved,
            ValidationReason = opportunity.ValidationReason,
            CreatedAt = DateTime.UtcNow
        };

        context.Opportunities.Add(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task<List<OpportunityData>> GetRecentOpportunities(int hours, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITradingDbContext>();
        var cutoff = DateTime.UtcNow.AddHours(-hours);
        return await context.Opportunities
            .Where(o => o.CreatedAt >= cutoff)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OpportunityData
            {
                Symbol = o.Symbol,
                Score = o.Score,
                Signal = o.Signal,
                Reason = o.Reason,
                Price = o.Price,
                IsApproved = o.IsApproved,
                ValidationReason = o.ValidationReason
            })
            .ToListAsync(ct);
    }

    public async Task LogPortfolioSnapshot(PortfolioData portfolio, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ITradingDbContext>();
        var entity = new PortfolioSnapshot
        {
            Free = portfolio.FreeUsdt,
            Total = portfolio.TotalUsdt,
            PositionsCount = portfolio.OpenPositions?.Count ?? 0,
            CreatedAt = DateTime.UtcNow
        };

        context.PortfolioSnapshots.Add(entity);
        await context.SaveChangesAsync(ct);
    }
}
