using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSeed.Migrations
{
    /// <inheritdoc />
    public partial class AddImageLink_JobTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobDetail_JobId",
                table: "JobDetail");

            migrationBuilder.AddColumn<string>(
                name: "ImageLink",
                table: "Job",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobDetail_JobId",
                table: "JobDetail",
                column: "JobId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobDetail_JobId",
                table: "JobDetail");

            migrationBuilder.DropColumn(
                name: "ImageLink",
                table: "Job");

            migrationBuilder.CreateIndex(
                name: "IX_JobDetail_JobId",
                table: "JobDetail",
                column: "JobId");
        }
    }
}
