using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingProject.Persistence.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUsdtColumnsSuffix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "total_usdt",
                table: "portfolio_snapshots",
                newName: "total");

            migrationBuilder.RenameColumn(
                name: "free_usdt",
                table: "portfolio_snapshots",
                newName: "free");

            migrationBuilder.RenameColumn(
                name: "pnl_usdt",
                table: "trades",
                newName: "pnl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "total",
                table: "portfolio_snapshots",
                newName: "total_usdt");

            migrationBuilder.RenameColumn(
                name: "free",
                table: "portfolio_snapshots",
                newName: "free_usdt");

            migrationBuilder.RenameColumn(
                name: "pnl",
                table: "trades",
                newName: "pnl_usdt");
        }
    }
}
