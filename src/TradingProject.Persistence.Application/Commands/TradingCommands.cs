using Cortex.Mediator.Commands;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Application.Common.Models;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Application.Commands;

public record LogTradeOpenCommand(OpenPosition Trade) : ICommand;
public record LogTradeCloseCommand(int TradeId, double ClosePrice, double PnlUsdt, double PnlPct, string Reason) : ICommand;
public record UpdateTakeProfitCommand(int TradeId, double TakeProfit) : ICommand;
public record LogOpportunityCommand(OpportunityData Opportunity) : ICommand;
public record LogPortfolioSnapshotCommand(PortfolioData Portfolio) : ICommand;

public class CommandHandlers(IDatabaseService db, ITradingDbContext context) : 
    ICommandHandler<LogTradeOpenCommand>,
    ICommandHandler<LogTradeCloseCommand>,
    ICommandHandler<UpdateTakeProfitCommand>,
    ICommandHandler<LogOpportunityCommand>,
    ICommandHandler<LogPortfolioSnapshotCommand>
{
    public async Task Handle(LogTradeOpenCommand command, CancellationToken ct)
    {
        await db.LogTradeOpen(command.Trade, ct);
    }

    public async Task Handle(LogTradeCloseCommand command, CancellationToken ct)
    {
        await db.LogTradeClose(command.TradeId, command.ClosePrice, command.PnlUsdt, command.PnlPct, command.Reason, ct);
    }

    public async Task Handle(UpdateTakeProfitCommand command, CancellationToken ct)
    {
        await db.UpdateTakeProfit(command.TradeId, command.TakeProfit, ct);
    }

    public async Task Handle(LogOpportunityCommand command, CancellationToken ct)
    {
        await db.LogOpportunity(command.Opportunity, ct);
    }

    public async Task Handle(LogPortfolioSnapshotCommand command, CancellationToken ct)
    {
        var entity = new PortfolioSnapshot
        {
            Free = command.Portfolio.FreeUsdt,
            Total = command.Portfolio.TotalUsdt,
            PositionsCount = command.Portfolio.OpenPositions?.Count ?? 0,
            CreatedAt = DateTime.UtcNow
        };

        context.PortfolioSnapshots.Add(entity);
        await context.SaveChangesAsync(ct);
    }
}
