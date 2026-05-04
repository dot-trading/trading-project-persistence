using Xunit;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Mappings;
using TradingProject.Persistence.Application.UseCases.Opportunities.GetOpportunities;
using TradingProject.Persistence.Domain.Entities;
using TradingProject.Persistence.Infrastructure.Persistence;

namespace TradingProject.Persistence.Application.Tests.UseCases.Opportunities.GetOpportunities;

public class GetOpportunitiesQueryHandlerTests
{
    private readonly TradingDbContext _context;
    private readonly IMapper _mapper;
    private readonly GetOpportunitiesQueryHandler _handler;

    public GetOpportunitiesQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<TradingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TradingDbContext(options);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<OpportunityProfile>());
        _mapper = config.CreateMapper();

        _handler = new GetOpportunitiesQueryHandler(_context, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedResults()
    {
        // Arrange
        var opportunities = Enumerable.Range(1, 10).Select(i => new Opportunity
        {
            Id = i,
            Symbol = "BTCUSDT",
            CreatedAt = DateTime.UtcNow.AddMinutes(-i),
            Signal = "BUY",
            Reason = "Test",
            Price = 50000 + i
        }).ToList();

        await _context.Opportunities.AddRangeAsync(opportunities);
        await _context.SaveChangesAsync();

        var query = new GetOpportunitiesQuery(Limit: 3, Page: 2);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result[0].Id.Should().Be(4); // Sorted by CreatedAt DESC, so 1 is newest, 10 is oldest.
        // 1, 2, 3 are page 1. 4, 5, 6 are page 2.
    }

    [Fact]
    public async Task Handle_ShouldFilterBySymbol()
    {
        // Arrange
        await _context.Opportunities.AddRangeAsync(
            new Opportunity { Symbol = "BTCUSDT", Price = 100, Signal = "BUY", Reason = "X", CreatedAt = DateTime.UtcNow },
            new Opportunity { Symbol = "ETHUSDT", Price = 100, Signal = "BUY", Reason = "X", CreatedAt = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        var query = new GetOpportunitiesQuery(Symbol: "ETHUSDT");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Symbol.Should().Be("ETHUSDT");
    }
}
