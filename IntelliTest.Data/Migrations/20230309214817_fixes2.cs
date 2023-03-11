using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class fixes2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClosedQuestionAnswers_Tests_TestId",
                table: "ClosedQuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_OpenQuestionAnswers_Tests_TestId",
                table: "OpenQuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_OpenQuestionAnswers_TestId",
                table: "OpenQuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_ClosedQuestionAnswers_TestId",
                table: "ClosedQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "TestId",
                table: "OpenQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "TestId",
                table: "ClosedQuestionAnswers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestId",
                table: "OpenQuestionAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TestId",
                table: "ClosedQuestionAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OpenQuestionAnswers_TestId",
                table: "OpenQuestionAnswers",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedQuestionAnswers_TestId",
                table: "ClosedQuestionAnswers",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClosedQuestionAnswers_Tests_TestId",
                table: "ClosedQuestionAnswers",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpenQuestionAnswers_Tests_TestId",
                table: "OpenQuestionAnswers",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
