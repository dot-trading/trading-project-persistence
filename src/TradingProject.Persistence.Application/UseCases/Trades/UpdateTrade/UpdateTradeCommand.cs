using Cortex.Mediator.Commands;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.UseCases.Trades.UpdateTrade;

public record UpdateTradeRequest(
    string? Status = null, double? ClosePrice = null, double? Pnl = null, double? PnlPct = null,
    double? TakeProfit = null, double? StopLoss = null);

public record UpdateTradeCommand(int Id, UpdateTradeRequest Updates) : ICommand<TradeResponse?>;

public class UpdateTradeCommandHandler(ITradingDbContext context)
    : ICommandHandler<UpdateTradeCommand, TradeResponse?>
{
    public async Task<TradeResponse?> Handle(UpdateTradeCommand command, CancellationToken ct)
    {
        var trade = await context.Trades.FirstOrDefaultAsync(t => t.Id == command.Id, ct);
        if (trade is null) return null;

        var u = command.Updates;
        if (u.Status is not null) trade.Status = u.Status;
        if (u.ClosePrice is not null) trade.ClosePrice = u.ClosePrice;
        if (u.Pnl is not null) trade.Pnl = u.Pnl;
        if (u.PnlPct is not null) trade.PnlPct = u.PnlPct;
        if (u.TakeProfit is not null) trade.TakeProfit = u.TakeProfit;
        if (u.StopLoss is not null) trade.StopLoss = u.StopLoss;

        if (u.Status == "closed" && trade.CloseAt is null)
            trade.CloseAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return new TradeResponse(
            trade.Id, trade.Symbol, trade.Side, trade.Status,
            trade.Price, trade.Quantity, trade.Value,
            trade.StopLoss, trade.TakeProfit, trade.AiScore,
            trade.ClosePrice, trade.Pnl, trade.PnlPct,
            trade.CreatedAt, trade.CloseAt);
    }
}
