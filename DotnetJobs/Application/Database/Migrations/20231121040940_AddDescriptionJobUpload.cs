using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetJobs.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionJobUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "job_uploads",
                type: "text",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "job_uploads");
        }
    }
}
