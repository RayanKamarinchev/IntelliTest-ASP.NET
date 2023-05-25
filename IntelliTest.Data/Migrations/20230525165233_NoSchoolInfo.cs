using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class NoSchoolInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomUser_AspNetUsers_UserId",
                table: "RoomUser");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomUser_Rooms_RoomId",
                table: "RoomUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomUser",
                table: "RoomUser");

            migrationBuilder.RenameTable(
                name: "RoomUser",
                newName: "RoomUsers");

            migrationBuilder.RenameIndex(
                name: "IX_RoomUser_UserId",
                table: "RoomUsers",
                newName: "IX_RoomUsers_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomUsers",
                table: "RoomUsers",
                columns: new[] { "RoomId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RoomUsers_AspNetUsers_UserId",
                table: "RoomUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomUsers_Rooms_RoomId",
                table: "RoomUsers",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomUsers_AspNetUsers_UserId",
                table: "RoomUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomUsers_Rooms_RoomId",
                table: "RoomUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomUsers",
                table: "RoomUsers");

            migrationBuilder.RenameTable(
                name: "RoomUsers",
                newName: "RoomUser");

            migrationBuilder.RenameIndex(
                name: "IX_RoomUsers_UserId",
                table: "RoomUser",
                newName: "IX_RoomUser_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomUser",
                table: "RoomUser",
                columns: new[] { "RoomId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RoomUser_AspNetUsers_UserId",
                table: "RoomUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomUser_Rooms_RoomId",
                table: "RoomUser",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
