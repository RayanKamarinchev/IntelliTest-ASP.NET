using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class FakeIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Teachers_TeacherId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_ClosedQuestionAnswers_ClosedQuestions_QuestionId",
                table: "ClosedQuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClosedQuestionAnswers_Students_StudentId",
                table: "ClosedQuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClosedQuestions_Lessons_LessonId",
                table: "ClosedQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_ClosedQuestions_Tests_TestId",
                table: "ClosedQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonLike_Lessons_LessonId",
                table: "LessonLike");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Teachers_CreatorId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_OpenQuestionAnswers_OpenQuestions_QuestionId",
                table: "OpenQuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_OpenQuestionAnswers_Students_StudentId",
                table: "OpenQuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_OpenQuestions_Lessons_LessonId",
                table: "OpenQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_OpenQuestions_Tests_TestId",
                table: "OpenQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Reads_Lessons_LessonId",
                table: "Reads");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_Classes_ClassId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_Students_StudentId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_TestLike_Tests_TestId",
                table: "TestLike");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Students_StudentId",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_TestId",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Teachers_CreatorId",
                table: "Tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tests",
                table: "Tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OpenQuestions",
                table: "OpenQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClosedQuestions",
                table: "ClosedQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClosedQuestionAnswers",
                table: "ClosedQuestionAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Classes",
                table: "Classes");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Tests",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Tests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Teachers",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Teachers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Students",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Students",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "OpenQuestions",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "OpenQuestions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Lessons",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Lessons",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClosedQuestions",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ClosedQuestions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ClosedQuestionAnswers",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ClosedQuestionAnswers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Classes",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Classes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tests",
                table: "Tests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OpenQuestions",
                table: "OpenQuestions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClosedQuestions",
                table: "ClosedQuestions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClosedQuestionAnswers",
                table: "ClosedQuestionAnswers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classes",
                table: "Classes",
                column: "Id");


            migrationBuilder.CreateTable(
                name: "ClassTest",
                columns: table => new
                {
                    TestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTest", x => new { x.TestId, x.ClassId });
                    table.ForeignKey(
                        name: "FK_ClassTest_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassTest_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassTest_ClassId",
                table: "ClassTest",
                column: "ClassId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
