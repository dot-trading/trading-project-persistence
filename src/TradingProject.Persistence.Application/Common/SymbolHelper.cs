namespace TradingProject.Persistence.Application.Common;

public static class SymbolHelper
{
    private static readonly string[] KnownQuoteAssets = ["USDC", "USDT", "BUSD", "BTC", "ETH", "BNB", "EUR", "USD"];

    public static string ExtractQuoteAsset(string symbol)
    {
        foreach (var asset in KnownQuoteAssets)
            if (symbol.EndsWith(asset, StringComparison.OrdinalIgnoreCase))
                return asset.ToUpperInvariant();
        return string.Empty;
    }
}
