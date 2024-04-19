using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OQS.CoreWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class QuestionResultRSMDbMigrate : Migration
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
                    SubmittedAnswers = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswersTypes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionResults", x => new { x.UserId, x.QuestionId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionResults");
        }
    }
}
