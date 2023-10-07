using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parcorpus.DataAccess.Context.Migrations
{
    /// <inheritdoc />
    public partial class FixWordDbModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MorphologicalAnnotation",
                table: "Words");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MorphologicalAnnotation",
                table: "Words",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
