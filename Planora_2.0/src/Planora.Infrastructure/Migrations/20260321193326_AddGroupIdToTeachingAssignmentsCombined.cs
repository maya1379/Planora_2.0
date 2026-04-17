using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupIdToTeachingAssignmentsCombined : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "TeachingAssignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TeachingAssignments_GroupId",
                table: "TeachingAssignments",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeachingAssignments_Groups_GroupId",
                table: "TeachingAssignments",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeachingAssignments_Groups_GroupId",
                table: "TeachingAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeachingAssignments_GroupId",
                table: "TeachingAssignments");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "TeachingAssignments");
        }
    }
}
