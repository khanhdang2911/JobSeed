using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSeed.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategory_job : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Job",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Job_CategoryId",
                table: "Job",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Category_CategoryId",
                table: "Job",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Job_Category_CategoryId",
                table: "Job");

            migrationBuilder.DropIndex(
                name: "IX_Job_CategoryId",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Job");
        }
    }
}
