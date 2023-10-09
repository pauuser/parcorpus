using System.Collections.Specialized;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.EntityFrameworkCore;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Context;
using Parcorpus.DataAccess.Models;
using Parcorpus.DataAccess.Repositories;
using Parcorpus.UnitTests.Common.Factories.CoreModels;
using Parcorpus.UnitTests.Common.Factories.DbModels;
using Parcorpus.UnitTests.Common.Helpers;

namespace Parcorpus.UnitTests.DataAccess;

public class LanguageRepositoryUnitTests
{
    private readonly ILanguageRepository _languageRepository;
    
    private readonly Mock<ParcorpusDbContext> _context = new();

    public LanguageRepositoryUnitTests()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var configuration = ConfigurationHelper.InitConfiguration<CacheConfiguration>();

        _languageRepository = new LanguageRepository(NullLogger<LanguageRepository>.Instance,
            _context.Object,
            cache,
            configuration);
    }

    [Fact]
    public async Task GetTextByIdOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;
        var numberOfSentences = 150;

        var text = MockFullText(userId, textId, numberOfSentences);
        var paging = new PaginationParameters(pageNumber: null, pageSize: null);
        
        var expectedText = new PagedText(sentencesPageNumber: 1,
            sentencesPageSize: numberOfSentences,
            sentencesTotalCount: numberOfSentences,
            text: text);
        
        // Act
        var actualText = await _languageRepository.GetTextById(textId, paging);

        // Assert
        Assert.Equal(expectedText, actualText);
    }
    
    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 5)]
    [InlineData(1, 150)]
    [InlineData(5, 7)]
    [InlineData(14, 11)]
    public async Task GetUserJobsValidPaginationTest(int pageNumber, int pageSize)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;
        var numberOfSentences = 150;

        var text = MockFullText(userId, textId, numberOfSentences);
        text.Sentences = text.Sentences
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var paging = new PaginationParameters(pageNumber: pageNumber, pageSize: pageSize);
        
        var expectedText = new PagedText(sentencesPageNumber: pageNumber,
            sentencesPageSize: pageSize,
            sentencesTotalCount: numberOfSentences,
            text: text);
        
        // Act
        var actualText = await _languageRepository.GetTextById(textId, paging);
        
        // Assert
        Assert.Equal(expectedText, actualText);
    }

    [Theory]
    [InlineData(0, 1, 150)]
    [InlineData(15, 11, 150)]
    [InlineData(1, 1, 0)]
    public async Task GetUserJobsInvalidPagingTest(int pageNumber, int pageSize, int numberOfSentences)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;

        MockFullText(userId, textId, numberOfSentences);
        var paging = new PaginationParameters(pageNumber: pageNumber, pageSize: pageSize);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidPagingException>(() => _languageRepository.GetTextById(textId, paging));
    }
    
    [Fact]
    public async Task GetTextByIdExceptionTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;
        var numberOfSentences = 150;

        MockFullText(userId, textId, numberOfSentences);
        _context.Setup<DbSet<TextDbModel>>(s => s.Texts)
            .Throws<Exception>();
        var paging = new PaginationParameters(pageNumber: null, pageSize: null);
        
        // Act & Assert
        await Assert.ThrowsAsync<LanguageRepositoryException>(() => _languageRepository.GetTextById(textId, paging));
    }

    private Text MockFullText(Guid userId, int textId, int numberOfSentences)
    {
        var genre = GenreDbModelFactory.Create();
        _context.Setup<DbSet<GenreDbModel>>(s => s.Genres)
            .ReturnsDbSet(new List<GenreDbModel> { genre }.AsQueryable());
        
        var metaAnnotation = MetaAnnotationDbModelFactory.Create();
        _context.Setup<DbSet<MetaAnnotationDbModel>>(s => s.MetaAnnotations)
            .ReturnsDbSet(new List<MetaAnnotationDbModel> { metaAnnotation }.AsQueryable());

        var metaGenre = MetaGenreDbModelFactory.Create(metaId: metaAnnotation.MetaId, genreId: genre.GenreId);
        metaGenre.GenreNavigation = genre;
        metaGenre.MetaNavigation = metaAnnotation;
        _context.Setup<DbSet<MetaGenreDbModel>>(s => s.MetaGenres)
            .ReturnsDbSet(new List<MetaGenreDbModel> { metaGenre }.AsQueryable());

        var sourceLanguage = LanguageDbModelFactory.Create(1, "en", "English");
        var targetLanguage = LanguageDbModelFactory.Create(2, "ru", "Russian");
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(new List<LanguageDbModel> { sourceLanguage, targetLanguage }.AsQueryable());

        var languagePair = LanguagePairDbModelFactory.Create(fromLanguage: sourceLanguage.LanguageId,
            toLanguage: targetLanguage.LanguageId);
        languagePair.FromLanguageNavigation = sourceLanguage;
        languagePair.ToLanguageNavigation = targetLanguage;
        _context.Setup<DbSet<LanguagePairDbModel>>(s => s.LanguagePairs)
            .ReturnsDbSet(new List<LanguagePairDbModel> { languagePair }.AsQueryable());

        var user = UserDbModelFactory.Create(userId: userId);
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> { user }.AsQueryable());

        var text = TextDbModelFactory.Create(textId: textId, metaAnnotation: metaAnnotation.MetaId,
            languagePair: languagePair.LanguagePairId, addedBy: userId);
        var sentences = Enumerable.Range(1, numberOfSentences)
            .Select(i => SentenceDbModelFactory.Create(sentenceId: i, sourceTextId: textId)).ToList();
        text.SentencesNavigation = sentences;
        text.AddedByNavigation = user;
        text.LanguagePairNavigation = languagePair;
        text.MetaAnnotationNavigation = metaAnnotation;
        var words = new List<WordDbModel>();
        foreach (var sentence in sentences)
        {
            var newWords = Enumerable.Range(1, 5)
                .Select(i => WordDbModelFactory.Create(i, sentence: sentence.SentenceId, sentenceModel: sentence)).ToList();
            
            words.AddRange(newWords);
            sentence.WordsNavigation = newWords;
        }
        _context.Setup<DbSet<TextDbModel>>(s => s.Texts)
            .ReturnsDbSet(new List<TextDbModel> { text }.AsQueryable());
        _context.Setup<DbSet<SentenceDbModel>>(s => s.Sentences)
            .ReturnsDbSet(sentences.AsQueryable());
        _context.Setup<DbSet<WordDbModel>>(s => s.Words)
            .ReturnsDbSet(words.AsQueryable());
        
        return TextFactory.Create(text);
    }
}