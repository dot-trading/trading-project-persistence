using Cortex.Mediator.Commands;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Application.UseCases.PortfolioSnapshots.CreatePortfolioSnapshot;

public record CreatePortfolioSnapshotRequest(
    double Total, double Free, int PositionsCount,
    double DailyPnl = 0, double TotalPnl = 0);

public record CreatePortfolioSnapshotCommand(CreatePortfolioSnapshotRequest Snapshot) : ICommand<PortfolioSnapshotResponse>;

public class CreatePortfolioSnapshotCommandHandler(ITradingDbContext context)
    : ICommandHandler<CreatePortfolioSnapshotCommand, PortfolioSnapshotResponse>
{
    public async Task<PortfolioSnapshotResponse> Handle(CreatePortfolioSnapshotCommand command, CancellationToken ct)
    {
        var req = command.Snapshot;
        var entity = new PortfolioSnapshot
        {
            Total = req.Total,
            Free = req.Free,
            PositionsCount = req.PositionsCount,
            DailyPnl = req.DailyPnl,
            TotalPnl = req.TotalPnl,
            CreatedAt = DateTime.UtcNow
        };
        context.PortfolioSnapshots.Add(entity);
        await context.SaveChangesAsync(ct);

        return new PortfolioSnapshotResponse(
            entity.Id, entity.Total, entity.Free,
            entity.PositionsCount, entity.DailyPnl, entity.TotalPnl, entity.CreatedAt);
    }
}
