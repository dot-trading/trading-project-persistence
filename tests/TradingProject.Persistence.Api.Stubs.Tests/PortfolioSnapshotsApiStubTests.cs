using FluentAssertions;
using TradingProject.Persistence.Api.Stubs.Models;
using TradingProject.Persistence.Api.Stubs.Stubs;

namespace TradingProject.Persistence.Api.Stubs.Tests;

public class PortfolioSnapshotsApiStubTests
{
    [Fact]
    public async Task CreateSnapshot_ShouldReturnResponse()
    {
        // Arrange
        var stub = new PortfolioSnapshotsApiStub();
        var request = new CreatePortfolioSnapshotRequest(
            Total: 15000,
            Free: 5000,
            PositionsCount: 3,
            DailyPnl: 250,
            TotalPnl: 5000);

        // Act
        var result = await stub.CreatePortfolioSnapshotAsync(request);

        // Assert
        result.Total.Should().Be(15000);
        result.Free.Should().Be(5000);
        result.PositionsCount.Should().Be(3);
        result.DailyPnl.Should().Be(250);
        result.TotalPnl.Should().Be(5000);
    }

    [Fact]
    public async Task GetSnapshots_ShouldReturnPaginatedResults()
    {
        // Arrange
        var stub = new PortfolioSnapshotsApiStub();
        for (int i = 0; i < 5; i++)
        {
            await stub.CreatePortfolioSnapshotAsync(new CreatePortfolioSnapshotRequest(
                Total: 10000 + i, Free: 5000, PositionsCount: 2));
        }

        // Act
        var result = await stub.GetPortfolioSnapshotsAsync(limit: 2, page: 2);

        // Assert
        result.Payload.Should().HaveCount(2);
        result.TotalCount.Should().Be(5);
    }
}
