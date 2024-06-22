using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSeed.Migrations
{
    /// <inheritdoc />
    public partial class AdState_UsersJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "State",
                table: "UsersJob",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "UsersJob");
        }
    }
}
