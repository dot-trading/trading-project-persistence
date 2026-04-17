using Cortex.Mediator;
using Cortex.Mediator.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TradingProject.Persistence.Api.Controllers;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Application.Queries;

namespace TradingProject.Persistence.Api.Tests;

public class TradingDataControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly TradingDataController _controller;

    public TradingDataControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new TradingDataController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetOpenPositionsCount_ShouldReturnOk_WithCount()
    {
        _mediatorMock
            .Setup(m => m.SendQueryAsync(It.IsAny<IQuery<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var result = await _controller.GetOpenPositionsCount(CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(5);
    }

    [Fact]
    public async Task GetDailyPnl_ShouldReturnOk_WithPnl()
    {
        _mediatorMock
            .Setup(m => m.SendQueryAsync(It.IsAny<IQuery<double>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(150.5);

        var result = await _controller.GetDailyPnl(CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(150.5);
    }

    [Fact]
    public async Task GetStats_ShouldReturnOk_WithStatsObject()
    {
        var stats = new Stats(10, 20, 30, 100, 1, 2, 3, 10, 8, 80.0);
        _mediatorMock
            .Setup(m => m.SendQueryAsync(It.IsAny<IQuery<Stats>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stats);

        var result = await _controller.GetStats(CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedStats = okResult.Value.Should().BeOfType<Stats>().Subject;
        returnedStats.PnlDay.Should().Be(10);
        returnedStats.Wins.Should().Be(8);
    }

    [Fact]
    public async Task GetPnlSummary_ShouldReturnOk_WithPnlSummary()
    {
        var summary = new PnlSummary(10.5, 50.2, 100.1, 500.5);
        _mediatorMock
            .Setup(m => m.SendQueryAsync(It.IsAny<IQuery<PnlSummary>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summary);

        var result = await _controller.GetPnlSummary(CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSummary = okResult.Value.Should().BeOfType<PnlSummary>().Subject;
        returnedSummary.Daily.Should().Be(10.5);
        returnedSummary.Total.Should().Be(500.5);
    }

    [Fact]
    public async Task GetOpenPositions_ShouldReturnOk_WithList()
    {
        var positions = new List<OpenPosition>
        {
            new("BTCUSDT", "BUY", 65000, 0.1, 6500, 60000, 70000, 95, DateTime.UtcNow)
        };
        _mediatorMock
            .Setup(m => m.SendQueryAsync(It.IsAny<IQuery<List<OpenPosition>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(positions);

        var result = await _controller.GetOpenPositions(CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPositions = okResult.Value.Should().BeOfType<List<OpenPosition>>().Subject;
        returnedPositions.Should().HaveCount(1);
        returnedPositions[0].Symbol.Should().Be("BTCUSDT");
    }

    [Fact]
    public async Task GetLastTrades_ShouldReturnOk_WithLimitList()
    {
        var trades = new List<ClosedTrade>
        {
            new("ETHUSDT", "SELL", 3500, 3400, 100, 2.8, 90, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow)
        };
        _mediatorMock
            .Setup(m => m.SendQueryAsync(It.IsAny<IQuery<List<ClosedTrade>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(trades);

        var result = await _controller.GetLastTrades(CancellationToken.None, 5);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTrades = okResult.Value.Should().BeOfType<List<ClosedTrade>>().Subject;
        returnedTrades.Should().HaveCount(1);
        returnedTrades[0].Pnl.Should().Be(100);
    }
}
