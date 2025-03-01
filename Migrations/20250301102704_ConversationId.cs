using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutorAI.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConversationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConversationId",
                table: "UserMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "UserMessages");
        }
    }
}
