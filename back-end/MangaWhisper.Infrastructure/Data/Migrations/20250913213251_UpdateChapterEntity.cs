using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MangaWhisper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChapterEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PublishedDate",
                table: "Chapters",
                newName: "ExtractedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExtractedAt",
                table: "Chapters",
                newName: "PublishedDate");
        }
    }
}
