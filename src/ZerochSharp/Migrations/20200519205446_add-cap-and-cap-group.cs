using Microsoft.EntityFrameworkCore.Migrations;

namespace ZerochSharp.Migrations
{
    public partial class addcapandcapgroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProhibitedWords",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "RestrictedUsers",
                table: "Boards");

            migrationBuilder.AddColumn<string>(
                name: "BoardCategory",
                table: "Boards",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardCategory",
                table: "Boards");

            migrationBuilder.AddColumn<string>(
                name: "ProhibitedWords",
                table: "Boards",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestrictedUsers",
                table: "Boards",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
