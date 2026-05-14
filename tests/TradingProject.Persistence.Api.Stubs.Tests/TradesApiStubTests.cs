using FluentAssertions;
using TradingProject.Persistence.Api.Stubs.Models;
using TradingProject.Persistence.Api.Stubs.Stubs;

namespace TradingProject.Persistence.Api.Stubs.Tests;

public class TradesApiStubTests
{
    [Fact]
    public async Task CreateTrade_ShouldReturnTradeWithBinanceOrderId()
    {
        // Arrange
        var stub = new TradesApiStub();
        var request = new CreateTradeRequest(
            Symbol: "BTCUSDT",
            Side: "BUY",
            Price: 60000,
            Quantity: 0.1,
            Value: 6000,
            BinanceOrderId: "BINANCE_ORDER_12345");

        // Act
        var result = await stub.CreateTradeAsync(request);

        // Assert
        result.BinanceOrderId.Should().Be("BINANCE_ORDER_12345");
        result.Symbol.Should().Be("BTCUSDT");
        result.Status.Should().Be("open");
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task CreateTrade_ShouldAllowNullBinanceOrderId()
    {
        // Arrange
        var stub = new TradesApiStub();
        var request = new CreateTradeRequest(
            Symbol: "ETHUSDT",
            Side: "SELL",
            Price: 3000,
            Quantity: 1,
            Value: 3000);

        // Act
        var result = await stub.CreateTradeAsync(request);

        // Assert
        result.BinanceOrderId.Should().BeNull();
    }

    [Fact]
    public async Task GetTrades_ShouldReturnPaginatedResults()
    {
        // Arrange
        var stub = new TradesApiStub();
        for (int i = 0; i < 10; i++)
        {
            await stub.CreateTradeAsync(new CreateTradeRequest(
                Symbol: "BTCUSDT",
                Side: "BUY",
                Price: 60000 + i,
                Quantity: 0.1,
                Value: 6000));
        }

        // Act
        var result = await stub.GetTradesAsync(limit: 3, page: 2);

        // Assert
        result.Payload.Should().HaveCount(3);
        result.TotalCount.Should().Be(10);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(3);
    }

    [Fact]
    public async Task GetTrades_ShouldFilterByStatus()
    {
        // Arrange
        var stub = new TradesApiStub();
        await stub.CreateTradeAsync(new CreateTradeRequest("BTCUSDT", "BUY", 60000, 0.1, 6000));
        await stub.CreateTradeAsync(new CreateTradeRequest("ETHUSDT", "SELL", 3000, 1, 3000));
        var trade2 = await stub.CreateTradeAsync(new CreateTradeRequest("SOLUSDT", "BUY", 150, 10, 1500));

        await stub.UpdateTradeAsync(trade2.Id, new UpdateTradeRequest(Status: "closed", Pnl: 100));

        // Act
        var openTrades = await stub.GetTradesAsync(status: "open");
        var closedTrades = await stub.GetTradesAsync(status: "closed");

        // Assert
        openTrades.Payload.Should().HaveCount(2);
        closedTrades.Payload.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateTrade_ShouldSetBinanceOrderId()
    {
        // Arrange
        var stub = new TradesApiStub();
        var trade = await stub.CreateTradeAsync(new CreateTradeRequest("BTCUSDT", "BUY", 60000, 0.1, 6000));

        // Act
        var updated = await stub.UpdateTradeAsync(trade.Id, new UpdateTradeRequest(BinanceOrderId: "BINANCE_UPDATED"));

        // Assert
        updated.Should().NotBeNull();
        updated!.BinanceOrderId.Should().Be("BINANCE_UPDATED");
    }

    [Fact]
    public async Task UpdateTrade_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var stub = new TradesApiStub();

        // Act
        var result = await stub.UpdateTradeAsync(999, new UpdateTradeRequest(Status: "closed"));

        // Assert
        result.Should().BeNull();
    }
}
