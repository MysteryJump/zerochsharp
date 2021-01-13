using Microsoft.EntityFrameworkCore.Migrations;

namespace ZerochSharp.Migrations
{
    public partial class addrestrictedusers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RestrictedUsers",
                table: "Boards",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestrictedUsers",
                table: "Boards");
        }
    }
}
