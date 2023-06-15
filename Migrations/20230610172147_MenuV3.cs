using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SavorAsia.Migrations
{
    public partial class MenuV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Rice",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Ramen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Noodles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Drinks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Rice");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Ramen");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Noodles");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Drinks");
        }
    }
}
