using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class scoreSaving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "OpenQuestionAnswers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Points",
                table: "OpenQuestionAnswers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "ClosedQuestionAnswers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Points",
                table: "ClosedQuestionAnswers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Explanation",
                table: "OpenQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "OpenQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "Explanation",
                table: "ClosedQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "ClosedQuestionAnswers");
        }
    }
}
