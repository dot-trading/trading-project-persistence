using FluentAssertions;
using System.Net.Http.Json;
using TradingProject.Persistence.Application.Common.Models;

namespace TradingProject.Persistence.IntegrationTests;

public class TradingDataIntegrationTests(TradingPersistenceApiFactory factory)
    : IClassFixture<TradingPersistenceApiFactory>
{
    [Fact]
    public async Task GetStats_ShouldReturn_200_And_ValidStats()
    {
        // Arrange
        var client = factory.CreateClient();
    
        // Act
        var response = await client.GetAsync("/api/TradingData/stats");
    
        // Assert
        response.EnsureSuccessStatusCode();
        var stats = await response.Content.ReadFromJsonAsync<Stats>();
        stats.Should().NotBeNull();
        stats!.PnlTotal.Should().Be(0); // Given db is empty initially
    }

    [Fact]
    public async Task GetOpenPositionsCount_ShouldReturn_200_And_Zero()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/TradingData/positions/open/count");

        // Assert
        response.EnsureSuccessStatusCode();
        var count = await response.Content.ReadFromJsonAsync<int>();
        count.Should().Be(0);
    }
}
