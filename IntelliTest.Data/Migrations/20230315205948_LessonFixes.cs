using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class LessonFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClosedQuestions_Lesson_LessonId",
                table: "ClosedQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Lesson_Teachers_CreatorId",
                table: "Lesson");

            migrationBuilder.DropForeignKey(
                name: "FK_OpenQuestions_Lesson_LessonId",
                table: "OpenQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lesson",
                table: "Lesson");

            migrationBuilder.RenameTable(
                name: "Lesson",
                newName: "Lessons");

            migrationBuilder.RenameIndex(
                name: "IX_Lesson_CreatorId",
                table: "Lessons",
                newName: "IX_Lessons_CreatorId");

            migrationBuilder.AddColumn<int>(
                name: "ReadingTime",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClosedQuestions_Lessons_LessonId",
                table: "ClosedQuestions",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Teachers_CreatorId",
                table: "Lessons",
                column: "CreatorId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpenQuestions_Lessons_LessonId",
                table: "OpenQuestions",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClosedQuestions_Lessons_LessonId",
                table: "ClosedQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Teachers_CreatorId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_OpenQuestions_Lessons_LessonId",
                table: "OpenQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "ReadingTime",
                table: "Lessons");

            migrationBuilder.RenameTable(
                name: "Lessons",
                newName: "Lesson");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_CreatorId",
                table: "Lesson",
                newName: "IX_Lesson_CreatorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lesson",
                table: "Lesson",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClosedQuestions_Lesson_LessonId",
                table: "ClosedQuestions",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lesson_Teachers_CreatorId",
                table: "Lesson",
                column: "CreatorId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpenQuestions_Lesson_LessonId",
                table: "OpenQuestions",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "Id");
        }
    }
}
