using Xunit;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Mappings;
using TradingProject.Persistence.Application.UseCases.Trades.CreateTrade;
using TradingProject.Persistence.Application.UseCases.Trades.GetTrades;
using TradingProject.Persistence.Application.UseCases.Trades.UpdateTrade;
using TradingProject.Persistence.Domain.Entities;
using TradingProject.Persistence.Infrastructure.Persistence;

namespace TradingProject.Persistence.Application.Tests.UseCases.Trades;

public class TradeTests
{
    private readonly TradingDbContext _context;
    private readonly IMapper _mapper;

    public TradeTests()
    {
        var options = new DbContextOptionsBuilder<TradingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TradingDbContext(options);

        var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<TradeProfile>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task GetTrades_ShouldReturnPaginatedResults()
    {
        // Arrange
        var handler = new GetTradesQueryHandler(_context, _mapper);
        var trades = Enumerable.Range(1, 10).Select(i => new Trade
        {
            Id = i,
            Symbol = "BTCUSDT",
            Side = "BUY",
            Status = "open",
            Price = 60000,
            Quantity = 0.1,
            Value = 6000,
            CreatedAt = DateTime.UtcNow.AddMinutes(-i)
        }).ToList();

        await _context.Trades.AddRangeAsync(trades);
        await _context.SaveChangesAsync();

        var query = new GetTradesQuery(Limit: 3, Page: 2);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result[0].Id.Should().Be(4);
    }

    [Fact]
    public async Task CreateTrade_ShouldCreateAndReturnResponse()
    {
        // Arrange
        var handler = new CreateTradeCommandHandler(_context, _mapper);
        var request = new CreateTradeRequest(
            Symbol: "ETHUSDT",
            Side: "BUY",
            Price: 3000,
            Quantity: 1,
            Value: 3000
        );
        var command = new CreateTradeCommand(request);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Symbol.Should().Be("ETHUSDT");
        result.Status.Should().Be("open");

        var entity = await _context.Trades.FirstOrDefaultAsync(t => t.Id == result.Id);
        entity.Should().NotBeNull();
        entity!.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UpdateTrade_ShouldApplyPartialUpdates()
    {
        // Arrange
        var trade = new Trade { Symbol = "BTCUSDT", Status = "open", CreatedAt = DateTime.UtcNow };
        _context.Trades.Add(trade);
        await _context.SaveChangesAsync();

        var handler = new UpdateTradeCommandHandler(_context, _mapper);
        var updates = new UpdateTradeRequest(Status: "closed", Pnl: 150);
        var command = new UpdateTradeCommand(trade.Id, updates);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be("closed");
        result.Pnl.Should().Be(150);
        result.CloseAt.Should().NotBeNull();

        var entity = await _context.Trades.FindAsync(trade.Id);
        entity!.Status.Should().Be("closed");
        entity.Pnl.Should().Be(150);
    }
}
