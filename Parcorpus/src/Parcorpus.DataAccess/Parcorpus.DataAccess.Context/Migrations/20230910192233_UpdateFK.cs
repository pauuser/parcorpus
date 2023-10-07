using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Parcorpus.DataAccess.Context.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LanguagePairs_Languages_FromLanguageNavigationLanguageId",
                table: "LanguagePairs");

            migrationBuilder.DropForeignKey(
                name: "FK_LanguagePairs_Languages_ToLanguageNavigationLanguageId",
                table: "LanguagePairs");

            migrationBuilder.DropForeignKey(
                name: "FK_Sentences_Texts_TextNavigationTextId",
                table: "Sentences");

            migrationBuilder.DropForeignKey(
                name: "FK_Texts_LanguagePairs_LanguagePairNavigationLanguagePairId",
                table: "Texts");

            migrationBuilder.DropForeignKey(
                name: "FK_Texts_Users_AddedByNavigationUserId",
                table: "Texts");

            migrationBuilder.DropForeignKey(
                name: "FK_Words_MorphologicalAnnotations_MorphologicalAnnotationNavig~",
                table: "Words");

            migrationBuilder.DropForeignKey(
                name: "FK_Words_Sentences_SentenceNavigationSentenceId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_MorphologicalAnnotationNavigationMorphId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_SentenceNavigationSentenceId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Texts_AddedByNavigationUserId",
                table: "Texts");

            migrationBuilder.DropIndex(
                name: "IX_Texts_LanguagePairNavigationLanguagePairId",
                table: "Texts");

            migrationBuilder.DropIndex(
                name: "IX_Sentences_TextNavigationTextId",
                table: "Sentences");

            migrationBuilder.DropIndex(
                name: "IX_LanguagePairs_FromLanguageNavigationLanguageId",
                table: "LanguagePairs");

            migrationBuilder.DropIndex(
                name: "IX_LanguagePairs_ToLanguageNavigationLanguageId",
                table: "LanguagePairs");

            migrationBuilder.DropColumn(
                name: "MorphologicalAnnotationNavigationMorphId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "SentenceNavigationSentenceId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "AddedByNavigationUserId",
                table: "Texts");

            migrationBuilder.DropColumn(
                name: "LanguagePairNavigationLanguagePairId",
                table: "Texts");

            migrationBuilder.DropColumn(
                name: "TextNavigationTextId",
                table: "Sentences");

            migrationBuilder.DropColumn(
                name: "FromLanguageNavigationLanguageId",
                table: "LanguagePairs");

            migrationBuilder.DropColumn(
                name: "ToLanguageNavigationLanguageId",
                table: "LanguagePairs");

            migrationBuilder.AlterColumn<int>(
                name: "MetaId",
                table: "MetaAnnotations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Words_MorphologicalAnnotation",
                table: "Words",
                column: "MorphologicalAnnotation");

            migrationBuilder.CreateIndex(
                name: "IX_Words_Sentence",
                table: "Words",
                column: "Sentence");

            migrationBuilder.CreateIndex(
                name: "IX_Texts_AddedBy",
                table: "Texts",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Texts_LanguagePair",
                table: "Texts",
                column: "LanguagePair");

            migrationBuilder.CreateIndex(
                name: "IX_Sentences_SourceTextId",
                table: "Sentences",
                column: "SourceTextId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguagePairs_FromLanguage",
                table: "LanguagePairs",
                column: "FromLanguage");

            migrationBuilder.CreateIndex(
                name: "IX_LanguagePairs_ToLanguage",
                table: "LanguagePairs",
                column: "ToLanguage");

            migrationBuilder.AddForeignKey(
                name: "FK_LanguagePairs_Languages_FromLanguage",
                table: "LanguagePairs",
                column: "FromLanguage",
                principalTable: "Languages",
                principalColumn: "LanguageId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LanguagePairs_Languages_ToLanguage",
                table: "LanguagePairs",
                column: "ToLanguage",
                principalTable: "Languages",
                principalColumn: "LanguageId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sentences_Texts_SourceTextId",
                table: "Sentences",
                column: "SourceTextId",
                principalTable: "Texts",
                principalColumn: "TextId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Texts_LanguagePairs_LanguagePair",
                table: "Texts",
                column: "LanguagePair",
                principalTable: "LanguagePairs",
                principalColumn: "LanguagePairId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Texts_Users_AddedBy",
                table: "Texts",
                column: "AddedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Words_MorphologicalAnnotations_MorphologicalAnnotation",
                table: "Words",
                column: "MorphologicalAnnotation",
                principalTable: "MorphologicalAnnotations",
                principalColumn: "MorphId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Sentences_Sentence",
                table: "Words",
                column: "Sentence",
                principalTable: "Sentences",
                principalColumn: "SentenceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LanguagePairs_Languages_FromLanguage",
                table: "LanguagePairs");

            migrationBuilder.DropForeignKey(
                name: "FK_LanguagePairs_Languages_ToLanguage",
                table: "LanguagePairs");

            migrationBuilder.DropForeignKey(
                name: "FK_Sentences_Texts_SourceTextId",
                table: "Sentences");

            migrationBuilder.DropForeignKey(
                name: "FK_Texts_LanguagePairs_LanguagePair",
                table: "Texts");

            migrationBuilder.DropForeignKey(
                name: "FK_Texts_Users_AddedBy",
                table: "Texts");

            migrationBuilder.DropForeignKey(
                name: "FK_Words_MorphologicalAnnotations_MorphologicalAnnotation",
                table: "Words");

            migrationBuilder.DropForeignKey(
                name: "FK_Words_Sentences_Sentence",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_MorphologicalAnnotation",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_Sentence",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Texts_AddedBy",
                table: "Texts");

            migrationBuilder.DropIndex(
                name: "IX_Texts_LanguagePair",
                table: "Texts");

            migrationBuilder.DropIndex(
                name: "IX_Sentences_SourceTextId",
                table: "Sentences");

            migrationBuilder.DropIndex(
                name: "IX_LanguagePairs_FromLanguage",
                table: "LanguagePairs");

            migrationBuilder.DropIndex(
                name: "IX_LanguagePairs_ToLanguage",
                table: "LanguagePairs");

            migrationBuilder.AddColumn<int>(
                name: "MorphologicalAnnotationNavigationMorphId",
                table: "Words",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SentenceNavigationSentenceId",
                table: "Words",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AddedByNavigationUserId",
                table: "Texts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "LanguagePairNavigationLanguagePairId",
                table: "Texts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TextNavigationTextId",
                table: "Sentences",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "MetaId",
                table: "MetaAnnotations",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "FromLanguageNavigationLanguageId",
                table: "LanguagePairs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToLanguageNavigationLanguageId",
                table: "LanguagePairs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Words_MorphologicalAnnotationNavigationMorphId",
                table: "Words",
                column: "MorphologicalAnnotationNavigationMorphId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_SentenceNavigationSentenceId",
                table: "Words",
                column: "SentenceNavigationSentenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Texts_AddedByNavigationUserId",
                table: "Texts",
                column: "AddedByNavigationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Texts_LanguagePairNavigationLanguagePairId",
                table: "Texts",
                column: "LanguagePairNavigationLanguagePairId");

            migrationBuilder.CreateIndex(
                name: "IX_Sentences_TextNavigationTextId",
                table: "Sentences",
                column: "TextNavigationTextId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguagePairs_FromLanguageNavigationLanguageId",
                table: "LanguagePairs",
                column: "FromLanguageNavigationLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguagePairs_ToLanguageNavigationLanguageId",
                table: "LanguagePairs",
                column: "ToLanguageNavigationLanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_LanguagePairs_Languages_FromLanguageNavigationLanguageId",
                table: "LanguagePairs",
                column: "FromLanguageNavigationLanguageId",
                principalTable: "Languages",
                principalColumn: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_LanguagePairs_Languages_ToLanguageNavigationLanguageId",
                table: "LanguagePairs",
                column: "ToLanguageNavigationLanguageId",
                principalTable: "Languages",
                principalColumn: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sentences_Texts_TextNavigationTextId",
                table: "Sentences",
                column: "TextNavigationTextId",
                principalTable: "Texts",
                principalColumn: "TextId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Texts_LanguagePairs_LanguagePairNavigationLanguagePairId",
                table: "Texts",
                column: "LanguagePairNavigationLanguagePairId",
                principalTable: "LanguagePairs",
                principalColumn: "LanguagePairId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Texts_Users_AddedByNavigationUserId",
                table: "Texts",
                column: "AddedByNavigationUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Words_MorphologicalAnnotations_MorphologicalAnnotationNavig~",
                table: "Words",
                column: "MorphologicalAnnotationNavigationMorphId",
                principalTable: "MorphologicalAnnotations",
                principalColumn: "MorphId");

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Sentences_SentenceNavigationSentenceId",
                table: "Words",
                column: "SentenceNavigationSentenceId",
                principalTable: "Sentences",
                principalColumn: "SentenceId");
        }
    }
}
