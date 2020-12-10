using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZerochSharp.Migrations
{
    public partial class addcapandcapgroup2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CapGroupId",
                table: "Boards",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CapGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CapGroupName = table.Column<string>(nullable: true),
                    IsAvailable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Caps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CapName = table.Column<string>(nullable: false),
                    CapId = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boards_CapGroupId",
                table: "Boards",
                column: "CapGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_CapGroups_CapGroupId",
                table: "Boards",
                column: "CapGroupId",
                principalTable: "CapGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_CapGroups_CapGroupId",
                table: "Boards");

            migrationBuilder.DropTable(
                name: "CapGroups");

            migrationBuilder.DropTable(
                name: "Caps");

            migrationBuilder.DropIndex(
                name: "IX_Boards_CapGroupId",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "CapGroupId",
                table: "Boards");
        }
    }
}
