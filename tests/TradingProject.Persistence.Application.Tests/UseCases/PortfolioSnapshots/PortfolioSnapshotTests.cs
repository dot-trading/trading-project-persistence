using Xunit;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Mappings;
using TradingProject.Persistence.Application.UseCases.PortfolioSnapshots;
using TradingProject.Persistence.Application.UseCases.PortfolioSnapshots.CreatePortfolioSnapshot;
using TradingProject.Persistence.Application.UseCases.PortfolioSnapshots.GetPortfolioSnapshots;
using TradingProject.Persistence.Domain.Entities;
using TradingProject.Persistence.Infrastructure.Persistence;

namespace TradingProject.Persistence.Application.Tests.UseCases.PortfolioSnapshots;

public class PortfolioSnapshotTests
{
    private readonly TradingDbContext _context;
    private readonly IMapper _mapper;

    public PortfolioSnapshotTests()
    {
        var options = new DbContextOptionsBuilder<TradingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TradingDbContext(options);

        var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<PortfolioSnapshotProfile>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task GetPortfolioSnapshots_ShouldReturnPaginatedResults()
    {
        // Arrange
        var handler = new GetPortfolioSnapshotsQueryHandler(_context, _mapper);
        var snapshots = Enumerable.Range(1, 10).Select(i => new PortfolioSnapshot
        {
            Id = i,
            Total = 1000 * i,
            Free = 500 * i,
            PositionsCount = i,
            CreatedAt = DateTime.UtcNow.AddMinutes(-i)
        }).ToList();

        await _context.PortfolioSnapshots.AddRangeAsync(snapshots);
        await _context.SaveChangesAsync();

        var query = new GetPortfolioSnapshotsQuery(Limit: 3, Page: 2);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result[0].Id.Should().Be(4); // DESC by CreatedAt
    }

    [Fact]
    public async Task CreatePortfolioSnapshot_ShouldCreateAndReturnResponse()
    {
        // Arrange
        var handler = new CreatePortfolioSnapshotCommandHandler(_context, _mapper);
        var request = new CreatePortfolioSnapshotRequest(
            Total: 10000,
            Free: 5000,
            PositionsCount: 3,
            DailyPnl: 100,
            TotalPnl: 1000
        );
        var command = new CreatePortfolioSnapshotCommand(request);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Total.Should().Be(10000);
        result.Id.Should().BeGreaterThan(0);

        var entity = await _context.PortfolioSnapshots.FirstOrDefaultAsync(s => s.Id == result.Id);
        entity.Should().NotBeNull();
        entity!.Total.Should().Be(10000);
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
