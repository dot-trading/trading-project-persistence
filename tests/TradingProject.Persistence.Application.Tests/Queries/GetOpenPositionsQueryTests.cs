using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TradingProject.Persistence.Application.Mappings;
using TradingProject.Persistence.Application.Queries;
using TradingProject.Persistence.Domain.Entities;
using TradingProject.Persistence.Infrastructure.Persistence;

namespace TradingProject.Persistence.Application.Tests.Queries;

public class GetOpenPositionsQueryTests
{
    private readonly TradingDbContext _context;
    private readonly IMapper _mapper;
    private readonly GetOpenPositionsQueryHandler _handler;

    public GetOpenPositionsQueryTests()
    {
        var options = new DbContextOptionsBuilder<TradingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TradingDbContext(options);

        var config = new MapperConfiguration(
            cfg => cfg.AddProfile<TradeProfile>(),
            NullLoggerFactory.Instance);
        _mapper = config.CreateMapper();

        _handler = new GetOpenPositionsQueryHandler(_context, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyOpenPositions()
    {
        // Arrange
        await _context.Trades.AddRangeAsync(
            new Trade
            {
                Symbol = "BTCUSDT",
                Side = "BUY",
                Status = "open",
                Price = 60000,
                Quantity = 0.1,
                Value = 6000,
                CreatedAt = DateTime.UtcNow
            },
            new Trade
            {
                Symbol = "ETHUSDT",
                Side = "BUY",
                Status = "closed",
                Price = 3000,
                Quantity = 1,
                Value = 3000,
                CreatedAt = DateTime.UtcNow
            }
        );
        await _context.SaveChangesAsync();

        var query = new GetOpenPositionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Symbol.Should().Be("BTCUSDT");
    }

    [Fact]
    public async Task Handle_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var trade = new Trade
        {
            Symbol = "BTCUSDT",
            Side = "BUY",
            Status = "open",
            Price = 60000,
            Quantity = 0.1,
            Value = 6000,
            StopLoss = 58000,
            TakeProfit = 65000,
            AiScore = 85,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Trades.AddAsync(trade);
        await _context.SaveChangesAsync();

        var query = new GetOpenPositionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        var position = result[0];
        position.Symbol.Should().Be("BTCUSDT");
        position.Side.Should().Be("BUY");
        position.Entry.Should().Be(60000);
        position.Quantity.Should().Be(0.1);
        position.UsdtValue.Should().Be(6000);
        position.StopLoss.Should().Be(58000);
        position.TakeProfit.Should().Be(65000);
        position.AiScore.Should().Be(85);
    }

    [Fact]
    public async Task Handle_ShouldOrderByCreatedAtDescending()
    {
        // Arrange
        var now = DateTime.UtcNow;
        await _context.Trades.AddRangeAsync(
            new Trade { Symbol = "OLDEST", Side = "BUY", Status = "open", Price = 100, Quantity = 1, Value = 100, CreatedAt = now.AddHours(-2) },
            new Trade { Symbol = "NEWEST", Side = "BUY", Status = "open", Price = 100, Quantity = 1, Value = 100, CreatedAt = now },
            new Trade { Symbol = "MIDDLE", Side = "BUY", Status = "open", Price = 100, Quantity = 1, Value = 100, CreatedAt = now.AddHours(-1) }
        );
        await _context.SaveChangesAsync();

        var query = new GetOpenPositionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result[0].Symbol.Should().Be("NEWEST");
        result[1].Symbol.Should().Be("MIDDLE");
        result[2].Symbol.Should().Be("OLDEST");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyListWhenNoOpenPositions()
    {
        // Arrange
        await _context.Trades.AddAsync(new Trade
        {
            Symbol = "BTCUSDT",
            Side = "BUY",
            Status = "closed",
            Price = 60000,
            Quantity = 0.1,
            Value = 6000,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var query = new GetOpenPositionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
