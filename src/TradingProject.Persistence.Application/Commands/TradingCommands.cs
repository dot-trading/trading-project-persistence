using Cortex.Mediator.Commands;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Application.Common.Models;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Application.Commands;

public record LogTradeOpenCommand(OpenPosition Trade) : ICommand<int>;
public record LogTradeCloseCommand(int TradeId, double ClosePrice, double Pnl, double PnlPct, string Reason) : ICommand;
public record UpdateTakeProfitCommand(int TradeId, double TakeProfit) : ICommand;
public record LogOpportunityCommand(OpportunityData Opportunity) : ICommand;
public record LogPortfolioSnapshotCommand(PortfolioData Portfolio) : ICommand;

public class CommandHandlers(IDatabaseService db, ITradingDbContext context) : 
    ICommandHandler<LogTradeOpenCommand, int>,
    ICommandHandler<LogTradeCloseCommand>,
    ICommandHandler<UpdateTakeProfitCommand>,
    ICommandHandler<LogOpportunityCommand>,
    ICommandHandler<LogPortfolioSnapshotCommand>
{
    public async Task<int> Handle(LogTradeOpenCommand command, CancellationToken ct)
    {
        return await db.LogTradeOpen(command.Trade, ct);
    }

    public async Task Handle(LogTradeCloseCommand command, CancellationToken ct)
    {
        await db.LogTradeClose(command.TradeId, command.ClosePrice, command.Pnl, command.PnlPct, command.Reason, ct);
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
            Free = command.Portfolio.Free,
            Total = command.Portfolio.Total,
            DailyPnl = command.Portfolio.DailyPnl,
            TotalPnl = command.Portfolio.TotalPnl,
            PositionsCount = command.Portfolio.OpenPositions?.Count ?? 0,
            CreatedAt = DateTime.UtcNow
        };

        context.PortfolioSnapshots.Add(entity);
        await context.SaveChangesAsync(ct);
    }
}
