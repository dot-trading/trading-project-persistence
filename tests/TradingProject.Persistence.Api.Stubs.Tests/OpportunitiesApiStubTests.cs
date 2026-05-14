using FluentAssertions;
using TradingProject.Persistence.Api.Stubs.Models;
using TradingProject.Persistence.Api.Stubs.Stubs;

namespace TradingProject.Persistence.Api.Stubs.Tests;

public class OpportunitiesApiStubTests
{
    [Fact]
    public async Task CreateOpportunity_ShouldReturnResponse()
    {
        // Arrange
        var stub = new OpportunitiesApiStub();
        var request = new CreateOpportunityRequest(
            Symbol: "BTCUSDT",
            Score: 85,
            Signal: "BUY",
            Reason: "Strong support level",
            Price: 60000);

        // Act
        var result = await stub.CreateOpportunityAsync(request);

        // Assert
        result.Symbol.Should().Be("BTCUSDT");
        result.Score.Should().Be(85);
        result.Signal.Should().Be("BUY");
        result.IsApproved.Should().BeTrue();
        result.Acted.Should().BeFalse();
    }

    [Fact]
    public async Task GetOpportunities_ShouldFilterBySymbol()
    {
        // Arrange
        var stub = new OpportunitiesApiStub();
        await stub.CreateOpportunityAsync(new CreateOpportunityRequest("BTCUSDT", 85, "BUY", "Test", 60000));
        await stub.CreateOpportunityAsync(new CreateOpportunityRequest("ETHUSDT", 70, "BUY", "Test", 3000));

        // Act
        var result = await stub.GetOpportunitiesAsync(symbol: "ETHUSDT");

        // Assert
        result.Payload.Should().HaveCount(1);
        result.Payload[0].Symbol.Should().Be("ETHUSDT");
    }
}
