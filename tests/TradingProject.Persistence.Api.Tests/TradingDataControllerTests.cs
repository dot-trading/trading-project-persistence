using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TradingProject.Persistence.Api.Controllers;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Api.Tests;

public class TradingDataControllerTests
{
    private readonly Mock<IDatabaseService> _databaseServiceMock;
    private readonly TradingDataController _controller;

    public TradingDataControllerTests()
    {
        _databaseServiceMock = new Mock<IDatabaseService>();
        _controller = new TradingDataController(_databaseServiceMock.Object);
    }

    [Fact]
    public void GetOpenPositionsCount_ShouldReturnOk_WithCount()
    {
        // Arrange
        _databaseServiceMock.Setup(service => service.GetOpenPositionsCount()).Returns(5);

        // Act
        var result = _controller.GetOpenPositionsCount();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(5);
    }

    [Fact]
    public void GetDailyPnl_ShouldReturnOk_WithPnl()
    {
        // Arrange
        _databaseServiceMock.Setup(service => service.GetDailyPnl()).Returns(150.5);

        // Act
        var result = _controller.GetDailyPnl();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(150.5);
    }

    [Fact]
    public void GetStats_ShouldReturnOk_WithStatsObject()
    {
        // Arrange
        var stats = new Stats(10, 20, 30, 100, 1, 2, 3, 10, 8, 80.0);
        _databaseServiceMock.Setup(service => service.GetStats()).Returns(stats);

        // Act
        var result = _controller.GetStats();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedStats = okResult.Value.Should().BeOfType<Stats>().Subject;
        
        returnedStats.PnlDay.Should().Be(10);
        returnedStats.Wins.Should().Be(8);
    }

    [Fact]
    public void GetPnlSummary_ShouldReturnOk_WithPnlSummary()
    {
        // Arrange
        var summary = new PnlSummary(10.5, 50.2, 100.1, 500.5);
        _databaseServiceMock.Setup(service => service.GetPnlSummary()).Returns(summary);

        // Act
        var result = _controller.GetPnlSummary();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSummary = okResult.Value.Should().BeOfType<PnlSummary>().Subject;

        returnedSummary.Daily.Should().Be(10.5);
        returnedSummary.Total.Should().Be(500.5);
    }

    [Fact]
    public void GetOpenPositions_ShouldReturnOk_WithList()
    {
        // Arrange
        var positions = new List<OpenPosition>
        {
            new("BTCUSDT", "BUY", 65000, 0.1, 6500, 60000, 70000, 95, DateTime.UtcNow)
        };
        _databaseServiceMock.Setup(service => service.GetOpenPositions()).Returns(positions);

        // Act
        var result = _controller.GetOpenPositions();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPositions = okResult.Value.Should().BeOfType<List<OpenPosition>>().Subject;

        returnedPositions.Should().HaveCount(1);
        returnedPositions[0].Symbol.Should().Be("BTCUSDT");
    }

    [Fact]
    public void GetLastTrades_ShouldReturnOk_WithLimitList()
    {
        // Arrange
        var trades = new List<ClosedTrade>
        {
            new("ETHUSDT", "SELL", 3500, 3400, 100, 2.8, 90, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow)
        };
        _databaseServiceMock.Setup(service => service.GetLastTrades(5)).Returns(trades);

        // Act
        var result = _controller.GetLastTrades(5);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTrades = okResult.Value.Should().BeOfType<List<ClosedTrade>>().Subject;

        returnedTrades.Should().HaveCount(1);
        returnedTrades[0].Pnl.Should().Be(100);
    }
}
