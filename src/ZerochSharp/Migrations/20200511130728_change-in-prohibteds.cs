using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZerochSharp.Migrations
{
    public partial class changeinprohibteds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NgWords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Pattern = table.Column<string>(nullable: true),
                    IsRegex = table.Column<bool>(nullable: false),
                    BoardKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NgWords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RestrictedUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Pattern = table.Column<string>(nullable: true),
                    IsRegex = table.Column<bool>(nullable: false),
                    BoardKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestrictedUsers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NgWords");

            migrationBuilder.DropTable(
                name: "RestrictedUsers");
        }
    }
}
