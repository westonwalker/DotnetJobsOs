using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetJobs.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemovePlatformLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "platform_link",
                table: "job_uploads");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "platform_link",
                table: "job_uploads",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
