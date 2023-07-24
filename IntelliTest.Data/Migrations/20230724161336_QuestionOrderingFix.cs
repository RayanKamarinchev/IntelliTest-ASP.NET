using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class QuestionOrderingFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "OpenQuestions");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ClosedQuestions");

            migrationBuilder.AddColumn<string>(
                name: "QuestionsOrder",
                table: "Tests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionsOrder",
                table: "Tests");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "OpenQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ClosedQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
