using TradingProject.Persistence.Api.Stubs.Models;

namespace TradingProject.Persistence.Api.Stubs.V1;

/// <summary>
/// Defines the V1 trades API contract.
/// </summary>
public interface ITradesApi
{
    Task<PagingList<TradeResponse>> GetTradesAsync(
        int limit = 50,
        int page = 1,
        string? status = null,
        string? symbol = null,
        CancellationToken cancellationToken = default);

    Task<TradeResponse> CreateTradeAsync(
        CreateTradeRequest request,
        CancellationToken cancellationToken = default);

    Task<TradeResponse?> UpdateTradeAsync(
        int id,
        UpdateTradeRequest request,
        CancellationToken cancellationToken = default);
}
