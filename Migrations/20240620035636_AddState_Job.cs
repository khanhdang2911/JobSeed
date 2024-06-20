using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSeed.Migrations
{
    /// <inheritdoc />
    public partial class AddState_Job : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "State",
                table: "Job",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Job");
        }
    }
}
