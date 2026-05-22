using TradingProject.Persistence.Api.Stubs.Models;
using TradingProject.Persistence.Api.Stubs.V1;

namespace TradingProject.Persistence.Api.Stubs.Stubs;

/// <summary>
/// In-memory stub implementation of <see cref="ITradesApi"/> for testing and development.
/// </summary>
public class TradesApiStub : ITradesApi
{
    private readonly List<TradeResponse> _trades = new();
    private int _nextId = 1;

    public Task<PagingList<TradeResponse>> GetTradesAsync(
        int limit = 50,
        int page = 1,
        string? status = null,
        string? symbol = null,
        CancellationToken cancellationToken = default)
    {
        var query = _trades.AsEnumerable();

        if (status is not null)
            query = query.Where(t => t.Status == status);
        if (symbol is not null)
            query = query.Where(t => t.Symbol == symbol);

        var ordered = query.OrderByDescending(t => t.CreatedAt).ToArray();
        var paged = ordered.Skip((page - 1) * limit).Take(limit).ToArray();

        return Task.FromResult(new PagingList<TradeResponse>(
            paged,
            page,
            limit,
            ordered.Length));
    }

    public Task<TradeResponse> CreateTradeAsync(
        CreateTradeRequest request,
        CancellationToken cancellationToken = default)
    {
        var trade = new TradeResponse(
            Id: _nextId++,
            Symbol: request.Symbol,
            QuoteAsset: ExtractQuoteAsset(request.Symbol),
            Side: request.Side,
            Status: "open",
            Price: request.Price,
            Quantity: request.Quantity,
            Value: request.Value,
            StopLoss: request.StopLoss,
            TakeProfit: request.TakeProfit,
            AiScore: request.AiScore,
            BinanceOrderId: request.BinanceOrderId,
            ClosePrice: null,
            Pnl: null,
            PnlPct: null,
            CreatedAt: DateTime.UtcNow,
            CloseAt: null);

        _trades.Add(trade);
        return Task.FromResult(trade);
    }

    public Task<TradeResponse?> UpdateTradeAsync(
        int id,
        UpdateTradeRequest request,
        CancellationToken cancellationToken = default)
    {
        var index = _trades.FindIndex(t => t.Id == id);
        if (index < 0)
            return Task.FromResult<TradeResponse?>(null);

        var existing = _trades[index];

        var updated = existing with
        {
            Status = request.Status ?? existing.Status,
            ClosePrice = request.ClosePrice ?? existing.ClosePrice,
            Pnl = request.Pnl ?? existing.Pnl,
            PnlPct = request.PnlPct ?? existing.PnlPct,
            TakeProfit = request.TakeProfit ?? existing.TakeProfit,
            StopLoss = request.StopLoss ?? existing.StopLoss,
            BinanceOrderId = request.BinanceOrderId ?? existing.BinanceOrderId,
            CloseAt = request.Status == "closed" && existing.CloseAt is null
                ? DateTime.UtcNow
                : existing.CloseAt
        };

        _trades[index] = updated;
        return Task.FromResult<TradeResponse?>(updated);
    }

    private static string ExtractQuoteAsset(string symbol)
    {
        var knownQuoteAssets = new[] { "USDC", "USDT", "BUSD", "BTC", "ETH", "BNB", "EUR", "USD" };
        foreach (var asset in knownQuoteAssets)
            if (symbol.EndsWith(asset, System.StringComparison.OrdinalIgnoreCase))
                return asset.ToUpperInvariant();
        return string.Empty;
    }
}
