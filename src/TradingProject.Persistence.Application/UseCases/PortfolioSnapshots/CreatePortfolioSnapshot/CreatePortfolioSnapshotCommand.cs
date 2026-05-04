using AutoMapper;
using Cortex.Mediator.Commands;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Application.UseCases.PortfolioSnapshots.CreatePortfolioSnapshot;

public record CreatePortfolioSnapshotRequest(
    double Total, double Free, int PositionsCount,
    double DailyPnl = 0, double TotalPnl = 0);

public record CreatePortfolioSnapshotCommand(CreatePortfolioSnapshotRequest Snapshot) : ICommand<PortfolioSnapshotResponse>;

public class CreatePortfolioSnapshotCommandHandler(ITradingDbContext context, IMapper mapper)
    : ICommandHandler<CreatePortfolioSnapshotCommand, PortfolioSnapshotResponse>
{
    public async Task<PortfolioSnapshotResponse> Handle(CreatePortfolioSnapshotCommand command, CancellationToken ct)
    {
        var entity = mapper.Map<PortfolioSnapshot>(command.Snapshot);
        
        context.PortfolioSnapshots.Add(entity);
        await context.SaveChangesAsync(ct);

        return mapper.Map<PortfolioSnapshotResponse>(entity);
    }
}
