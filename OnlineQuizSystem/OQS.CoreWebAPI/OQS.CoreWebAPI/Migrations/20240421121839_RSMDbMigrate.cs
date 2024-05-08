using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OQS.CoreWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class RSMDbMigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionResults",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    PseudoDictionaryChoicesResults = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewNeededAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewNeededResult = table.Column<int>(type: "int", nullable: true),
                    TrueFalseAnswerResult = table.Column<int>(type: "int", nullable: true),
                    WrittenAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WrittenAnswerResult = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionResults", x => new { x.UserId, x.QuestionId });
                });

            migrationBuilder.CreateTable(
                name: "QuizResultBodies",
                columns: table => new
                {
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizResultBodies", x => new { x.UserId, x.QuizId });
                });

            migrationBuilder.CreateTable(
                name: "QuizResultHeaders",
                columns: table => new
                {
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionTime = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    ReviewPending = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizResultHeaders", x => new { x.UserId, x.QuizId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionResults");

            migrationBuilder.DropTable(
                name: "QuizResultBodies");

            migrationBuilder.DropTable(
                name: "QuizResultHeaders");
        }
    }
}
