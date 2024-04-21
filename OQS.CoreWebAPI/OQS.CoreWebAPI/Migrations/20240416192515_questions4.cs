using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OQS.CoreWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class questions4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TrueFalseAnswer",
                table: "Questions",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrueFalseAnswer",
                table: "Questions");
        }
    }
}
