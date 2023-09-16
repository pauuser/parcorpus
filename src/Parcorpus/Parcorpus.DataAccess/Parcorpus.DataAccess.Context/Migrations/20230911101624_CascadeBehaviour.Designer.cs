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
    [Migration("20230911101624_CascadeBehaviour")]
    partial class CascadeBehaviour
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
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("CountryId");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.GenreDbModel", b =>
                {
                    b.Property<int>("GenreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("GenreId"));

                    b.Property<string>("Name")
                        .IsRequired()
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
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LanguageId");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.LanguagePairDbModel", b =>
                {
                    b.Property<int>("LanguagePairId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("LanguagePairId"));

                    b.Property<int>("FromLanguage")
                        .HasColumnType("integer");

                    b.Property<int>("ToLanguage")
                        .HasColumnType("integer");

                    b.HasKey("LanguagePairId");

                    b.HasIndex("FromLanguage");

                    b.HasIndex("ToLanguage");

                    b.ToTable("LanguagePairs");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.MetaAnnotationDbModel", b =>
                {
                    b.Property<int>("MetaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MetaId"));

                    b.Property<DateTime>("AddDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("CreationYear")
                        .HasColumnType("integer");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("MetaId");

                    b.ToTable("MetaAnnotations");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.MetaGenreDbModel", b =>
                {
                    b.Property<int>("MgId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MgId"));

                    b.Property<int>("GenreId")
                        .HasColumnType("integer");

                    b.Property<int>("MetaId")
                        .HasColumnType("integer");

                    b.HasKey("MgId");

                    b.HasIndex("GenreId");

                    b.HasIndex("MetaId");

                    b.ToTable("MetaGenres");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.SentenceDbModel", b =>
                {
                    b.Property<int>("SentenceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SentenceId"));

                    b.Property<string>("AlignedTranslation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SourceText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SourceTextId")
                        .HasColumnType("integer");

                    b.HasKey("SentenceId");

                    b.HasIndex("SourceTextId");

                    b.ToTable("Sentences");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.TextDbModel", b =>
                {
                    b.Property<int>("TextId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("TextId"));

                    b.Property<Guid>("AddedBy")
                        .HasColumnType("uuid");

                    b.Property<int>("LanguagePair")
                        .HasColumnType("integer");

                    b.Property<int>("MetaAnnotation")
                        .HasColumnType("integer");

                    b.HasKey("TextId");

                    b.HasIndex("AddedBy");

                    b.HasIndex("LanguagePair");

                    b.HasIndex("MetaAnnotation")
                        .IsUnique();

                    b.ToTable("Texts");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.UserDbModel", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Country")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NativeLanguage")
                        .HasColumnType("integer");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.HasIndex("Country");

                    b.HasIndex("NativeLanguage");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.WordDbModel", b =>
                {
                    b.Property<int>("WordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("WordId"));

                    b.Property<string>("AlignedWord")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MorphologicalAnnotation")
                        .HasColumnType("integer");

                    b.Property<int>("Sentence")
                        .HasColumnType("integer");

                    b.Property<string>("SourceWord")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("WordId");

                    b.HasIndex("Sentence");

                    b.ToTable("Words");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.LanguagePairDbModel", b =>
                {
                    b.HasOne("Parcorpus.DataAccess.Models.LanguageDbModel", "FromLanguageNavigation")
                        .WithMany("LanguagePairFromLanguageNavigations")
                        .HasForeignKey("FromLanguage")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Parcorpus.DataAccess.Models.LanguageDbModel", "ToLanguageNavigation")
                        .WithMany("LanguagePairToLanguageNavigations")
                        .HasForeignKey("ToLanguage")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FromLanguageNavigation");

                    b.Navigation("ToLanguageNavigation");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.MetaGenreDbModel", b =>
                {
                    b.HasOne("Parcorpus.DataAccess.Models.GenreDbModel", "GenreNavigation")
                        .WithMany("MetaGenres")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Parcorpus.DataAccess.Models.MetaAnnotationDbModel", "MetaNavigation")
                        .WithMany("MetaGenresNavigation")
                        .HasForeignKey("MetaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GenreNavigation");

                    b.Navigation("MetaNavigation");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.SentenceDbModel", b =>
                {
                    b.HasOne("Parcorpus.DataAccess.Models.TextDbModel", "TextNavigation")
                        .WithMany("Sentences")
                        .HasForeignKey("SourceTextId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TextNavigation");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.TextDbModel", b =>
                {
                    b.HasOne("Parcorpus.DataAccess.Models.UserDbModel", "AddedByNavigation")
                        .WithMany("Texts")
                        .HasForeignKey("AddedBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Parcorpus.DataAccess.Models.LanguagePairDbModel", "LanguagePairNavigation")
                        .WithMany("Texts")
                        .HasForeignKey("LanguagePair")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Parcorpus.DataAccess.Models.MetaAnnotationDbModel", "MetaAnnotationNavigation")
                        .WithOne("TextNavigation")
                        .HasForeignKey("Parcorpus.DataAccess.Models.TextDbModel", "MetaAnnotation")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AddedByNavigation");

                    b.Navigation("LanguagePairNavigation");

                    b.Navigation("MetaAnnotationNavigation");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.UserDbModel", b =>
                {
                    b.HasOne("Parcorpus.DataAccess.Models.CountryDbModel", "CountryNavigation")
                        .WithMany("Users")
                        .HasForeignKey("Country")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Parcorpus.DataAccess.Models.LanguageDbModel", "NativeLanguageNavigation")
                        .WithMany("Users")
                        .HasForeignKey("NativeLanguage")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CountryNavigation");

                    b.Navigation("NativeLanguageNavigation");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.WordDbModel", b =>
                {
                    b.HasOne("Parcorpus.DataAccess.Models.SentenceDbModel", "SentenceNavigation")
                        .WithMany("WordsNavigation")
                        .HasForeignKey("Sentence")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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
                    b.Navigation("MetaGenresNavigation");

                    b.Navigation("TextNavigation")
                        .IsRequired();
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.SentenceDbModel", b =>
                {
                    b.Navigation("WordsNavigation");
                });

            modelBuilder.Entity("Parcorpus.DataAccess.Models.TextDbModel", b =>
                {
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
