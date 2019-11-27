using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZerochSharp.Migrations
{
    public partial class revisesomeparams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "Sessions");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Sessions",
                nullable: false,
                defaultValue: 0L)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Sessions");

            migrationBuilder.AddColumn<long>(
                name: "Key",
                table: "Sessions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions",
                column: "Key");
        }
    }
}
