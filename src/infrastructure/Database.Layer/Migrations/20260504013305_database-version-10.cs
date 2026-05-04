using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Layer.Migrations
{
    /// <inheritdoc />
    public partial class databaseversion10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompetitionId",
                table: "calender");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "sponsors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Budget = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sponsors", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sponsors");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "matches");

            migrationBuilder.AddColumn<int>(
                name: "CompetitionId",
                table: "calender",
                type: "integer",
                nullable: true);
        }
    }
}
