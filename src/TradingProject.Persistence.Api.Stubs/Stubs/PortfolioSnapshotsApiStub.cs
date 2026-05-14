using TradingProject.Persistence.Api.Stubs.Models;
using TradingProject.Persistence.Api.Stubs.V1;

namespace TradingProject.Persistence.Api.Stubs.Stubs;

/// <summary>
/// In-memory stub implementation of <see cref="IPortfolioSnapshotsApi"/> for testing and development.
/// </summary>
public class PortfolioSnapshotsApiStub : IPortfolioSnapshotsApi
{
    private readonly List<PortfolioSnapshotResponse> _snapshots = new();
    private int _nextId = 1;

    public Task<PagingList<PortfolioSnapshotResponse>> GetPortfolioSnapshotsAsync(
        int limit = 50,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var ordered = _snapshots.OrderByDescending(s => s.CreatedAt).ToArray();
        var paged = ordered.Skip((page - 1) * limit).Take(limit).ToArray();

        return Task.FromResult(new PagingList<PortfolioSnapshotResponse>(
            paged,
            page,
            limit,
            ordered.Length));
    }

    public Task<PortfolioSnapshotResponse> CreatePortfolioSnapshotAsync(
        CreatePortfolioSnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        var snapshot = new PortfolioSnapshotResponse(
            Id: _nextId++,
            Total: request.Total,
            Free: request.Free,
            PositionsCount: request.PositionsCount,
            DailyPnl: request.DailyPnl,
            TotalPnl: request.TotalPnl,
            CreatedAt: DateTime.UtcNow);

        _snapshots.Add(snapshot);
        return Task.FromResult(snapshot);
    }
}
