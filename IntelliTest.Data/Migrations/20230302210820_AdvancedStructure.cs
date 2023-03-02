using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class AdvancedStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "AnswerIndex",
                table: "ClosedQuestions");

            migrationBuilder.RenameColumn(
                name: "Students",
                table: "Tests",
                newName: "CreatorId");

            migrationBuilder.AddColumn<string>(
                name: "AnswerIndexes",
                table: "ClosedQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: false),
                    School = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Grades = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Student_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teacher",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teacher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teacher_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClosedQuestionAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerIndex = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClosedQuestionAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClosedQuestionAnswer_ClosedQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "ClosedQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClosedQuestionAnswer_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpenQuestionAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenQuestionAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenQuestionAnswer_OpenQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "OpenQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpenQuestionAnswer_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestStudent",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestStudent", x => new { x.TestId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_TestStudent_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestStudent_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Class",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Class", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Class_Teacher_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teacher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentClass",
                columns: table => new
                {
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentClass", x => new { x.ClassId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_StudentClass_Class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentClass_Student_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tests_CreatorId",
                table: "Tests",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Class_TeacherId",
                table: "Class",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedQuestionAnswer_QuestionId",
                table: "ClosedQuestionAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedQuestionAnswer_StudentId",
                table: "ClosedQuestionAnswer",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenQuestionAnswer_QuestionId",
                table: "OpenQuestionAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenQuestionAnswer_StudentId",
                table: "OpenQuestionAnswer",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_UserId",
                table: "Student",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClass_StudentId",
                table: "StudentClass",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_UserId",
                table: "Teacher",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestStudent_StudentId",
                table: "TestStudent",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Teacher_CreatorId",
                table: "Tests",
                column: "CreatorId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Teacher_CreatorId",
                table: "Tests");

            migrationBuilder.DropTable(
                name: "ClosedQuestionAnswer");

            migrationBuilder.DropTable(
                name: "OpenQuestionAnswer");

            migrationBuilder.DropTable(
                name: "StudentClass");

            migrationBuilder.DropTable(
                name: "TestStudent");

            migrationBuilder.DropTable(
                name: "Class");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "Teacher");

            migrationBuilder.DropIndex(
                name: "IX_Tests_CreatorId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "AnswerIndexes",
                table: "ClosedQuestions");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Tests",
                newName: "Students");

            migrationBuilder.AddColumn<int>(
                name: "AnswerIndex",
                table: "ClosedQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Tests",
                columns: new[] { "Id", "AverageScore", "Color1", "Color2", "CreatedOn", "Description", "Grade", "IsDeleted", "MaxScore", "School", "Students", "Subject", "Time", "Title" },
                values: new object[] { 1, 67.5m, "#358DF1", "#2383f0", new DateTime(2023, 2, 26, 19, 53, 6, 58, DateTimeKind.Local).AddTicks(7307), "Просто тест", 10, false, 20, "ППМГ Добри Чинтулов", 15, "Физика", 30, "Електромагнитни вълни" });
        }
    }
}
