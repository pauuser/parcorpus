using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parcorpus.DataAccess.Context.Migrations
{
    /// <inheritdoc />
    public partial class InitialAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sentences_Texts_TextId",
                table: "Sentences");

            migrationBuilder.RenameColumn(
                name: "TextId",
                table: "Sentences",
                newName: "TextNavigationTextId");

            migrationBuilder.RenameIndex(
                name: "IX_Sentences_TextId",
                table: "Sentences",
                newName: "IX_Sentences_TextNavigationTextId");

            migrationBuilder.AlterColumn<int>(
                name: "MetaId",
                table: "MetaAnnotations",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Sentences_Texts_TextNavigationTextId",
                table: "Sentences",
                column: "TextNavigationTextId",
                principalTable: "Texts",
                principalColumn: "TextId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sentences_Texts_TextNavigationTextId",
                table: "Sentences");

            migrationBuilder.RenameColumn(
                name: "TextNavigationTextId",
                table: "Sentences",
                newName: "TextId");

            migrationBuilder.RenameIndex(
                name: "IX_Sentences_TextNavigationTextId",
                table: "Sentences",
                newName: "IX_Sentences_TextId");

            migrationBuilder.AlterColumn<int>(
                name: "MetaId",
                table: "MetaAnnotations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Sentences_Texts_TextId",
                table: "Sentences",
                column: "TextId",
                principalTable: "Texts",
                principalColumn: "TextId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
