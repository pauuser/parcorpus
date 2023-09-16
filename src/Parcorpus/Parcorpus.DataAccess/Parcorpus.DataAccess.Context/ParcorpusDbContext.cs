using Microsoft.EntityFrameworkCore;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.DataAccess.Context;

public partial class ParcorpusDbContext : DbContext
{
    public DbSet<CountryDbModel> Countries { get; set; }
    
    public DbSet<GenreDbModel> Genres { get; set; }
    
    public DbSet<LanguageDbModel> Languages { get; set; }
    
    public DbSet<LanguagePairDbModel> LanguagePairs { get; set; }
    
    public DbSet<MetaAnnotationDbModel> MetaAnnotations { get; set; }

    public DbSet<MetaGenreDbModel> MetaGenres { get; set; }

    public DbSet<SentenceDbModel> Sentences { get; set; }
    
    public DbSet<TextDbModel> Texts { get; set; }
    
    public DbSet<UserDbModel> Users { get; set; }
    
    public DbSet<WordDbModel> Words { get; set; }

    public DbSet<CredentialDbModel> Credentials { get; set; }

    public DbSet<SearchHistoryDbModel> SearchHistory { get; set; }

    public ParcorpusDbContext(DbContextOptions<ParcorpusDbContext> options) : base(options)
    {
    }
    
    public ParcorpusDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LanguageDbModel>(entity =>
        {
            entity.HasKey(l => l.LanguageId);
            entity.Property(s => s.LanguageId)
                .ValueGeneratedOnAdd();
            
            entity.HasMany(l => l.LanguagePairFromLanguageNavigations)
                  .WithOne(lp => lp.FromLanguageNavigation);
            
            entity.HasMany(l => l.LanguagePairToLanguageNavigations)
                  .WithOne(lp => lp.ToLanguageNavigation);
        });
            

        modelBuilder.Entity<TextDbModel>(entity =>
        {
            entity.HasKey(t => t.TextId);
            entity.Property(s => s.TextId)
                .ValueGeneratedOnAdd();

            entity.HasOne(t => t.MetaAnnotationNavigation)
                .WithOne(m => m.TextNavigation)
                .HasForeignKey<TextDbModel>(m => m.MetaAnnotation)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.LanguagePairNavigation)
                .WithMany(p => p.Texts)
                .HasForeignKey(d => d.LanguagePair);

            entity.HasOne(d => d.AddedByNavigation)
                .WithMany(p => p.TextsNavigation)
                .HasForeignKey(d => d.AddedBy);
        });

        modelBuilder.Entity<CountryDbModel>(entity =>
        {
            entity.HasKey(c => c.CountryId);
            entity.Property(s => s.CountryId)
                .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<GenreDbModel>(entity =>
        {
            entity.HasKey(g => g.GenreId);
            entity.Property(s => s.GenreId)
                .ValueGeneratedOnAdd();
        });
        
        modelBuilder.Entity<LanguagePairDbModel>(entity =>
        {
            entity.HasKey(g => g.LanguagePairId);
            entity.Property(s => s.LanguagePairId)
                .ValueGeneratedOnAdd();

            entity.HasOne(d => d.FromLanguageNavigation)
                .WithMany(p => p.LanguagePairFromLanguageNavigations)
                .HasForeignKey(d => d.FromLanguage);

            entity.HasOne(d => d.ToLanguageNavigation)
                .WithMany(p => p.LanguagePairToLanguageNavigations)
                .HasForeignKey(d => d.ToLanguage);

        });
        
        modelBuilder.Entity<MetaAnnotationDbModel>(entity =>
        {
            entity.HasKey(g => g.MetaId);
            entity.Property(s => s.MetaId)
                .ValueGeneratedOnAdd();

            entity.HasMany(m => m.MetaGenresNavigation)
                .WithOne(mg => mg.MetaNavigation)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<MetaGenreDbModel>(entity =>
        {
            entity.HasKey(g => g.MgId);
            entity.Property(s => s.MgId)
                .ValueGeneratedOnAdd();

            entity.HasOne(d => d.GenreNavigation)
                .WithMany(p => p.MetaGenres)
                .HasForeignKey(d => d.GenreId);

            entity.HasOne(d => d.MetaNavigation)
                .WithMany(p => p.MetaGenresNavigation)
                .HasForeignKey(d => d.MetaId);
        });
        
        modelBuilder.Entity<SentenceDbModel>(entity =>
        {
            entity.HasKey(g => g.SentenceId);
            entity.Property(s => s.SentenceId)
                .ValueGeneratedOnAdd();

            entity.HasOne(d => d.TextNavigation)
                .WithMany(p => p.SentencesNavigation)
                .HasForeignKey(d => d.SourceTextId);
        });
        
        modelBuilder.Entity<WordDbModel>(entity =>
        {
            entity.HasKey(g => g.WordId);
            entity.Property(s => s.WordId)
                .ValueGeneratedOnAdd();

            entity.HasOne(d => d.SentenceNavigation)
                .WithMany(p => p.WordsNavigation)
                .HasForeignKey(d => d.Sentence);

        });
        
        modelBuilder.Entity<UserDbModel>(entity =>
        {
            entity.HasKey(g => g.UserId);
            entity.Property(s => s.UserId)
                .ValueGeneratedOnAdd();

            entity.HasOne(d => d.CountryNavigation)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.Country);

            entity.HasOne(d => d.NativeLanguageNavigation)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.NativeLanguage);
        });

        modelBuilder.Entity<CredentialDbModel>(entity =>
        {
            entity.HasKey(c => c.CredentialId);
            entity.Property(c => c.CredentialId)
                .ValueGeneratedOnAdd();

            entity.HasOne(c => c.UserNavigation)
                .WithOne(u => u.CredentialNavigation);
        });

        modelBuilder.Entity<SearchHistoryDbModel>(entity =>
        {
            entity.HasKey(s => s.SearchHistoryId);
            entity.Property(s => s.SearchHistoryId)
                .ValueGeneratedOnAdd();
            
            entity.Property(s => s.Query)    
                .HasColumnType("jsonb");

            entity.HasOne(s => s.UserNavigation)
                .WithMany(u => u.SearchHistoryNavigation)
                .HasForeignKey(s => s.UserId);
        });
    }
}