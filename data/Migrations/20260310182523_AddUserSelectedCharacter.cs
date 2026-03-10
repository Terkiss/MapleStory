using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapleStoryMarketGraph.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSelectedCharacter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectedCharacterName",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedCharacterOcid",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedCharacterName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SelectedCharacterOcid",
                table: "Users");
        }
    }
}
