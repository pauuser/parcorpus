using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parcorpus.DataAccess.Context.Migrations
{
    /// <inheritdoc />
    public partial class FixFKForMetaAnnotations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetaAnnotations_Texts_MetaId",
                table: "MetaAnnotations");

            migrationBuilder.CreateIndex(
                name: "IX_Texts_MetaAnnotation",
                table: "Texts",
                column: "MetaAnnotation",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Texts_MetaAnnotations_MetaAnnotation",
                table: "Texts",
                column: "MetaAnnotation",
                principalTable: "MetaAnnotations",
                principalColumn: "MetaId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Texts_MetaAnnotations_MetaAnnotation",
                table: "Texts");

            migrationBuilder.DropIndex(
                name: "IX_Texts_MetaAnnotation",
                table: "Texts");

            migrationBuilder.AddForeignKey(
                name: "FK_MetaAnnotations_Texts_MetaId",
                table: "MetaAnnotations",
                column: "MetaId",
                principalTable: "Texts",
                principalColumn: "TextId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
