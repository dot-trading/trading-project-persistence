using System.Text.Json;
using Cortex.Mediator;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TradingProject.Persistence.Api.Controllers.V0;
using TradingProject.Persistence.Application.Commands;
using TradingProject.Persistence.Application.Common.Models;

namespace TradingProject.Persistence.Api.Tests.V0;

#pragma warning disable CS0618
public class V0BackwardCompatibilityTests
{
    private static readonly JsonSerializerOptions AspNetOptions = new() { PropertyNameCaseInsensitive = true };

    // ── JSON contract tests ──────────────────────────────────────────────────

    [Fact]
    public void LogTradeCloseRequest_ShouldDeserialize_WithPnlUsdtKey()
    {
        var json = """{"tradeId":1,"closePrice":100.0,"pnlUsdt":50.0,"pnlPct":5.0,"reason":"TP hit"}""";
        var req = JsonSerializer.Deserialize<LogTradeCloseRequest>(json, AspNetOptions);
        req!.PnlUsdt.Should().Be(50.0);
    }

    [Fact]
    public void OpenPosition_ShouldSerialize_ValueAs_UsdtValueKey()
    {
        var position = new OpenPosition(1, "BTCUSDT", "BUY", 60000, 0.1, 6000, null, null, null, DateTime.UtcNow);
        var json = JsonSerializer.Serialize(position);
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.TryGetProperty("usdtValue", out var prop).Should().BeTrue("V0 contract requires usdtValue key");
        prop.GetDouble().Should().Be(6000);
        doc.RootElement.TryGetProperty("value", out _).Should().BeFalse("V0 must not expose renamed value key");
    }

    [Fact]
    public void OpenPosition_ShouldDeserialize_FromUsdtValueKey()
    {
        var json = """{"id":1,"symbol":"BTCUSDT","side":"BUY","entry":60000,"quantity":0.1,"usdtValue":6000,"stopLoss":null,"takeProfit":null,"aiScore":null,"createdAt":"2026-01-01T00:00:00Z"}""";
        var position = JsonSerializer.Deserialize<OpenPosition>(json, AspNetOptions);
        position!.Value.Should().Be(6000);
    }

    [Fact]
    public void PortfolioData_ShouldDeserialize_WithFreeUsdtAndTotalUsdtKeys()
    {
        var json = """{"freeUsdt":5000.0,"totalUsdt":10000.0,"openPositions":[]}""";
        var data = JsonSerializer.Deserialize<PortfolioData>(json, AspNetOptions);
        data!.Free.Should().Be(5000.0);
        data.Total.Should().Be(10000.0);
    }

    [Fact]
    public void PortfolioPositionData_ShouldDeserialize_WithUsdtValueKey()
    {
        var json = """{"symbol":"BTCUSDT","usdtValue":6000.0}""";
        var data = JsonSerializer.Deserialize<PortfolioPositionData>(json, AspNetOptions);
        data!.Value.Should().Be(6000.0);
    }

    // ── Controller behaviour tests ───────────────────────────────────────────

    [Fact]
    public async Task V0_LogTradeClose_ShouldForwardPnlUsdtValueToCommand()
    {
        var mediatorMock = new Mock<IMediator>();
        var controller = new TradingDataController(mediatorMock.Object);
        var req = new LogTradeCloseRequest(TradeId: 42, ClosePrice: 100.0, PnlUsdt: 50.0, PnlPct: 5.0, Reason: "TP hit");

        var result = await controller.LogTradeClose(req, CancellationToken.None);

        result.Should().BeOfType<OkResult>();
        mediatorMock.Verify(m => m.SendCommandAsync(
            It.Is<LogTradeCloseCommand>(c => c.TradeId == 42 && c.Pnl == 50.0 && c.PnlPct == 5.0),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task V0_LogPortfolioSnapshot_ShouldForwardFreeAndTotalToCommand()
    {
        var mediatorMock = new Mock<IMediator>();
        var controller = new TradingDataController(mediatorMock.Object);
        var portfolio = new PortfolioData { Free = 5000, Total = 10000 };

        var result = await controller.LogPortfolioSnapshot(portfolio, CancellationToken.None);

        result.Should().BeOfType<OkResult>();
        mediatorMock.Verify(m => m.SendCommandAsync(
            It.Is<LogPortfolioSnapshotCommand>(c => c.Portfolio.Free == 5000 && c.Portfolio.Total == 10000),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
