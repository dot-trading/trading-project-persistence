using TradingProject.Persistence.Api.Stubs.Models;

namespace TradingProject.Persistence.Api.Stubs.V1;

/// <summary>
/// Defines the V1 portfolio snapshots API contract.
/// </summary>
public interface IPortfolioSnapshotsApi
{
    Task<PagingList<PortfolioSnapshotResponse>> GetPortfolioSnapshotsAsync(
        int limit = 50,
        int page = 1,
        CancellationToken cancellationToken = default);

    Task<PortfolioSnapshotResponse> CreatePortfolioSnapshotAsync(
        CreatePortfolioSnapshotRequest request,
        CancellationToken cancellationToken = default);
}
