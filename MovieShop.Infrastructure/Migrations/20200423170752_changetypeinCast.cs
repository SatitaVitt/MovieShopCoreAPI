using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieShop.Infrastructure.Migrations
{
    public partial class changetypeinCast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TmdbUtrl",
                table: "Crew");

            migrationBuilder.AddColumn<string>(
                name: "TmdbUrl",
                table: "Crew",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TmdbUrl",
                table: "Crew");

            migrationBuilder.AddColumn<string>(
                name: "TmdbUtrl",
                table: "Crew",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
