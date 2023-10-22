using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class questionsEquationsAndPhotos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "OpenQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsEquation",
                table: "OpenQuestions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "ClosedQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsEquation",
                table: "ClosedQuestions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "OpenQuestions");

            migrationBuilder.DropColumn(
                name: "IsEquation",
                table: "OpenQuestions");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "ClosedQuestions");

            migrationBuilder.DropColumn(
                name: "IsEquation",
                table: "ClosedQuestions");
        }
    }
}
