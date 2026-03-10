using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapleStoryMarketGraph.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMesoMarketTrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MesoMarketTrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TradeType = table.Column<string>(type: "TEXT", nullable: false),
                    MesoAmount = table.Column<double>(type: "REAL", nullable: false),
                    PointAmount = table.Column<double>(type: "REAL", nullable: false),
                    UnitPrice = table.Column<double>(type: "REAL", nullable: false),
                    Memo = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MesoMarketTrades", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MesoMarketTrades");
        }
    }
}
