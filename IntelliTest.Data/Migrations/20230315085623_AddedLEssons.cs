using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class AddedLEssons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LessonId",
                table: "OpenQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LessonId",
                table: "ClosedQuestions",
                type: "int",
                nullable: true);

            
            migrationBuilder.CreateIndex(
                name: "IX_OpenQuestions_LessonId",
                table: "OpenQuestions",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedQuestions_LessonId",
                table: "ClosedQuestions",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClosedQuestions_Lesson_LessonId",
                table: "ClosedQuestions",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OpenQuestions_Lesson_LessonId",
                table: "OpenQuestions",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClosedQuestions_Lesson_LessonId",
                table: "ClosedQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_OpenQuestions_Lesson_LessonId",
                table: "OpenQuestions");

            migrationBuilder.DropTable(
                name: "Lesson");

            migrationBuilder.DropIndex(
                name: "IX_OpenQuestions_LessonId",
                table: "OpenQuestions");

            migrationBuilder.DropIndex(
                name: "IX_ClosedQuestions_LessonId",
                table: "ClosedQuestions");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "LessonId",
                table: "OpenQuestions");

            migrationBuilder.DropColumn(
                name: "LessonId",
                table: "ClosedQuestions");
        }
    }
}
