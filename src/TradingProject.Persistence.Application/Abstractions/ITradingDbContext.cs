using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Application.Abstractions;

public interface ITradingDbContext
{
    DbSet<Trade> Trades { get; }
    DbSet<Opportunity> Opportunities { get; }
    DbSet<PortfolioSnapshot> PortfolioSnapshots { get; }

    int SaveChanges();
    int SaveChanges(bool acceptAllChangesOnSuccess);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default);
}