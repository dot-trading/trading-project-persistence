using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingProject.Persistence.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBinanceOrderIdToTrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "binance_order_id",
                table: "trades",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "binance_order_id",
                table: "trades");
        }
    }
}
