using Cortex.Mediator.Commands;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Application.UseCases.Trades.CreateTrade;

public record CreateTradeRequest(
    string Symbol, string Side,
    double Price, double Quantity, double Value,
    double? StopLoss = null, double? TakeProfit = null, int? AiScore = null);

public record CreateTradeCommand(CreateTradeRequest Trade) : ICommand<TradeResponse>;

public class CreateTradeCommandHandler(ITradingDbContext context)
    : ICommandHandler<CreateTradeCommand, TradeResponse>
{
    public async Task<TradeResponse> Handle(CreateTradeCommand command, CancellationToken ct)
    {
        var req = command.Trade;
        var entity = new Trade
        {
            Symbol = req.Symbol,
            Side = req.Side,
            Status = "open",
            Price = req.Price,
            Quantity = req.Quantity,
            Value = req.Value,
            StopLoss = req.StopLoss,
            TakeProfit = req.TakeProfit,
            AiScore = req.AiScore,
            CreatedAt = DateTime.UtcNow
        };
        context.Trades.Add(entity);
        await context.SaveChangesAsync(ct);

        return new TradeResponse(
            entity.Id, entity.Symbol, entity.Side, entity.Status,
            entity.Price, entity.Quantity, entity.Value,
            entity.StopLoss, entity.TakeProfit, entity.AiScore,
            entity.ClosePrice, entity.Pnl, entity.PnlPct,
            entity.CreatedAt, entity.CloseAt);
    }
}
