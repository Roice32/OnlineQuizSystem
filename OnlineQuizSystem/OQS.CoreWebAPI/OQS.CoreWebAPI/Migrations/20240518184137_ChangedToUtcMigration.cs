using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OQS.CoreWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangedToUtcMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Quizzes",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "SubmittedAt",
                table: "QuizResultHeaders",
                newName: "SubmittedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "Quizzes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "SubmittedAtUtc",
                table: "QuizResultHeaders",
                newName: "SubmittedAt");
        }
    }
}
