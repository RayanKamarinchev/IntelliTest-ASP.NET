using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class removedreadersCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Read_AspNetUsers_UserId",
                table: "Read");

            migrationBuilder.DropForeignKey(
                name: "FK_Read_Lessons_LessonId",
                table: "Read");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Read",
                table: "Read");

            migrationBuilder.DropColumn(
                name: "Readers",
                table: "Lessons");

            migrationBuilder.RenameTable(
                name: "Read",
                newName: "Reads");

            migrationBuilder.RenameIndex(
                name: "IX_Read_UserId",
                table: "Reads",
                newName: "IX_Reads_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reads",
                table: "Reads",
                columns: new[] { "LessonId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Reads_AspNetUsers_UserId",
                table: "Reads",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reads_Lessons_LessonId",
                table: "Reads",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reads_AspNetUsers_UserId",
                table: "Reads");

            migrationBuilder.DropForeignKey(
                name: "FK_Reads_Lessons_LessonId",
                table: "Reads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reads",
                table: "Reads");

            migrationBuilder.RenameTable(
                name: "Reads",
                newName: "Read");

            migrationBuilder.RenameIndex(
                name: "IX_Reads_UserId",
                table: "Read",
                newName: "IX_Read_UserId");

            migrationBuilder.AddColumn<int>(
                name: "Readers",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Read",
                table: "Read",
                columns: new[] { "LessonId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Read_AspNetUsers_UserId",
                table: "Read",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Read_Lessons_LessonId",
                table: "Read",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
