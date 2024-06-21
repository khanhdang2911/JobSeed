using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSeed.Migrations
{
    /// <inheritdoc />
    public partial class update_userjob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageLink",
                table: "UsersJob",
                newName: "CV");

            migrationBuilder.AddColumn<string>(
                name: "Coverletter",
                table: "UsersJob",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SocialLinkAccount",
                table: "UsersJob",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coverletter",
                table: "UsersJob");

            migrationBuilder.DropColumn(
                name: "SocialLinkAccount",
                table: "UsersJob");

            migrationBuilder.RenameColumn(
                name: "CV",
                table: "UsersJob",
                newName: "ImageLink");
        }
    }
}
