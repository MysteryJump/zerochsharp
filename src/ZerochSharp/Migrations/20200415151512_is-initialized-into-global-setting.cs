using Microsoft.EntityFrameworkCore.Migrations;

namespace ZerochSharp.Migrations
{
    public partial class isinitializedintoglobalsetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInitialized",
                table: "GlobalSettings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "GlobalSettings",
                columns: new[] { "Id", "IsInitialized", "IsInitializedElasticsearchService" },
                values: new object[] { 1, false, false });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GlobalSettings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "IsInitialized",
                table: "GlobalSettings");
        }
    }
}
