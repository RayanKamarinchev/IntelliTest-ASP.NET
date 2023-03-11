using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class questionScoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "Tests");

            migrationBuilder.AddColumn<int>(
                name: "MaxScore",
                table: "OpenQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxScore",
                table: "ClosedQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "ClosedQuestions",
                keyColumn: "Id",
                keyValue: 1,
                column: "MaxScore",
                value: 2);

            migrationBuilder.UpdateData(
                table: "OpenQuestions",
                keyColumn: "Id",
                keyValue: 2,
                column: "MaxScore",
                value: 3);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "OpenQuestions");

            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "ClosedQuestions");

            migrationBuilder.AddColumn<int>(
                name: "MaxScore",
                table: "Tests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "Id",
                keyValue: 1,
                column: "MaxScore",
                value: 20);
        }
    }
}
