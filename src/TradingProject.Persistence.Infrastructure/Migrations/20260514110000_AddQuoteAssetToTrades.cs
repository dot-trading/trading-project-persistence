using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingProject.Persistence.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuoteAssetToTrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "quote_asset",
                table: "trades",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quote_asset",
                table: "trades");
        }
    }
}
