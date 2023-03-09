using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class multipleAnswers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerIndex",
                table: "ClosedQuestionAnswers");

            migrationBuilder.AddColumn<string>(
                name: "AnswerIndexes",
                table: "ClosedQuestionAnswers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ClosedQuestionAnswers",
                keyColumn: "Id",
                keyValue: 1,
                column: "AnswerIndexes",
                value: "0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerIndexes",
                table: "ClosedQuestionAnswers");

            migrationBuilder.AddColumn<int>(
                name: "AnswerIndex",
                table: "ClosedQuestionAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
