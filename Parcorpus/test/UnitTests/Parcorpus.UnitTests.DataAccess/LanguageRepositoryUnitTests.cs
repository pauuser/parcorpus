using Allure.Xunit.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
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

[TestCaseOrderer(
    ordererTypeName: "Parcorpus.UnitTests.Common.Orderers.RandomOrderer",
    ordererAssemblyName: "Parcorpus.UnitTests.Common")]
[AllureSuite("Repositories")]
[AllureSubSuite("LanguageRepository")]
public class LanguageRepositoryUnitTests
{
    private readonly ILanguageRepository _languageRepository;
    
    private readonly Mock<ParcorpusDbContext> _context = new();
    private readonly Mock<ITransactionManager> _transactionManager = new();

    public LanguageRepositoryUnitTests()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var configuration = ConfigurationHelper.InitConfiguration<CacheConfiguration>();
        
        _languageRepository = new LanguageRepository(NullLogger<LanguageRepository>.Instance,
            _context.Object,
            _transactionManager.Object,
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
    public async Task GetTextByIdValidPaginationTest(int pageNumber, int pageSize)
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
    public async Task GetTextByIdInvalidPagingTest(int pageNumber, int pageSize, int numberOfSentences)
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
    
    [Theory]
    [InlineData("Author", "Title", "ru", "en", true)]
    [InlineData("Author1", "Title", "ru", "en", false)]
    [InlineData("Author", "Title1", "ru", "en", false)]
    [InlineData("Author", "Title", "en", "ru", false)]
    [InlineData("Author", "Title", "ru", "fr", false)]
    [InlineData("Author1", "Title", "ru", "fr", false)]
    public async Task TextExistsOkTest(string author, string title, 
        string sourceLanguage, string destinationLanguage, bool expectedResult)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;
        var numberOfSentences = 150;

        var sourceLanguageDbModel = LanguageDbModelFactory.Create(shortName: "ru", fullName: "Russian");
        var targetLanguageDbModel = LanguageDbModelFactory.Create(shortName: "en", fullName: "English");
        var metaAnnotationDbModel = MetaAnnotationDbModelFactory.Create(author: "Author", title: "Title");

        MockFullText(userId, textId, numberOfSentences, sourceLanguage: sourceLanguageDbModel, 
            targetLanguage: targetLanguageDbModel, metaAnnotation:metaAnnotationDbModel);

        var languageConfiguration = ConfigurationHelper.InitConfiguration<LanguagesConfiguration>().Value;
        var inSourceLanguageDbModel = LanguageFactory.Create(shortName: sourceLanguage, languageConfiguration);
        var inTargetLanguageDbModel = LanguageFactory.Create(shortName: destinationLanguage, languageConfiguration);
        var inMetaAnnotationDbModel = MetaAnnotationFactory.Create(author: author, title: title);
        
        // Act
        var actualResult = await _languageRepository.TextExists(inMetaAnnotationDbModel, inSourceLanguageDbModel, inTargetLanguageDbModel);

        // Assert
        Assert.Equal(expectedResult, actualResult);
    }
    
    [Fact]
    public async Task TextExistsExceptionTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;
        var numberOfSentences = 150;
        
        MockFullText(userId, textId, numberOfSentences);
        _context.Setup<DbSet<TextDbModel>>(s => s.Texts)
            .Throws<Exception>();
        
        var languageConfiguration = ConfigurationHelper.InitConfiguration<LanguagesConfiguration>().Value;
        var inSourceLanguageDbModel = LanguageFactory.Create(shortName: "fr", languageConfiguration);
        var inTargetLanguageDbModel = LanguageFactory.Create(shortName: "de", languageConfiguration);
        var inMetaAnnotationDbModel = MetaAnnotationFactory.Create(author: "Author", title: "Title");
        
        // Act & Assert
        await Assert.ThrowsAsync<LanguageRepositoryException>(() => _languageRepository.TextExists(inMetaAnnotationDbModel, 
            inSourceLanguageDbModel, inTargetLanguageDbModel));
    }
    
    [Fact]
    public async Task DeleteTextByIdOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;
        var numberOfSentences = 150;

        var internalTexts = new List<TextDbModel>();
        MockFullText(userId, textId, numberOfSentences, internalTexts: internalTexts);
        _context.Setup(s => s.Texts.Remove(It.IsAny<TextDbModel>()))
            .Returns((TextDbModel text) =>
            {
                var entry = EntityHelper.GetMockEntityEntry(text);
                internalTexts.RemoveAll(t => t.TextId == text.TextId);
                
                return entry;
            });
        
        // Act
        await _languageRepository.DeleteTextById(userId, textId);

        // Assert
        Assert.Empty(internalTexts);
    }
    
    [Fact]
    public async Task DeleteTextByIdNotFoundTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;
        var numberOfSentences = 150;

        var internalTexts = new List<TextDbModel>();
        MockFullText(userId, textId, numberOfSentences, internalTexts: internalTexts);
        _context.Setup(s => s.Texts.Remove(It.IsAny<TextDbModel>()))
            .Returns((TextDbModel text) =>
            {
                var entry = EntityHelper.GetMockEntityEntry(text);
                internalTexts.RemoveAll(t => t.TextId == text.TextId);
                
                return entry;
            });
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _languageRepository.DeleteTextById(userId, textId: 1));
    }
    
    [Fact]
    public async Task DeleteTextByIdExceptionTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;
        var numberOfSentences = 150;

        var internalTexts = new List<TextDbModel>();
        MockFullText(userId, textId, numberOfSentences, internalTexts: internalTexts);
        _context.Setup(s => s.Texts.Remove(It.IsAny<TextDbModel>()))
            .Throws<Exception>();
        
        // Act & Assert
        await Assert.ThrowsAsync<LanguageRepositoryException>(() => _languageRepository.DeleteTextById(userId, textId: textId));
    }

    [Fact]
    public async Task AddAlignedTextOkTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var text = TextFactory.Create();

        var userDb = UserDbModelFactory.Create(userId: userId);
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> { userDb }.AsQueryable());

        List<TextDbModel> internalTexts = new();
        List<GenreDbModel> internalGenres = new();
        List<LanguageDbModel> languageDbModels = new();
        List<LanguagePairDbModel> languagePairDbModels = new();
        List<MetaAnnotationDbModel> metaAnnotationDbModels = new();
        List<MetaGenreDbModel> metaGenreDbModels = new();
        List<SentenceDbModel> sentenceDbModels = new();
        List<WordDbModel> wordDbModels = new();
        MockDbSets(internalTexts, internalGenres, languageDbModels,
            languagePairDbModels, metaAnnotationDbModels, metaGenreDbModels, sentenceDbModels, wordDbModels);

        var transaction = new Mock<IDbContextTransaction>();
        _transactionManager.Setup(s => s.BeginTransactionAsync())
            .ReturnsAsync(transaction.Object);
        
        // Act
        await _languageRepository.AddAlignedText(userId, text);

        // Assert
        Assert.NotEmpty(internalTexts);
    }
    
    [Fact]
    public async Task AddAlignedTextExceptionTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var text = TextFactory.Create();
        
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .Throws<Exception>();

        List<TextDbModel> internalTexts = new();
        List<GenreDbModel> internalGenres = new();
        List<LanguageDbModel> languageDbModels = new();
        List<LanguagePairDbModel> languagePairDbModels = new();
        List<MetaAnnotationDbModel> metaAnnotationDbModels = new();
        List<MetaGenreDbModel> metaGenreDbModels = new();
        List<SentenceDbModel> sentenceDbModels = new();
        List<WordDbModel> wordDbModels = new();
        MockDbSets(internalTexts, internalGenres, languageDbModels,
            languagePairDbModels, metaAnnotationDbModels, metaGenreDbModels, sentenceDbModels, wordDbModels);
        
        var transaction = new Mock<IDbContextTransaction>();
        _transactionManager.Setup(s => s.BeginTransactionAsync())
            .ReturnsAsync(transaction.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<LanguageRepositoryException>(() => _languageRepository.AddAlignedText(userId, text));
    }
    
    [Fact]
    public async Task AddAlignedTextNoLanguagesTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var text = TextFactory.Create(sourceLanguage: new Language("br", "Brbrbr"));

        var userDb = UserDbModelFactory.Create(userId: userId);
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> { userDb }.AsQueryable());

        List<TextDbModel> internalTexts = new();
        List<GenreDbModel> internalGenres = new();
        List<LanguageDbModel> languageDbModels = new();
        List<LanguagePairDbModel> languagePairDbModels = new();
        List<MetaAnnotationDbModel> metaAnnotationDbModels = new();
        List<MetaGenreDbModel> metaGenreDbModels = new();
        List<SentenceDbModel> sentenceDbModels = new();
        List<WordDbModel> wordDbModels = new();
        MockDbSets(internalTexts, internalGenres, languageDbModels,
            languagePairDbModels, metaAnnotationDbModels, metaGenreDbModels, sentenceDbModels, wordDbModels);

        var transaction = new Mock<IDbContextTransaction>();
        _transactionManager.Setup(s => s.BeginTransactionAsync())
            .ReturnsAsync(transaction.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidLanguageException>(() => _languageRepository.AddAlignedText(userId, text));
    }
    
    [Fact]
    public async Task AddAlignedTextNoUserTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var text = TextFactory.Create();
        
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(new List<UserDbModel> {  }.AsQueryable());

        List<TextDbModel> internalTexts = new();
        List<GenreDbModel> internalGenres = new();
        List<LanguageDbModel> languageDbModels = new();
        List<LanguagePairDbModel> languagePairDbModels = new();
        List<MetaAnnotationDbModel> metaAnnotationDbModels = new();
        List<MetaGenreDbModel> metaGenreDbModels = new();
        List<SentenceDbModel> sentenceDbModels = new();
        List<WordDbModel> wordDbModels = new();
        MockDbSets(internalTexts, internalGenres, languageDbModels,
            languagePairDbModels, metaAnnotationDbModels, metaGenreDbModels, sentenceDbModels, wordDbModels);

        var transaction = new Mock<IDbContextTransaction>();
        _transactionManager.Setup(s => s.BeginTransactionAsync())
            .ReturnsAsync(transaction.Object);
        
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _languageRepository.AddAlignedText(userId, text));
    }
    
    [Fact]
    public async Task GetTextsAddedByUserNoPaginationTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;
        var numberOfTexts = 150;
        
        List<TextDbModel> internalTexts = new();
        List<GenreDbModel> internalGenres = new();
        List<LanguageDbModel> languageDbModels = new();
        List<LanguagePairDbModel> languagePairDbModels = new();
        List<MetaAnnotationDbModel> metaAnnotationDbModels = new();
        List<MetaGenreDbModel> metaGenreDbModels = new();
        List<SentenceDbModel> sentenceDbModels = new();
        List<WordDbModel> wordDbModels = new();
        List<UserDbModel> userDbModels = new();

        var texts = Enumerable.Range(1, numberOfTexts)
            .Select(i => MockFullText(userId, textId, 150, title: i.ToString(),
                internalTexts, internalGenres, languageDbModels, languagePairDbModels, metaAnnotationDbModels, 
                metaGenreDbModels, sentenceDbModels, wordDbModels, userDbModels)).ToList();
        
        var paging = new PaginationParameters(pageNumber: null, pageSize: null);
        
        var expectedTexts = new Paged<Text>(pageNumber: 1,
            pageSize: numberOfTexts,
            totalCount: numberOfTexts,
            items: texts);
        
        // Act
        var actualTexts = await _languageRepository.GetTextsAddedByUser(userId, paging);

        // Assert
        Assert.Equal(expectedTexts, actualTexts);
    }
    
    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 5)]
    [InlineData(1, 150)]
    [InlineData(5, 7)]
    [InlineData(14, 11)]
    public async Task GetTextsAddedByUserValidPaginationTest(int pageNumber, int pageSize)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;
        var numberOfTexts = 150;
        
        List<TextDbModel> internalTexts = new();
        List<GenreDbModel> internalGenres = new();
        List<LanguageDbModel> languageDbModels = new();
        List<LanguagePairDbModel> languagePairDbModels = new();
        List<MetaAnnotationDbModel> metaAnnotationDbModels = new();
        List<MetaGenreDbModel> metaGenreDbModels = new();
        List<SentenceDbModel> sentenceDbModels = new();
        List<WordDbModel> wordDbModels = new();
        List<UserDbModel> userDbModels = new();

        var texts = Enumerable.Range(1, numberOfTexts)
            .Select(i => MockFullText(userId, textId, 150, title: i.ToString(),
                internalTexts, internalGenres, languageDbModels, languagePairDbModels, metaAnnotationDbModels, 
                metaGenreDbModels, sentenceDbModels, wordDbModels, userDbModels)).ToList();
        
        var paging = new PaginationParameters(pageNumber: pageNumber, pageSize: pageSize);
        
        var expectedTexts = new Paged<Text>(pageNumber: pageNumber,
            pageSize: pageSize,
            totalCount: numberOfTexts,
            items: texts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList());
        
        // Act
        var actualTexts = await _languageRepository.GetTextsAddedByUser(userId, paging);

        // Assert
        Assert.Equal(expectedTexts, actualTexts);
    }

    [Theory]
    [InlineData(0, 1, 150)]
    public async Task GetSearchHistoryInvalidPagingTest(int pageNumber, int pageSize, int numberOfTexts)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;
        
        List<TextDbModel> internalTexts = new();
        List<GenreDbModel> internalGenres = new();
        List<LanguageDbModel> languageDbModels = new();
        List<LanguagePairDbModel> languagePairDbModels = new();
        List<MetaAnnotationDbModel> metaAnnotationDbModels = new();
        List<MetaGenreDbModel> metaGenreDbModels = new();
        List<SentenceDbModel> sentenceDbModels = new();
        List<WordDbModel> wordDbModels = new();
        List<UserDbModel> userDbModels = new();

        var texts = Enumerable.Range(1, numberOfTexts)
            .Select(i => MockFullText(userId, textId, 150, title: i.ToString(),
                internalTexts, internalGenres, languageDbModels, languagePairDbModels, metaAnnotationDbModels, 
                metaGenreDbModels, sentenceDbModels, wordDbModels, userDbModels)).ToList();

        MockFullText(Guid.Empty, textId, 150, title: string.Empty,
            internalTexts, internalGenres, languageDbModels, languagePairDbModels, metaAnnotationDbModels,
            metaGenreDbModels, sentenceDbModels, wordDbModels, userDbModels);
        
        var paging = new PaginationParameters(pageNumber: pageNumber, pageSize: pageSize);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidPagingException>(() => _languageRepository.GetTextsAddedByUser(userId, paging));
    }
    
    [Fact]
    public async Task GetSearchHistoryExceptionTest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _context.Setup<DbSet<TextDbModel>>(s => s.Texts)
            .Throws<Exception>();
        
        var paging = new PaginationParameters(pageNumber: null, pageSize: null);
        
        // Act & Assert
        await Assert.ThrowsAsync<LanguageRepositoryException>(() => _languageRepository.GetTextsAddedByUser(userId, paging));
    }
    
    [Fact]
    public async Task GetConcordanceNoPaginationTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;
        var numberOfSentences = 30;
        var numberOfTexts = 150;
        
        List<TextDbModel> internalTexts = new();
        List<GenreDbModel> internalGenres = new();
        List<LanguageDbModel> languageDbModels = new();
        List<LanguagePairDbModel> languagePairDbModels = new();
        List<MetaAnnotationDbModel> metaAnnotationDbModels = new();
        List<MetaGenreDbModel> metaGenreDbModels = new();
        List<SentenceDbModel> sentenceDbModels = new();
        List<WordDbModel> wordDbModels = new();
        List<UserDbModel> userDbModels = new();

        var texts = Enumerable.Range(1, numberOfTexts)
            .Select(i => MockFullText(userId, textId, numberOfSentences, "Title",
                internalTexts, internalGenres, languageDbModels, languagePairDbModels, metaAnnotationDbModels, 
                metaGenreDbModels, sentenceDbModels, wordDbModels, userDbModels)).ToList();
        
        var paging = new PaginationParameters(pageNumber: null, pageSize: null);
        
        var expectedConcordance = new Paged<Concordance>(pageNumber: 1,
            pageSize: numberOfTexts * numberOfSentences * 5,
            totalCount: numberOfTexts * numberOfSentences * 5,
            items: Enumerable.Range(1, numberOfTexts * numberOfSentences * 5).Select(_ => ConcordanceFactory.Create("apple", "яблоко", sourceText: "apple", alignedTranslation: "яблоко")).ToList());

        var languageConfiguration = ConfigurationHelper.InitConfiguration<LanguagesConfiguration>();
        var word = new Word("apple", LanguageFactory.Create("en", languageConfiguration.Value));
        var filter = FilterFactory.Create();
        
        // Act
        var actualConcordance = await _languageRepository.GetConcordance(word: word,
            desiredLanguage: LanguageFactory.Create("ru", languageConfiguration.Value), filter: filter, paging);

        // Assert
        Assert.Equal(expectedConcordance, actualConcordance);
    }
    
    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 5)]
    [InlineData(1, 150)]
    [InlineData(5, 7)]
    [InlineData(14, 11)]
    public async Task GetConcordanceValidPaginationTest(int pageNumber, int pageSize)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;
        var numberOfSentences = 30;
        var numberOfTexts = 150;
        var totalItems = numberOfTexts * numberOfSentences * 5;
        
        List<TextDbModel> internalTexts = new();
        List<GenreDbModel> internalGenres = new();
        List<LanguageDbModel> languageDbModels = new();
        List<LanguagePairDbModel> languagePairDbModels = new();
        List<MetaAnnotationDbModel> metaAnnotationDbModels = new();
        List<MetaGenreDbModel> metaGenreDbModels = new();
        List<SentenceDbModel> sentenceDbModels = new();
        List<WordDbModel> wordDbModels = new();
        List<UserDbModel> userDbModels = new();

        var texts = Enumerable.Range(1, numberOfTexts)
            .Select(i => MockFullText(userId, textId, numberOfSentences, "Title",
                internalTexts, internalGenres, languageDbModels, languagePairDbModels, metaAnnotationDbModels, 
                metaGenreDbModels, sentenceDbModels, wordDbModels, userDbModels)).ToList();
        
        var paging = new PaginationParameters(pageNumber: pageNumber, pageSize: pageSize);
        
        var expectedConcordance = new Paged<Concordance>(pageNumber: pageNumber,
            pageSize: pageSize,
            totalCount: totalItems,
            items: Enumerable.Range(1, numberOfTexts * numberOfSentences * 5)
                .Select(_ => ConcordanceFactory.Create("apple", "яблоко", sourceText: "apple", alignedTranslation: "яблоко"))
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList());

        var languageConfiguration = ConfigurationHelper.InitConfiguration<LanguagesConfiguration>();
        var word = new Word("apple", LanguageFactory.Create("en", languageConfiguration.Value));
        var filter = FilterFactory.Create();
        
        // Act
        var actualConcordance = await _languageRepository.GetConcordance(word: word,
            desiredLanguage: LanguageFactory.Create("ru", languageConfiguration.Value), filter: filter, paging);

        // Assert
        Assert.Equal(expectedConcordance, actualConcordance);
    }

    [Theory]
    [InlineData(0, 1, 150)]
    [InlineData(2047, 11, 150)]
    public async Task GetConcordanceInvalidPagingTest(int pageNumber, int pageSize, int numberOfTexts)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var textId = 35;
        var numberOfSentences = 30;
        var totalItems = numberOfTexts * numberOfSentences * 5;
        
        List<TextDbModel> internalTexts = new();
        List<GenreDbModel> internalGenres = new();
        List<LanguageDbModel> languageDbModels = new();
        List<LanguagePairDbModel> languagePairDbModels = new();
        List<MetaAnnotationDbModel> metaAnnotationDbModels = new();
        List<MetaGenreDbModel> metaGenreDbModels = new();
        List<SentenceDbModel> sentenceDbModels = new();
        List<WordDbModel> wordDbModels = new();
        List<UserDbModel> userDbModels = new();

        var texts = Enumerable.Range(1, numberOfTexts)
            .Select(i => MockFullText(userId, textId, numberOfSentences, "Title",
                internalTexts, internalGenres, languageDbModels, languagePairDbModels, metaAnnotationDbModels, 
                metaGenreDbModels, sentenceDbModels, wordDbModels, userDbModels)).ToList();
        
        var paging = new PaginationParameters(pageNumber: pageNumber, pageSize: pageSize);

        var languageConfiguration = ConfigurationHelper.InitConfiguration<LanguagesConfiguration>();
        var word = new Word("apple", LanguageFactory.Create("en", languageConfiguration.Value));
        var filter = FilterFactory.Create();
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidPagingException>(() => _languageRepository.GetConcordance(word: word,
            desiredLanguage: LanguageFactory.Create("ru", languageConfiguration.Value), filter: filter, paging));
    }
    
    [Fact]
    public async Task GetConcordanceExceptionTest()
    {
        // Arrange
        _context.Setup<DbSet<WordDbModel>>(s => s.Words)
            .Throws<Exception>();
        
        var paging = new PaginationParameters(pageNumber: null, pageSize: null);
        
        var languageConfiguration = ConfigurationHelper.InitConfiguration<LanguagesConfiguration>();
        var word = new Word("find", LanguageFactory.Create("en", languageConfiguration.Value));
        var filter = FilterFactory.Create();
        
        // Act & Assert
        await Assert.ThrowsAsync<LanguageRepositoryException>(() => _languageRepository.GetConcordance(word: word,
            desiredLanguage: LanguageFactory.Create("ru", languageConfiguration.Value), filter: filter, paging));
    }


    private Text MockFullText(Guid userId, int textId, int numberOfSentences,
        string title = "Title",
        List<TextDbModel>? internalTexts = null,
        List<GenreDbModel>? internalGenres = null,
        List<LanguageDbModel>? languageDbModels = null,
        List<LanguagePairDbModel>? languagePairDbModels = null,
        List<MetaAnnotationDbModel>? metaAnnotationDbModels = null,
        List<MetaGenreDbModel>? metaGenreDbModels = null,
        List<SentenceDbModel>? sentenceDbModels = null,
        List<WordDbModel>? wordDbModels = null,
        List<UserDbModel>? internalUsers = null,
        LanguageDbModel? sourceLanguage = null, LanguageDbModel? targetLanguage = null,
        MetaAnnotationDbModel? metaAnnotation = null)
    {
        var genre = GenreDbModelFactory.Create();
        internalGenres ??= new();
        internalGenres.Add(genre);
        _context.Setup<DbSet<GenreDbModel>>(s => s.Genres)
            .ReturnsDbSet(internalGenres.AsQueryable());
        
        metaAnnotation ??= MetaAnnotationDbModelFactory.Create(title: title);
        metaAnnotationDbModels ??= new();
        metaAnnotationDbModels.Add(metaAnnotation);
        _context.Setup<DbSet<MetaAnnotationDbModel>>(s => s.MetaAnnotations)
            .ReturnsDbSet(metaAnnotationDbModels.AsQueryable());

        var metaGenre = MetaGenreDbModelFactory.Create(metaId: metaAnnotation.MetaId, genreId: genre.GenreId);
        metaGenre.GenreNavigation = genre;
        metaGenre.MetaNavigation = metaAnnotation;
        metaAnnotation.MetaGenresNavigation.Add(metaGenre);
        metaGenreDbModels ??= new();
        metaGenreDbModels.Add(metaGenre);
        _context.Setup<DbSet<MetaGenreDbModel>>(s => s.MetaGenres)
            .ReturnsDbSet(metaGenreDbModels.AsQueryable());

        sourceLanguage ??= LanguageDbModelFactory.Create(1, "en", "English");
        targetLanguage ??= LanguageDbModelFactory.Create(2, "ru", "Russian");
        languageDbModels ??= new();
        languageDbModels.Add(sourceLanguage);
        languageDbModels.Add(targetLanguage);
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(languageDbModels.AsQueryable());

        var languagePair = LanguagePairDbModelFactory.Create(fromLanguage: sourceLanguage.LanguageId,
            toLanguage: targetLanguage.LanguageId);
        languagePair.FromLanguageNavigation = sourceLanguage;
        languagePair.ToLanguageNavigation = targetLanguage;
        languagePairDbModels ??= new();
        languagePairDbModels.Add(languagePair);
        _context.Setup<DbSet<LanguagePairDbModel>>(s => s.LanguagePairs)
            .ReturnsDbSet(languagePairDbModels.AsQueryable());

        var user = UserDbModelFactory.Create(userId: userId);
        internalUsers ??= new();
        internalUsers.Add(user);
        _context.Setup<DbSet<UserDbModel>>(s => s.Users)
            .ReturnsDbSet(internalUsers.AsQueryable());

        var text = TextDbModelFactory.Create(textId: textId, metaAnnotation: metaAnnotation.MetaId,
            languagePair: languagePair.LanguagePairId, addedBy: userId);
        var sentences = Enumerable.Range(1, numberOfSentences)
            .Select(i =>
            {
                var sentence = SentenceDbModelFactory.Create(sentenceId: i, sourceTextId: textId);
                sentence.TextNavigation = text;
                return sentence;
            }).ToList();
        text.SentencesNavigation = sentences;
        text.AddedByNavigation = user;
        text.LanguagePairNavigation = languagePair;
        text.MetaAnnotationNavigation = metaAnnotation;
        var words = new List<WordDbModel>();
        foreach (var sentence in sentences)
        {
            var newWords = Enumerable.Range(1, 5)
                .Select(i =>
                {
                    var word = WordDbModelFactory.Create(i, sourceWord: "apple", alignedWord: "яблоко",
                        sentence: sentence.SentenceId, sentenceModel: sentence);
                    word.SentenceNavigation = sentence;
                    return word;
                }).ToList();
            
            words.AddRange(newWords);
            sentence.WordsNavigation = newWords;
        }

        internalTexts ??= new List<TextDbModel>();
        internalTexts.Add(text);
        _context.Setup<DbSet<TextDbModel>>(s => s.Texts)
            .ReturnsDbSet(internalTexts.AsQueryable());
        sentenceDbModels ??= new List<SentenceDbModel>();
        sentenceDbModels.AddRange(sentences);
        _context.Setup<DbSet<SentenceDbModel>>(s => s.Sentences)
            .ReturnsDbSet(sentenceDbModels.AsQueryable());
        wordDbModels ??= new List<WordDbModel>();
        wordDbModels.AddRange(words);
        _context.Setup<DbSet<WordDbModel>>(s => s.Words)
            .ReturnsDbSet(wordDbModels.AsQueryable());
        
        return TextFactory.Create(text);
    }

    private void MockDbSets(List<TextDbModel>? internalTexts = null,
        List<GenreDbModel>? internalGenres = null,
        List<LanguageDbModel>? languageDbModels = null,
        List<LanguagePairDbModel>? languagePairDbModels = null,
        List<MetaAnnotationDbModel>? metaAnnotationDbModels = null,
        List<MetaGenreDbModel>? metaGenreDbModels = null,
        List<SentenceDbModel>? sentenceDbModels = null,
        List<WordDbModel>? wordDbModels = null)
    {
        internalTexts ??= new();
        _context.Setup<DbSet<TextDbModel>>(s => s.Texts)
            .ReturnsDbSet(internalTexts.AsQueryable());
        _context.Setup(s => s.Texts.AddAsync(It.IsAny<TextDbModel>(), It.IsAny<CancellationToken>()))
            .Returns((TextDbModel text, CancellationToken cancellationToken) =>
            {
                var entry = EntityHelper.GetMockEntityEntry(text);
                internalTexts.Add(entry.Entity);
                return ValueTask.FromResult(entry);
            });
        
        internalGenres ??= new();
        _context.Setup<DbSet<GenreDbModel>>(s => s.Genres)
            .ReturnsDbSet(internalGenres.AsQueryable());
        _context.Setup(s => s.Genres.AddAsync(It.IsAny<GenreDbModel>(), It.IsAny<CancellationToken>()))
            .Returns((GenreDbModel genre, CancellationToken cancellationToken) =>
            {
                var entry = EntityHelper.GetMockEntityEntry(genre);
                internalGenres.Add(entry.Entity);
                return ValueTask.FromResult(entry);
            });
        
        var sourceLanguage = LanguageDbModelFactory.Create(1, "en", "English");
        var targetLanguage = LanguageDbModelFactory.Create(2, "ru", "Russian");
        languageDbModels ??= new();
        languageDbModels.Add(sourceLanguage);
        languageDbModels.Add(targetLanguage);
        _context.Setup<DbSet<LanguageDbModel>>(s => s.Languages)
            .ReturnsDbSet(languageDbModels.AsQueryable());
        _context.Setup(s => s.Languages.AddAsync(It.IsAny<LanguageDbModel>(), It.IsAny<CancellationToken>()))
            .Returns((LanguageDbModel language, CancellationToken cancellationToken) =>
            {
                var entry = EntityHelper.GetMockEntityEntry(language);
                languageDbModels.Add(entry.Entity);
                return ValueTask.FromResult(entry);
            });

        languagePairDbModels ??= new();
        _context.Setup<DbSet<LanguagePairDbModel>>(s => s.LanguagePairs)
            .ReturnsDbSet(languagePairDbModels.AsQueryable());
        _context.Setup(s => s.LanguagePairs.AddAsync(It.IsAny<LanguagePairDbModel>(), It.IsAny<CancellationToken>()))
            .Returns((LanguagePairDbModel languagePair, CancellationToken cancellationToken) =>
            {
                var entry = EntityHelper.GetMockEntityEntry(languagePair);
                languagePairDbModels.Add(entry.Entity);
                return ValueTask.FromResult(entry);
            });

        metaAnnotationDbModels ??= new();
        _context.Setup<DbSet<MetaAnnotationDbModel>>(s => s.MetaAnnotations)
            .ReturnsDbSet(metaAnnotationDbModels.AsQueryable());
        _context.Setup(s => s.MetaAnnotations.AddAsync(It.IsAny<MetaAnnotationDbModel>(), It.IsAny<CancellationToken>()))
            .Returns((MetaAnnotationDbModel metaAnnotation, CancellationToken cancellationToken) =>
            {
                var entry = EntityHelper.GetMockEntityEntry(metaAnnotation);
                metaAnnotationDbModels.Add(entry.Entity);
                return ValueTask.FromResult(entry);
            });

        metaGenreDbModels ??= new();
        _context.Setup<DbSet<MetaGenreDbModel>>(s => s.MetaGenres)
            .ReturnsDbSet(metaGenreDbModels.AsQueryable());
        _context.Setup(s => s.MetaGenres.AddAsync(It.IsAny<MetaGenreDbModel>(), It.IsAny<CancellationToken>()))
            .Returns((MetaGenreDbModel metaGenre, CancellationToken cancellationToken) =>
            {
                var entry = EntityHelper.GetMockEntityEntry(metaGenre);
                metaGenreDbModels.Add(entry.Entity);
                return ValueTask.FromResult(entry);
            });
        
        sentenceDbModels ??= new();
        _context.Setup<DbSet<SentenceDbModel>>(s => s.Sentences)
            .ReturnsDbSet(sentenceDbModels.AsQueryable());
        _context.Setup(s => s.Sentences.AddAsync(It.IsAny<SentenceDbModel>(), It.IsAny<CancellationToken>()))
            .Returns((SentenceDbModel sentence, CancellationToken cancellationToken) =>
            {
                var entry = EntityHelper.GetMockEntityEntry(sentence);
                sentenceDbModels.Add(entry.Entity);
                return ValueTask.FromResult(entry);
            });
        
        wordDbModels ??= new();
        _context.Setup<DbSet<WordDbModel>>(s => s.Words)
            .ReturnsDbSet(wordDbModels.AsQueryable());
        _context.Setup(s => s.Words.AddAsync(It.IsAny<WordDbModel>(), It.IsAny<CancellationToken>()))
            .Returns((WordDbModel word, CancellationToken cancellationToken) =>
            {
                var entry = EntityHelper.GetMockEntityEntry(word);
                wordDbModels.Add(entry.Entity);
                return ValueTask.FromResult(entry);
            });
    }
}