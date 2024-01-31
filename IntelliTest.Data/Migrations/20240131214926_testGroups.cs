using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class testGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClosedQuestions_Tests_TestId",
                table: "ClosedQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_OpenQuestions_Tests_TestId",
                table: "OpenQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_TestId",
                table: "TestResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestResults",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "QuestionsOrder",
                table: "Tests");

            migrationBuilder.RenameColumn(
                name: "TestId",
                table: "OpenQuestions",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_OpenQuestions_TestId",
                table: "OpenQuestions",
                newName: "IX_OpenQuestions_GroupId");

            migrationBuilder.RenameColumn(
                name: "TestId",
                table: "ClosedQuestions",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ClosedQuestions_TestId",
                table: "ClosedQuestions",
                newName: "IX_ClosedQuestions_GroupId");

            migrationBuilder.AlterColumn<float>(
                name: "Score",
                table: "TestResults",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "TestId",
                table: "TestResults",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "TestResults",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<float>(
                name: "MaxScore",
                table: "OpenQuestions",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "Points",
                table: "OpenQuestionAnswers",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<float>(
                name: "MaxScore",
                table: "ClosedQuestions",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestResults",
                table: "TestResults",
                columns: new[] { "GroupId", "StudentId" });

            migrationBuilder.CreateTable(
                name: "TestGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    QuestionsOrder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxScore = table.Column<float>(type: "real", nullable: false),
                    TestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestGroups_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestId",
                table: "TestResults",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestGroups_TestId",
                table: "TestGroups",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClosedQuestions_TestGroups_GroupId",
                table: "ClosedQuestions",
                column: "GroupId",
                principalTable: "TestGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpenQuestions_TestGroups_GroupId",
                table: "OpenQuestions",
                column: "GroupId",
                principalTable: "TestGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_TestGroups_GroupId",
                table: "TestResults",
                column: "GroupId",
                principalTable: "TestGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Tests_TestId",
                table: "TestResults",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClosedQuestions_TestGroups_GroupId",
                table: "ClosedQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_OpenQuestions_TestGroups_GroupId",
                table: "OpenQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_TestGroups_GroupId",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_TestId",
                table: "TestResults");

            migrationBuilder.DropTable(
                name: "TestGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestResults",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_TestId",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "TestResults");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "OpenQuestions",
                newName: "TestId");

            migrationBuilder.RenameIndex(
                name: "IX_OpenQuestions_GroupId",
                table: "OpenQuestions",
                newName: "IX_OpenQuestions_TestId");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "ClosedQuestions",
                newName: "TestId");

            migrationBuilder.RenameIndex(
                name: "IX_ClosedQuestions_GroupId",
                table: "ClosedQuestions",
                newName: "IX_ClosedQuestions_TestId");

            migrationBuilder.AddColumn<string>(
                name: "QuestionsOrder",
                table: "Tests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "TestId",
                table: "TestResults",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Score",
                table: "TestResults",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<int>(
                name: "MaxScore",
                table: "OpenQuestions",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "Points",
                table: "OpenQuestionAnswers",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<int>(
                name: "MaxScore",
                table: "ClosedQuestions",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestResults",
                table: "TestResults",
                columns: new[] { "TestId", "StudentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ClosedQuestions_Tests_TestId",
                table: "ClosedQuestions",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpenQuestions_Tests_TestId",
                table: "OpenQuestions",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Tests_TestId",
                table: "TestResults",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
