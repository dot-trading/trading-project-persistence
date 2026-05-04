using Xunit;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Mappings;
using TradingProject.Persistence.Application.UseCases.Opportunities.CreateOpportunity;
using TradingProject.Persistence.Infrastructure.Persistence;

namespace TradingProject.Persistence.Application.Tests.UseCases.Opportunities.CreateOpportunity;

public class CreateOpportunityCommandHandlerTests
{
    private readonly TradingDbContext _context;
    private readonly IMapper _mapper;
    private readonly CreateOpportunityCommandHandler _handler;

    public CreateOpportunityCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<TradingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TradingDbContext(options);

        var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<OpportunityProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new CreateOpportunityCommandHandler(_context, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldCreateOpportunityAndReturnResponse()
    {
        // Arrange
        var request = new CreateOpportunityRequest(
            Symbol: "BTCUSDT",
            Score: 80,
            Signal: "BUY",
            Reason: "Bullish divergence",
            Price: 60000,
            TargetPct: 5,
            StopLossPct: 2
        );
        var command = new CreateOpportunityCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Symbol.Should().Be("BTCUSDT");
        result.Score.Should().Be(80);
        result.Signal.Should().Be("BUY");
        result.Id.Should().BeGreaterThan(0);

        var entity = await _context.Opportunities.FirstOrDefaultAsync(o => o.Id == result.Id);
        entity.Should().NotBeNull();
        entity!.Symbol.Should().Be("BTCUSDT");
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
