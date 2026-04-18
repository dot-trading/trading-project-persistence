using Cortex.Mediator;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Application.Commands;

public record LogTradeOpenCommand(OpenPosition Trade) : ICommand;
public record LogTradeCloseCommand(int TradeId, double ClosePrice, double PnlUsdt, double PnlPct, string Reason) : ICommand;
public record UpdateTakeProfitCommand(int TradeId, double TakeProfit) : ICommand;
public record LogOpportunityCommand(OpportunityData Opportunity) : ICommand;
public record LogPortfolioSnapshotCommand(PortfolioData Portfolio) : ICommand;

public class CommandHandlers(IDatabaseService db) : 
    ICommandHandler<LogTradeOpenCommand>,
    ICommandHandler<LogTradeCloseCommand>,
    ICommandHandler<UpdateTakeProfitCommand>,
    ICommandHandler<LogOpportunityCommand>,
    ICommandHandler<LogPortfolioSnapshotCommand>
{
    public Task HandleAsync(LogTradeOpenCommand command, CancellationToken ct)
    {
        db.LogTradeOpen(command.Trade);
        return Task.CompletedTask;
    }

    public Task HandleAsync(LogTradeCloseCommand command, CancellationToken ct)
    {
        db.LogTradeClose(command.TradeId, command.ClosePrice, command.PnlUsdt, command.PnlPct, command.Reason);
        return Task.CompletedTask;
    }

    public Task HandleAsync(UpdateTakeProfitCommand command, CancellationToken ct)
    {
        db.UpdateTakeProfit(command.TradeId, command.TakeProfit);
        return Task.CompletedTask;
    }

    public Task HandleAsync(LogOpportunityCommand command, CancellationToken ct)
    {
        db.LogOpportunity(command.Opportunity);
        return Task.CompletedTask;
    }

    public Task HandleAsync(LogPortfolioSnapshotCommand command, CancellationToken ct)
    {
        db.LogPortfolioSnapshot(command.Portfolio);
        return Task.CompletedTask;
    }
}
