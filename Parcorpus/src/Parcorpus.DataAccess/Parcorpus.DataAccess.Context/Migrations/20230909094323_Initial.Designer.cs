﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Parcorpus.DataAccess.Context;

#nullable disable

namespace Parcorpus.DataAccess.Context.Migrations
{
    [DbContext(typeof(ParcorpusDbContext))]
    [Migration("20230909094323_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Parcorpus.DataAccess.Models.CountryDbModel", b =>
                {
                    b.Property<int>("CountryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CountryId"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("CountryId");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.GenreDbModel", b =>
                {
                    b.Property<int?>("GenreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("GenreId"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("GenreId");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.LanguageDbModel", b =>
                {
                    b.Property<int>("LanguageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("LanguageId"));

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<string>("ShortName")
                        .HasColumnType("text");

                    b.HasKey("LanguageId");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.LanguagePairDbModel", b =>
                {
                    b.Property<int?>("LanguagePairId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("LanguagePairId"));

                    b.Property<int>("FromLanguage")
                        .HasColumnType("integer");

                    b.Property<int?>("FromLanguageNavigationLanguageId")
                        .HasColumnType("integer");

                    b.Property<int>("ToLanguage")
                        .HasColumnType("integer");

                    b.Property<int?>("ToLanguageNavigationLanguageId")
                        .HasColumnType("integer");

                    b.HasKey("LanguagePairId");

                    b.HasIndex("FromLanguageNavigationLanguageId");

                    b.HasIndex("ToLanguageNavigationLanguageId");

                    b.ToTable("LanguagePairs");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.MetaAnnotationDbModel", b =>
                {
                    b.Property<int?>("MetaId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("AddDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Author")
                        .HasColumnType("text");

                    b.Property<int?>("CreationYear")
                        .HasColumnType("integer");

                    b.Property<string>("Source")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("MetaId");

                    b.ToTable("MetaAnnotations");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.MetaGenreDbModel", b =>
                {
                    b.Property<int?>("MgId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("MgId"));

                    b.Property<int?>("GenreId")
                        .HasColumnType("integer");

                    b.Property<int?>("MetaId")
                        .HasColumnType("integer");

                    b.HasKey("MgId");

                    b.HasIndex("GenreId");

                    b.HasIndex("MetaId");

                    b.ToTable("MetaGenres");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.MorphologicalAnnotationDbModel", b =>
                {
                    b.Property<int?>("MorphId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("MorphId"));

                    b.Property<string>("Anim")
                        .HasColumnType("text");

                    b.Property<string>("GrammaticalCategory")
                        .HasColumnType("text");

                    b.Property<string>("PartOfSpeech")
                        .HasColumnType("text");

                    b.HasKey("MorphId");

                    b.ToTable("MorphologicalAnnotations");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.SentenceDbModel", b =>
                {
                    b.Property<int?>("SentenceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("SentenceId"));

                    b.Property<string>("AlignedTranslation")
                        .HasColumnType("text");

                    b.Property<string>("SourceText")
                        .HasColumnType("text");

                    b.Property<int?>("SourceTextId")
                        .HasColumnType("integer");

                    b.Property<int?>("TextId")
                        .HasColumnType("integer");

                    b.HasKey("SentenceId");

                    b.HasIndex("TextId");

                    b.ToTable("Sentences");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.TextDbModel", b =>
                {
                    b.Property<int?>("TextId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("TextId"));

                    b.Property<Guid?>("AddedBy")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AddedByNavigationUserId")
                        .HasColumnType("uuid");

                    b.Property<int?>("LanguagePair")
                        .HasColumnType("integer");

                    b.Property<int?>("LanguagePairNavigationLanguagePairId")
                        .HasColumnType("integer");

                    b.Property<int?>("MetaAnnotation")
                        .HasColumnType("integer");

                    b.HasKey("TextId");

                    b.HasIndex("AddedByNavigationUserId");

                    b.HasIndex("LanguagePairNavigationLanguagePairId");

                    b.ToTable("Texts");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.UserDbModel", b =>
                {
                    b.Property<Guid?>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int?>("Country")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("NativeLanguage")
                        .HasColumnType("integer");

                    b.Property<string>("Surname")
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.HasIndex("Country");

                    b.HasIndex("NativeLanguage");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.WordDbModel", b =>
                {
                    b.Property<int?>("WordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("WordId"));

                    b.Property<string>("AlignedWord")
                        .HasColumnType("text");

                    b.Property<int?>("MorphologicalAnnotation")
                        .HasColumnType("integer");

                    b.Property<int?>("MorphologicalAnnotationNavigationMorphId")
                        .HasColumnType("integer");

                    b.Property<int?>("Sentence")
                        .HasColumnType("integer");

                    b.Property<int?>("SentenceNavigationSentenceId")
                        .HasColumnType("integer");

                    b.Property<string>("SourceWord")
                        .HasColumnType("text");

                    b.HasKey("WordId");

                    b.HasIndex("MorphologicalAnnotationNavigationMorphId");

                    b.HasIndex("SentenceNavigationSentenceId");

                    b.ToTable("Words");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.LanguagePairDbModel", b =>
                {
                    b.HasOne("Parcorpus.DataAccess.Models.LanguageDbModel", "FromLanguageNavigation")
                        .WithMany("LanguagePairFromLanguageNavigations")
                        .HasForeignKey("FromLanguageNavigationLanguageId");

                    b.HasOne("Parcorpus.DataAccess.Models.LanguageDbModel", "ToLanguageNavigation")
                        .WithMany("LanguagePairToLanguageNavigations")
                        .HasForeignKey("ToLanguageNavigationLanguageId");

                    b.Navigation("FromLanguageNavigation");

                    b.Navigation("ToLanguageNavigation");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.MetaAnnotationDbModel", b =>
                {
                    b.HasOne("Parcorpus.DataAccess.Models.TextDbModel", "TextNavigation")
                        .WithOne("MetaAnnotationNavigation")
                        .HasForeignKey("Parcorpus.DataAccess.Models.MetaAnnotationDbModel", "MetaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TextNavigation");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.MetaGenreDbModel", b =>
                {
                    b.HasOne("Parcorpus.DataAccess.Models.GenreDbModel", "Genre")
                        .WithMany("MetaGenres")
                        .HasForeignKey("GenreId");

                    b.HasOne("Parcorpus.DataAccess.Models.MetaAnnotationDbModel", "Meta")
                        .WithMany("MetaGenres")
                        .HasForeignKey("MetaId");

                    b.Navigation("Genre");

                    b.Navigation("Meta");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.SentenceDbModel", b =>
                {
                    b.HasOne("Parcorpus.DataAccess.Models.TextDbModel", "Text")
                        .WithMany("Sentences")
                        .HasForeignKey("TextId");

                    b.Navigation("Text");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.TextDbModel", b =>
                {
                    b.HasOne("Parcorpus.DataAccess.Models.UserDbModel", "AddedByNavigation")
                        .WithMany("Texts")
                        .HasForeignKey("AddedByNavigationUserId");

                    b.HasOne("Parcorpus.DataAccess.Models.LanguagePairDbModel", "LanguagePairNavigation")
                        .WithMany("Texts")
                        .HasForeignKey("LanguagePairNavigationLanguagePairId");

                    b.Navigation("AddedByNavigation");

                    b.Navigation("LanguagePairNavigation");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.UserDbModel", b =>
                {
                    b.HasOne("Parcorpus.DataAccess.Models.CountryDbModel", "CountryNavigation")
                        .WithMany("Users")
                        .HasForeignKey("Country");

                    b.HasOne("Parcorpus.DataAccess.Models.LanguageDbModel", "NativeLanguageNavigation")
                        .WithMany("Users")
                        .HasForeignKey("NativeLanguage");

                    b.Navigation("CountryNavigation");

                    b.Navigation("NativeLanguageNavigation");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.WordDbModel", b =>
                {
                    b.HasOne("Parcorpus.DataAccess.Models.MorphologicalAnnotationDbModel", "MorphologicalAnnotationNavigation")
                        .WithMany("Words")
                        .HasForeignKey("MorphologicalAnnotationNavigationMorphId");

                    b.HasOne("Parcorpus.DataAccess.Models.SentenceDbModel", "SentenceNavigation")
                        .WithMany("Words")
                        .HasForeignKey("SentenceNavigationSentenceId");

                    b.Navigation("MorphologicalAnnotationNavigation");

                    b.Navigation("SentenceNavigation");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.CountryDbModel", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.GenreDbModel", b =>
                {
                    b.Navigation("MetaGenres");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.LanguageDbModel", b =>
                {
                    b.Navigation("LanguagePairFromLanguageNavigations");

                    b.Navigation("LanguagePairToLanguageNavigations");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.LanguagePairDbModel", b =>
                {
                    b.Navigation("Texts");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.MetaAnnotationDbModel", b =>
                {
                    b.Navigation("MetaGenres");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.MorphologicalAnnotationDbModel", b =>
                {
                    b.Navigation("Words");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.SentenceDbModel", b =>
                {
                    b.Navigation("Words");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.TextDbModel", b =>
                {
                    b.Navigation("MetaAnnotationNavigation");

                    b.Navigation("Sentences");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.UserDbModel", b =>
                {
                    b.Navigation("Texts");
                });
#pragma warning restore 612, 618
        }
    }
}