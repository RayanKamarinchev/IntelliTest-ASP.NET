using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class IdsRenamed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FakeId",
                table: "Tests",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FakeId",
                table: "Teachers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FakeId",
                table: "Students",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FakeId",
                table: "OpenQuestions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FakeId",
                table: "OpenQuestionAnswers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FakeId",
                table: "Lessons",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FakeId",
                table: "ClosedQuestions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FakeId",
                table: "ClosedQuestionAnswers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "FakeId",
                table: "Classes",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tests",
                newName: "FakeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Teachers",
                newName: "FakeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Students",
                newName: "FakeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OpenQuestions",
                newName: "FakeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OpenQuestionAnswers",
                newName: "FakeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Lessons",
                newName: "FakeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ClosedQuestions",
                newName: "FakeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ClosedQuestionAnswers",
                newName: "FakeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Classes",
                newName: "FakeId");
        }
    }
}
