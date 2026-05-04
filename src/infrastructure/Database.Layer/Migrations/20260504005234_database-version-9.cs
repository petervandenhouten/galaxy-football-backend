using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Layer.Migrations
{
    /// <inheritdoc />
    public partial class databaseversion9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "league_league_results");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "league_league_results",
                columns: table => new
                {
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeagueResultId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_league_league_results", x => new { x.LeagueId, x.LeagueResultId });
                });
        }
    }
}
