using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobSeed.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployerId",
                table: "Job",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "UsersJob",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false),
                    ApplyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageLink = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersJob", x => new { x.UsersId, x.JobId });
                    table.ForeignKey(
                        name: "FK_UsersJob_Job_JobId",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersJob_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Job_EmployerId",
                table: "Job",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersJob_JobId",
                table: "UsersJob",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Users_EmployerId",
                table: "Job",
                column: "EmployerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Job_Users_EmployerId",
                table: "Job");

            migrationBuilder.DropTable(
                name: "UsersJob");

            migrationBuilder.DropIndex(
                name: "IX_Job_EmployerId",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "EmployerId",
                table: "Job");
        }
    }
}
