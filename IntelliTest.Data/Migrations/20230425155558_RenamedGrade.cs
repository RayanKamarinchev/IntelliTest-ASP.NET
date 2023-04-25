using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class RenamedGrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageScore",
                table: "Tests");

            migrationBuilder.RenameColumn(
                name: "Grade",
                table: "TestResults",
                newName: "Mark");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Mark",
                table: "TestResults",
                newName: "Grade");

            migrationBuilder.AddColumn<decimal>(
                name: "AverageScore",
                table: "Tests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
