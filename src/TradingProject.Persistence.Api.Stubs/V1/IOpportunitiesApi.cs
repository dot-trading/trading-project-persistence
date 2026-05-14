using TradingProject.Persistence.Api.Stubs.Models;

namespace TradingProject.Persistence.Api.Stubs.V1;

/// <summary>
/// Defines the V1 opportunities API contract.
/// </summary>
public interface IOpportunitiesApi
{
    Task<PagingList<OpportunityResponse>> GetOpportunitiesAsync(
        int limit = 50,
        int page = 1,
        string? symbol = null,
        bool? isApproved = null,
        CancellationToken cancellationToken = default);

    Task<OpportunityResponse> CreateOpportunityAsync(
        CreateOpportunityRequest request,
        CancellationToken cancellationToken = default);
}
