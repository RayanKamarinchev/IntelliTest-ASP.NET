using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Student",
                columns: new[] { "Id", "Grade", "Grades", "School", "UserId" },
                values: new object[] { 1, 8, "6&5", "PPMG Dobri Chintulov", "4fb46fcc-ad1d-4120-835d-d351849efc73" });

            migrationBuilder.InsertData(
                table: "Teacher",
                columns: new[] { "Id", "UserId" },
                values: new object[] { 1, "e9242048-504d-4ea9-9776-47691844c4a6" });

            migrationBuilder.InsertData(
                table: "Tests",
                columns: new[] { "Id", "AverageScore", "Color1", "Color2", "CreatedOn", "CreatorId", "Description", "Grade", "IsDeleted", "MaxScore", "School", "Subject", "Time", "Title" },
                values: new object[] { 1, 67.5m, "#358DF1", "#2383f0", new DateTime(2023, 2, 26, 19, 53, 6, 58, DateTimeKind.Local).AddTicks(7307), 1, "Просто тест", 10, false, 20, "ППМГ Добри Чинтулов", "Физика", 30, "Електромагнитни вълни" });

            migrationBuilder.InsertData(
                table: "Class",
                columns: new[] { "Id", "Description", "Name", "TeacherId" },
                values: new object[] { 1, "This is the first class ever made", "Nothing class", 1 });

            migrationBuilder.InsertData(
                table: "ClosedQuestions",
                columns: new[] { "Id", "AnswerIndexes", "Answers", "IsDeleted", "Order", "TestId", "Text" },
                values: new object[] { 1, "1", "Ti&Az&dvamata&nikoi", false, 1, 1, "Koi suzdade testut" });

            migrationBuilder.InsertData(
                table: "OpenQuestions",
                columns: new[] { "Id", "Answer", "IsDeleted", "Order", "TestId", "Text" },
                values: new object[] { 2, "Az", false, 0, 1, "Koi suzdade testut" });

            migrationBuilder.InsertData(
                table: "TestStudent",
                columns: new[] { "StudentId", "TestId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "ClosedQuestionAnswer",
                columns: new[] { "Id", "AnswerIndex", "QuestionId", "StudentId" },
                values: new object[] { 1, 0, 1, 1 });

            migrationBuilder.InsertData(
                table: "OpenQuestionAnswer",
                columns: new[] { "Id", "Answer", "QuestionId", "StudentId" },
                values: new object[] { 1, "Ti", 2, 1 });

            migrationBuilder.InsertData(
                table: "StudentClass",
                columns: new[] { "ClassId", "StudentId" },
                values: new object[] { 1, 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ClosedQuestionAnswer",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OpenQuestionAnswer",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "StudentClass",
                keyColumns: new[] { "ClassId", "StudentId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "TestStudent",
                keyColumns: new[] { "StudentId", "TestId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "Class",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ClosedQuestions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OpenQuestions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Student",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Teacher",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
