using AutoMapper;
using Cortex.Mediator.Commands;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.UseCases.Trades.UpdateTrade;

public record UpdateTradeRequest(
    string? Status = null, double? ClosePrice = null, double? Pnl = null, double? PnlPct = null,
    double? TakeProfit = null, double? StopLoss = null);

public record UpdateTradeCommand(int Id, UpdateTradeRequest Updates) : ICommand<TradeResponse?>;

public class UpdateTradeCommandHandler(ITradingDbContext context, IMapper mapper)
    : ICommandHandler<UpdateTradeCommand, TradeResponse?>
{
    public async Task<TradeResponse?> Handle(UpdateTradeCommand command, CancellationToken ct)
    {
        var trade = await context.Trades.FirstOrDefaultAsync(t => t.Id == command.Id, ct);
        if (trade is null) return null;

        mapper.Map(command.Updates, trade);

        if (command.Updates.Status == "closed" && trade.CloseAt is null)
            trade.CloseAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return mapper.Map<TradeResponse>(trade);
    }
}
