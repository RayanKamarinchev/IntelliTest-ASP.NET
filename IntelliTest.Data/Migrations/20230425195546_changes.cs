using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassTest_Classes_ClassId",
                table: "ClassTest");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassTest_Tests_TestId",
                table: "ClassTest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassTest",
                table: "ClassTest");

            migrationBuilder.RenameTable(
                name: "ClassTest",
                newName: "ClassTests");

            migrationBuilder.RenameIndex(
                name: "IX_ClassTest_ClassId",
                table: "ClassTests",
                newName: "IX_ClassTests_ClassId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassTests",
                table: "ClassTests",
                columns: new[] { "TestId", "ClassId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ClassTests_Classes_ClassId",
                table: "ClassTests",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassTests_Tests_TestId",
                table: "ClassTests",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassTests_Classes_ClassId",
                table: "ClassTests");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassTests_Tests_TestId",
                table: "ClassTests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassTests",
                table: "ClassTests");

            migrationBuilder.RenameTable(
                name: "ClassTests",
                newName: "ClassTest");

            migrationBuilder.RenameIndex(
                name: "IX_ClassTests_ClassId",
                table: "ClassTest",
                newName: "IX_ClassTest_ClassId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassTest",
                table: "ClassTest",
                columns: new[] { "TestId", "ClassId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ClassTest_Classes_ClassId",
                table: "ClassTest",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassTest_Tests_TestId",
                table: "ClassTest",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
