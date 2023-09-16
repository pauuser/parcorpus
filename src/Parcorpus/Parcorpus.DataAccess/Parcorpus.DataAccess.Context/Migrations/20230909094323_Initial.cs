using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Parcorpus.DataAccess.Context.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    GenreId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.GenreId);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShortName = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageId);
                });

            migrationBuilder.CreateTable(
                name: "MorphologicalAnnotations",
                columns: table => new
                {
                    MorphId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartOfSpeech = table.Column<string>(type: "text", nullable: true),
                    GrammaticalCategory = table.Column<string>(type: "text", nullable: true),
                    Anim = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MorphologicalAnnotations", x => x.MorphId);
                });

            migrationBuilder.CreateTable(
                name: "LanguagePairs",
                columns: table => new
                {
                    LanguagePairId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FromLanguage = table.Column<int>(type: "integer", nullable: false),
                    ToLanguage = table.Column<int>(type: "integer", nullable: false),
                    FromLanguageNavigationLanguageId = table.Column<int>(type: "integer", nullable: true),
                    ToLanguageNavigationLanguageId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguagePairs", x => x.LanguagePairId);
                    table.ForeignKey(
                        name: "FK_LanguagePairs_Languages_FromLanguageNavigationLanguageId",
                        column: x => x.FromLanguageNavigationLanguageId,
                        principalTable: "Languages",
                        principalColumn: "LanguageId");
                    table.ForeignKey(
                        name: "FK_LanguagePairs_Languages_ToLanguageNavigationLanguageId",
                        column: x => x.ToLanguageNavigationLanguageId,
                        principalTable: "Languages",
                        principalColumn: "LanguageId");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Surname = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<int>(type: "integer", nullable: true),
                    NativeLanguage = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Countries_Country",
                        column: x => x.Country,
                        principalTable: "Countries",
                        principalColumn: "CountryId");
                    table.ForeignKey(
                        name: "FK_Users_Languages_NativeLanguage",
                        column: x => x.NativeLanguage,
                        principalTable: "Languages",
                        principalColumn: "LanguageId");
                });

            migrationBuilder.CreateTable(
                name: "Texts",
                columns: table => new
                {
                    TextId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetaAnnotation = table.Column<int>(type: "integer", nullable: true),
                    LanguagePair = table.Column<int>(type: "integer", nullable: true),
                    AddedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    AddedByNavigationUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LanguagePairNavigationLanguagePairId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Texts", x => x.TextId);
                    table.ForeignKey(
                        name: "FK_Texts_LanguagePairs_LanguagePairNavigationLanguagePairId",
                        column: x => x.LanguagePairNavigationLanguagePairId,
                        principalTable: "LanguagePairs",
                        principalColumn: "LanguagePairId");
                    table.ForeignKey(
                        name: "FK_Texts_Users_AddedByNavigationUserId",
                        column: x => x.AddedByNavigationUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "MetaAnnotations",
                columns: table => new
                {
                    MetaId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Author = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    CreationYear = table.Column<int>(type: "integer", nullable: true),
                    AddDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaAnnotations", x => x.MetaId);
                    table.ForeignKey(
                        name: "FK_MetaAnnotations_Texts_MetaId",
                        column: x => x.MetaId,
                        principalTable: "Texts",
                        principalColumn: "TextId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sentences",
                columns: table => new
                {
                    SentenceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourceText = table.Column<string>(type: "text", nullable: true),
                    AlignedTranslation = table.Column<string>(type: "text", nullable: true),
                    SourceTextId = table.Column<int>(type: "integer", nullable: true),
                    TextId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sentences", x => x.SentenceId);
                    table.ForeignKey(
                        name: "FK_Sentences_Texts_TextId",
                        column: x => x.TextId,
                        principalTable: "Texts",
                        principalColumn: "TextId");
                });

            migrationBuilder.CreateTable(
                name: "MetaGenres",
                columns: table => new
                {
                    MgId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetaId = table.Column<int>(type: "integer", nullable: true),
                    GenreId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaGenres", x => x.MgId);
                    table.ForeignKey(
                        name: "FK_MetaGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "GenreId");
                    table.ForeignKey(
                        name: "FK_MetaGenres_MetaAnnotations_MetaId",
                        column: x => x.MetaId,
                        principalTable: "MetaAnnotations",
                        principalColumn: "MetaId");
                });

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    WordId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourceWord = table.Column<string>(type: "text", nullable: true),
                    AlignedWord = table.Column<string>(type: "text", nullable: true),
                    Sentence = table.Column<int>(type: "integer", nullable: true),
                    MorphologicalAnnotation = table.Column<int>(type: "integer", nullable: true),
                    MorphologicalAnnotationNavigationMorphId = table.Column<int>(type: "integer", nullable: true),
                    SentenceNavigationSentenceId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.WordId);
                    table.ForeignKey(
                        name: "FK_Words_MorphologicalAnnotations_MorphologicalAnnotationNavig~",
                        column: x => x.MorphologicalAnnotationNavigationMorphId,
                        principalTable: "MorphologicalAnnotations",
                        principalColumn: "MorphId");
                    table.ForeignKey(
                        name: "FK_Words_Sentences_SentenceNavigationSentenceId",
                        column: x => x.SentenceNavigationSentenceId,
                        principalTable: "Sentences",
                        principalColumn: "SentenceId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LanguagePairs_FromLanguageNavigationLanguageId",
                table: "LanguagePairs",
                column: "FromLanguageNavigationLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguagePairs_ToLanguageNavigationLanguageId",
                table: "LanguagePairs",
                column: "ToLanguageNavigationLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaGenres_GenreId",
                table: "MetaGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaGenres_MetaId",
                table: "MetaGenres",
                column: "MetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Sentences_TextId",
                table: "Sentences",
                column: "TextId");

            migrationBuilder.CreateIndex(
                name: "IX_Texts_AddedByNavigationUserId",
                table: "Texts",
                column: "AddedByNavigationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Texts_LanguagePairNavigationLanguagePairId",
                table: "Texts",
                column: "LanguagePairNavigationLanguagePairId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Country",
                table: "Users",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NativeLanguage",
                table: "Users",
                column: "NativeLanguage");

            migrationBuilder.CreateIndex(
                name: "IX_Words_MorphologicalAnnotationNavigationMorphId",
                table: "Words",
                column: "MorphologicalAnnotationNavigationMorphId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_SentenceNavigationSentenceId",
                table: "Words",
                column: "SentenceNavigationSentenceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetaGenres");

            migrationBuilder.DropTable(
                name: "Words");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "MetaAnnotations");

            migrationBuilder.DropTable(
                name: "MorphologicalAnnotations");

            migrationBuilder.DropTable(
                name: "Sentences");

            migrationBuilder.DropTable(
                name: "Texts");

            migrationBuilder.DropTable(
                name: "LanguagePairs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
