using Cortex.Mediator;
using Cortex.Mediator.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TradingProject.Persistence.Api.Controllers.V1;

namespace TradingProject.Persistence.Api.Tests;

#pragma warning disable CS0618
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

    // [Fact]
    // public async Task GetDailyPnl_ShouldReturnOk_WithPnl()
    // {
    //     _mediatorMock
    //         .Setup(m => m.SendQueryAsync(It.IsAny<IQuery<double>>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(150.5);

    //     var result = await _controller.GetDailyPnl("EUR",CancellationToken.None);

    //     var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
    //     okResult.Value.Should().Be(150.5);
    // }

    
}
