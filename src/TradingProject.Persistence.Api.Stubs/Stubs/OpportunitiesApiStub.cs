using TradingProject.Persistence.Api.Stubs.Models;
using TradingProject.Persistence.Api.Stubs.V1;

namespace TradingProject.Persistence.Api.Stubs.Stubs;

/// <summary>
/// In-memory stub implementation of <see cref="IOpportunitiesApi"/> for testing and development.
/// </summary>
public class OpportunitiesApiStub : IOpportunitiesApi
{
    private readonly List<OpportunityResponse> _opportunities = new();
    private int _nextId = 1;

    public Task<PagingList<OpportunityResponse>> GetOpportunitiesAsync(
        int limit = 50,
        int page = 1,
        string? symbol = null,
        bool? isApproved = null,
        CancellationToken cancellationToken = default)
    {
        var query = _opportunities.AsEnumerable();

        if (symbol is not null)
            query = query.Where(o => o.Symbol == symbol);
        if (isApproved is not null)
            query = query.Where(o => o.IsApproved == isApproved.Value);

        var ordered = query.OrderByDescending(o => o.CreatedAt).ToArray();
        var paged = ordered.Skip((page - 1) * limit).Take(limit).ToArray();

        return Task.FromResult(new PagingList<OpportunityResponse>(
            paged,
            page,
            limit,
            ordered.Length));
    }

    public Task<OpportunityResponse> CreateOpportunityAsync(
        CreateOpportunityRequest request,
        CancellationToken cancellationToken = default)
    {
        var opportunity = new OpportunityResponse(
            Id: _nextId++,
            Symbol: request.Symbol,
            Score: request.Score,
            Signal: request.Signal,
            Reason: request.Reason,
            TargetPct: request.TargetPct,
            StopLossPct: request.StopLossPct,
            Price: request.Price,
            Acted: false,
            IsApproved: request.IsApproved,
            ValidationReason: request.ValidationReason,
            CreatedAt: DateTime.UtcNow);

        _opportunities.Add(opportunity);
        return Task.FromResult(opportunity);
    }
}
