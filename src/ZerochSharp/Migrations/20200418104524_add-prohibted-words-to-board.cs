using Microsoft.EntityFrameworkCore.Migrations;

namespace ZerochSharp.Migrations
{
    public partial class addprohibtedwordstoboard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProhibitedWords",
                table: "Boards",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProhibitedWords",
                table: "Boards");
        }
    }
}
