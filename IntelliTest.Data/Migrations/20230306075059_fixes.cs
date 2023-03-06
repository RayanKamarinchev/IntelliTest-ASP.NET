using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntelliTest.Data.Migrations
{
    public partial class fixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Class_Teacher_TeacherId",
                table: "Class");

            migrationBuilder.DropForeignKey(
                name: "FK_ClosedQuestionAnswer_ClosedQuestions_QuestionId",
                table: "ClosedQuestionAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_ClosedQuestionAnswer_Student_StudentId",
                table: "ClosedQuestionAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_OpenQuestionAnswer_OpenQuestions_QuestionId",
                table: "OpenQuestionAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_OpenQuestionAnswer_Student_StudentId",
                table: "OpenQuestionAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_AspNetUsers_UserId",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClass_Class_ClassId",
                table: "StudentClass");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClass_Student_StudentId",
                table: "StudentClass");

            migrationBuilder.DropForeignKey(
                name: "FK_Teacher_AspNetUsers_UserId",
                table: "Teacher");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Teacher_CreatorId",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestStudent_Student_StudentId",
                table: "TestStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_TestStudent_Tests_TestId",
                table: "TestStudent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestStudent",
                table: "TestStudent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teacher",
                table: "Teacher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentClass",
                table: "StudentClass");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Student",
                table: "Student");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OpenQuestionAnswer",
                table: "OpenQuestionAnswer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClosedQuestionAnswer",
                table: "ClosedQuestionAnswer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Class",
                table: "Class");

            migrationBuilder.RenameTable(
                name: "TestStudent",
                newName: "TestStudents");

            migrationBuilder.RenameTable(
                name: "Teacher",
                newName: "Teachers");

            migrationBuilder.RenameTable(
                name: "StudentClass",
                newName: "StudentClasses");

            migrationBuilder.RenameTable(
                name: "Student",
                newName: "Students");

            migrationBuilder.RenameTable(
                name: "OpenQuestionAnswer",
                newName: "OpenQuestionAnswers");

            migrationBuilder.RenameTable(
                name: "ClosedQuestionAnswer",
                newName: "ClosedQuestionAnswers");

            migrationBuilder.RenameTable(
                name: "Class",
                newName: "Classes");

            migrationBuilder.RenameIndex(
                name: "IX_TestStudent_StudentId",
                table: "TestStudents",
                newName: "IX_TestStudents_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Teacher_UserId",
                table: "Teachers",
                newName: "IX_Teachers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentClass_StudentId",
                table: "StudentClasses",
                newName: "IX_StudentClasses_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Student_UserId",
                table: "Students",
                newName: "IX_Students_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_OpenQuestionAnswer_StudentId",
                table: "OpenQuestionAnswers",
                newName: "IX_OpenQuestionAnswers_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_OpenQuestionAnswer_QuestionId",
                table: "OpenQuestionAnswers",
                newName: "IX_OpenQuestionAnswers_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_ClosedQuestionAnswer_StudentId",
                table: "ClosedQuestionAnswers",
                newName: "IX_ClosedQuestionAnswers_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_ClosedQuestionAnswer_QuestionId",
                table: "ClosedQuestionAnswers",
                newName: "IX_ClosedQuestionAnswers_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Class_TeacherId",
                table: "Classes",
                newName: "IX_Classes_TeacherId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestStudents",
                table: "TestStudents",
                columns: new[] { "TestId", "StudentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentClasses",
                table: "StudentClasses",
                columns: new[] { "ClassId", "StudentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OpenQuestionAnswers",
                table: "OpenQuestionAnswers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClosedQuestionAnswers",
                table: "ClosedQuestionAnswers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classes",
                table: "Classes",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatorId",
                value: 1);

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Teachers_TeacherId",
                table: "Classes",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClosedQuestionAnswers_ClosedQuestions_QuestionId",
                table: "ClosedQuestionAnswers",
                column: "QuestionId",
                principalTable: "ClosedQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClosedQuestionAnswers_Students_StudentId",
                table: "ClosedQuestionAnswers",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpenQuestionAnswers_OpenQuestions_QuestionId",
                table: "OpenQuestionAnswers",
                column: "QuestionId",
                principalTable: "OpenQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpenQuestionAnswers_Students_StudentId",
                table: "OpenQuestionAnswers",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_Classes_ClassId",
                table: "StudentClasses",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_Students_StudentId",
                table: "StudentClasses",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AspNetUsers_UserId",
                table: "Students",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_AspNetUsers_UserId",
                table: "Teachers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Teachers_CreatorId",
                table: "Tests",
                column: "CreatorId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestStudents_Students_StudentId",
                table: "TestStudents",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestStudents_Tests_TestId",
                table: "TestStudents",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "FK_OpenQuestionAnswers_OpenQuestions_QuestionId",
                table: "OpenQuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_OpenQuestionAnswers_Students_StudentId",
                table: "OpenQuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_Classes_ClassId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_Students_StudentId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AspNetUsers_UserId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_AspNetUsers_UserId",
                table: "Teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Teachers_CreatorId",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "FK_TestStudents_Students_StudentId",
                table: "TestStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_TestStudents_Tests_TestId",
                table: "TestStudents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestStudents",
                table: "TestStudents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teachers",
                table: "Teachers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentClasses",
                table: "StudentClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OpenQuestionAnswers",
                table: "OpenQuestionAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClosedQuestionAnswers",
                table: "ClosedQuestionAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Classes",
                table: "Classes");

            migrationBuilder.RenameTable(
                name: "TestStudents",
                newName: "TestStudent");

            migrationBuilder.RenameTable(
                name: "Teachers",
                newName: "Teacher");

            migrationBuilder.RenameTable(
                name: "Students",
                newName: "Student");

            migrationBuilder.RenameTable(
                name: "StudentClasses",
                newName: "StudentClass");

            migrationBuilder.RenameTable(
                name: "OpenQuestionAnswers",
                newName: "OpenQuestionAnswer");

            migrationBuilder.RenameTable(
                name: "ClosedQuestionAnswers",
                newName: "ClosedQuestionAnswer");

            migrationBuilder.RenameTable(
                name: "Classes",
                newName: "Class");

            migrationBuilder.RenameIndex(
                name: "IX_TestStudents_StudentId",
                table: "TestStudent",
                newName: "IX_TestStudent_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Teachers_UserId",
                table: "Teacher",
                newName: "IX_Teacher_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Students_UserId",
                table: "Student",
                newName: "IX_Student_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentClasses_StudentId",
                table: "StudentClass",
                newName: "IX_StudentClass_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_OpenQuestionAnswers_StudentId",
                table: "OpenQuestionAnswer",
                newName: "IX_OpenQuestionAnswer_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_OpenQuestionAnswers_QuestionId",
                table: "OpenQuestionAnswer",
                newName: "IX_OpenQuestionAnswer_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_ClosedQuestionAnswers_StudentId",
                table: "ClosedQuestionAnswer",
                newName: "IX_ClosedQuestionAnswer_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_ClosedQuestionAnswers_QuestionId",
                table: "ClosedQuestionAnswer",
                newName: "IX_ClosedQuestionAnswer_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Classes_TeacherId",
                table: "Class",
                newName: "IX_Class_TeacherId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestStudent",
                table: "TestStudent",
                columns: new[] { "TestId", "StudentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teacher",
                table: "Teacher",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Student",
                table: "Student",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentClass",
                table: "StudentClass",
                columns: new[] { "ClassId", "StudentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OpenQuestionAnswer",
                table: "OpenQuestionAnswer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClosedQuestionAnswer",
                table: "ClosedQuestionAnswer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Class",
                table: "Class",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Tests",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatorId",
                value: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Class_Teacher_TeacherId",
                table: "Class",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClosedQuestionAnswer_ClosedQuestions_QuestionId",
                table: "ClosedQuestionAnswer",
                column: "QuestionId",
                principalTable: "ClosedQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClosedQuestionAnswer_Student_StudentId",
                table: "ClosedQuestionAnswer",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpenQuestionAnswer_OpenQuestions_QuestionId",
                table: "OpenQuestionAnswer",
                column: "QuestionId",
                principalTable: "OpenQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpenQuestionAnswer_Student_StudentId",
                table: "OpenQuestionAnswer",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_AspNetUsers_UserId",
                table: "Student",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClass_Class_ClassId",
                table: "StudentClass",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClass_Student_StudentId",
                table: "StudentClass",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teacher_AspNetUsers_UserId",
                table: "Teacher",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Teacher_CreatorId",
                table: "Tests",
                column: "CreatorId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestStudent_Student_StudentId",
                table: "TestStudent",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestStudent_Tests_TestId",
                table: "TestStudent",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
