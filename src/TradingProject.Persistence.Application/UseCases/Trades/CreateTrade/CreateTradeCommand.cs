using AutoMapper;
using Cortex.Mediator.Commands;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Application.UseCases.Trades.CreateTrade;

public record CreateTradeRequest(
    string Symbol, string Side,
    double Price, double Quantity, double Value,
    double? StopLoss = null, double? TakeProfit = null, int? AiScore = null);

public record CreateTradeCommand(CreateTradeRequest Trade) : ICommand<TradeResponse>;

public class CreateTradeCommandHandler(ITradingDbContext context, IMapper mapper)
    : ICommandHandler<CreateTradeCommand, TradeResponse>
{
    public async Task<TradeResponse> Handle(CreateTradeCommand command, CancellationToken ct)
    {
        var entity = mapper.Map<Trade>(command.Trade);
        
        context.Trades.Add(entity);
        await context.SaveChangesAsync(ct);

        return mapper.Map<TradeResponse>(entity);
    }
}
