using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TradingProject.Persistence.Api.Controllers.V1;
using TradingProject.Persistence.Application.UseCases.Trades;
using TradingProject.Persistence.Application.UseCases.Trades.CreateTrade;
using TradingProject.Persistence.Application.UseCases.Trades.UpdateTrade;

namespace TradingProject.Persistence.Api.Tests.V1;

public class TradesControllerTests
{
    [Fact]
    public async Task CreateTrade_ShouldForwardBinanceOrderId()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var controller = new TradesController(mediatorMock.Object);

        var request = new CreateTradeRequest(
            Symbol: "BTCUSDT",
            Side: "BUY",
            Price: 60000,
            Quantity: 0.1,
            Value: 6000,
            BinanceOrderId: "BINANCE_ORDER_123");

        var expectedResponse = new TradeResponse(
            Id: 1, Symbol: "BTCUSDT", QuoteAsset: "USDT", Side: "BUY", Status: "open",
            Price: 60000, Quantity: 0.1, Value: 6000,
            StopLoss: null, TakeProfit: null, AiScore: null,
            BinanceOrderId: "BINANCE_ORDER_123",
            ClosePrice: null, Pnl: null, PnlPct: null,
            CreatedAt: DateTime.UtcNow, CloseAt: null);

        mediatorMock
            .Setup(m => m.SendCommandAsync<CreateTradeCommand, TradeResponse>(
                It.Is<CreateTradeCommand>(c => c.Trade.BinanceOrderId == "BINANCE_ORDER_123"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.CreateTrade(request, CancellationToken.None);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should().BeOfType<TradeResponse>().Subject;
        response.BinanceOrderId.Should().Be("BINANCE_ORDER_123");
    }

    [Fact]
    public async Task CreateTrade_ShouldAllowNullBinanceOrderId()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var controller = new TradesController(mediatorMock.Object);

        var request = new CreateTradeRequest(
            Symbol: "ETHUSDT",
            Side: "SELL",
            Price: 3000,
            Quantity: 1,
            Value: 3000);

        var expectedResponse = new TradeResponse(
            Id: 2, Symbol: "ETHUSDT", QuoteAsset: "USDT", Side: "SELL", Status: "open",
            Price: 3000, Quantity: 1, Value: 3000,
            StopLoss: null, TakeProfit: null, AiScore: null,
            BinanceOrderId: null,
            ClosePrice: null, Pnl: null, PnlPct: null,
            CreatedAt: DateTime.UtcNow, CloseAt: null);

        mediatorMock
            .Setup(m => m.SendCommandAsync<CreateTradeCommand, TradeResponse>(
                It.Is<CreateTradeCommand>(c => c.Trade.BinanceOrderId == null),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.CreateTrade(request, CancellationToken.None);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should().BeOfType<TradeResponse>().Subject;
        response.BinanceOrderId.Should().BeNull();
    }

    [Fact]
    public async Task UpdateTrade_ShouldForwardBinanceOrderId()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var controller = new TradesController(mediatorMock.Object);

        var request = new UpdateTradeRequest(
            Status: "closed",
            ClosePrice: 65000,
            Pnl: 500,
            PnlPct: 8.33,
            BinanceOrderId: "BINANCE_UPDATED");

        var expectedResponse = new TradeResponse(
            Id: 1, Symbol: "BTCUSDT", QuoteAsset: "USDT", Side: "BUY", Status: "closed",
            Price: 60000, Quantity: 0.1, Value: 6000,
            StopLoss: null, TakeProfit: null, AiScore: null,
            BinanceOrderId: "BINANCE_UPDATED",
            ClosePrice: 65000, Pnl: 500, PnlPct: 8.33,
            CreatedAt: DateTime.UtcNow, CloseAt: DateTime.UtcNow);

        mediatorMock
            .Setup(m => m.SendCommandAsync<UpdateTradeCommand, TradeResponse?>(
                It.Is<UpdateTradeCommand>(c => c.Updates.BinanceOrderId == "BINANCE_UPDATED"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.UpdateTrade(1, request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<TradeResponse>().Subject;
        response.BinanceOrderId.Should().Be("BINANCE_UPDATED");
    }
}
