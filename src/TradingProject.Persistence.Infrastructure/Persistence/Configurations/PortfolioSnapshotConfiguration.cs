using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Infrastructure.Persistence.Configurations;

public class PortfolioSnapshotConfiguration : IEntityTypeConfiguration<PortfolioSnapshot>
{
    public void Configure(EntityTypeBuilder<PortfolioSnapshot> entity)
    {
        entity.ToTable("portfolio_snapshots");
        entity.HasKey(s => s.Id);

        entity.Property(s => s.Id).HasColumnName("id");
        entity.Property(s => s.Total).HasColumnName("total_usdt");
        entity.Property(s => s.Free).HasColumnName("free_usdt");
        entity.Property(s => s.PositionsCount).HasColumnName("positions_count");
        entity.Property(s => s.DailyPnl).HasColumnName("daily_pnl");
        entity.Property(s => s.TotalPnl).HasColumnName("total_pnl");
        entity.Property(s => s.CreatedAt).HasColumnName("created_at");
    }
}
