using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeachingAssignmentUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeachingAssignments_TeacherId_SubjectId",
                table: "TeachingAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_TeachingAssignments_TeacherId_SubjectId_GroupId",
                table: "TeachingAssignments",
                columns: new[] { "TeacherId", "SubjectId", "GroupId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeachingAssignments_TeacherId_SubjectId_GroupId",
                table: "TeachingAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_TeachingAssignments_TeacherId_SubjectId",
                table: "TeachingAssignments",
                columns: new[] { "TeacherId", "SubjectId" },
                unique: true);
        }
    }
}
