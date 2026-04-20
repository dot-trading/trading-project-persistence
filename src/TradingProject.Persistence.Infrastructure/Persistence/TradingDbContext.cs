using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Infrastructure.Persistence;

public class TradingDbContext(DbContextOptions<TradingDbContext> options) : DbContext(options), ITradingDbContext
{
    public DbSet<Trade> Trades => Set<Trade>();
    public DbSet<Opportunity> Opportunities => Set<Opportunity>();
    public DbSet<PortfolioSnapshot> PortfolioSnapshots => Set<PortfolioSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TradingDbContext).Assembly);
    }
}
