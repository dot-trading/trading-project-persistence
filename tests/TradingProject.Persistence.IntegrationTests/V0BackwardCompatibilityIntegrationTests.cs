using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace TradingProject.Persistence.IntegrationTests;

public class V0BackwardCompatibilityIntegrationTests(TradingPersistenceApiFactory factory)
    : IClassFixture<TradingPersistenceApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    // ── POST endpoints accept old JSON key names ─────────────────────────────

    [Fact]
    public async Task V0_LogTradeOpen_WithUsdtValueKey_ShouldReturn200()
    {
        var body = new
        {
            id = 0,
            symbol = "BTCUSDT",
            side = "BUY",
            entry = 60000.0,
            quantity = 0.1,
            usdtValue = 6000.0,
            stopLoss = (double?)null,
            takeProfit = (double?)null,
            aiScore = (int?)null,
            createdAt = DateTime.UtcNow
        };

        var response = await _client.PostAsJsonAsync("/api/tradingdata/trades/open", body);

        response.StatusCode.Should().Be(HttpStatusCode.OK, "V0 endpoint must accept usdtValue key");
    }

    [Fact]
    public async Task V0_LogTradeClose_WithPnlUsdtKey_ShouldReturn200()
    {
        var body = new
        {
            tradeId = 999,
            closePrice = 65000.0,
            pnlUsdt = 500.0,
            pnlPct = 8.3,
            reason = "TP hit"
        };

        var response = await _client.PostAsJsonAsync("/api/tradingdata/trades/close", body);

        response.StatusCode.Should().Be(HttpStatusCode.OK, "V0 endpoint must accept pnlUsdt key");
    }

    [Fact]
    public async Task V0_LogPortfolioSnapshot_WithFreeUsdtAndTotalUsdtKeys_ShouldReturn200()
    {
        var body = new
        {
            freeUsdt = 5000.0,
            totalUsdt = 10000.0,
            openPositions = new[]
            {
                new { symbol = "BTCUSDT", usdtValue = 6000.0 }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/tradingdata/portfolio/snapshot", body);

        response.StatusCode.Should().Be(HttpStatusCode.OK, "V0 endpoint must accept freeUsdt/totalUsdt/usdtValue keys");
    }

    [Fact]
    public async Task V0_LogOpportunity_ShouldReturn200()
    {
        var body = new
        {
            symbol = "BTCUSDT",
            score = 80,
            signal = "BUY",
            reason = "Strong momentum",
            price = 60000.0,
            isApproved = true,
            validationReason = (string?)null
        };

        var response = await _client.PostAsJsonAsync("/api/tradingdata/opportunities", body);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── GET endpoints return old JSON key names ──────────────────────────────

    [Fact]
    public async Task V0_GetOpenPositions_ResponseShouldContain_UsdtValueKey()
    {
        // Insert a trade using the V0 contract
        await _client.PostAsJsonAsync("/api/tradingdata/trades/open", new
        {
            id = 0,
            symbol = "V0COMPAT",
            side = "BUY",
            entry = 100.0,
            quantity = 1.0,
            usdtValue = 100.0,
            stopLoss = (double?)null,
            takeProfit = (double?)null,
            aiScore = (int?)null,
            createdAt = DateTime.UtcNow
        });

        var response = await _client.GetAsync("/api/tradingdata/positions/open");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var position = doc.RootElement.EnumerateArray()
            .FirstOrDefault(p => p.GetProperty("symbol").GetString() == "V0COMPAT");
        position.ValueKind.Should().NotBe(JsonValueKind.Undefined, "inserted trade must appear in open positions");
        position.TryGetProperty("usdtValue", out _).Should().BeTrue("V0 response must use usdtValue key");
        position.TryGetProperty("value", out _).Should().BeFalse("V0 response must not expose renamed value key");
    }

    // ── Route compatibility ──────────────────────────────────────────────────

    [Fact]
    public async Task V0_UnversionedRoute_Stats_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/tradingdata/stats");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task V0_UnversionedRoute_OpenPositionsCount_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/tradingdata/positions/open/count");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task V0_VersionedRoute_ShouldAlsoWork()
    {
        var response = await _client.GetAsync("/api/v0/tradingdata/stats");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
