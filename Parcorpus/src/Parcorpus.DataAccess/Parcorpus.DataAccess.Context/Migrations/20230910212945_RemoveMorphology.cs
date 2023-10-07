using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Parcorpus.DataAccess.Context.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMorphology : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Words_MorphologicalAnnotations_MorphologicalAnnotation",
                table: "Words");

            migrationBuilder.DropTable(
                name: "MorphologicalAnnotations");

            migrationBuilder.DropIndex(
                name: "IX_Words_MorphologicalAnnotation",
                table: "Words");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MorphologicalAnnotations",
                columns: table => new
                {
                    MorphId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Anim = table.Column<string>(type: "text", nullable: false),
                    GrammaticalCategory = table.Column<string>(type: "text", nullable: false),
                    PartOfSpeech = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MorphologicalAnnotations", x => x.MorphId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Words_MorphologicalAnnotation",
                table: "Words",
                column: "MorphologicalAnnotation");

            migrationBuilder.AddForeignKey(
                name: "FK_Words_MorphologicalAnnotations_MorphologicalAnnotation",
                table: "Words",
                column: "MorphologicalAnnotation",
                principalTable: "MorphologicalAnnotations",
                principalColumn: "MorphId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
