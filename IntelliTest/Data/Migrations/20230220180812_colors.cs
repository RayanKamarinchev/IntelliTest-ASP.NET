using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class colors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Color",
                table: "Tests",
                newName: "Color2");

            migrationBuilder.AddColumn<string>(
                name: "Color1",
                table: "Tests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color1",
                table: "Tests");

            migrationBuilder.RenameColumn(
                name: "Color2",
                table: "Tests",
                newName: "Color");
        }
    }
}
