using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.Services.AnnotationService;
using Parcorpus.UnitTests.Common.Factories;
using Parcorpus.UnitTests.Common.Factories.CoreModels;
using Parcorpus.UnitTests.Common.Helpers;

namespace Parcorpus.UnitTests.Services;

public class AnnotationServiceUnitTests
{
    private readonly IAnnotationService _annotationService;
    
    private readonly Mock<IWordAligner> _mockWordAligner = new();
    private readonly Mock<ISentenceAligner> _mockSentenceAligner = new();
    private readonly LanguagesConfiguration _languagesConfiguration;

    public AnnotationServiceUnitTests()
    {
        var languagesConfiguration = ConfigurationHelper.InitConfiguration<LanguagesConfiguration>();
        _languagesConfiguration = languagesConfiguration.Value;
        
        _annotationService = new AnnotationService(logger: NullLogger<AnnotationService>.Instance,
            wordAligner: _mockWordAligner.Object,
            sentenceAligner: _mockSentenceAligner.Object);
    }
    
    [Fact]
    public async Task AlignSentencesMultipleOkTest()
    {
        // Arrange
        var sourceLanguage = LanguageFactory.Create("ru", _languagesConfiguration);
        var targetLanguage = LanguageFactory.Create("en", _languagesConfiguration);

        var expectedSentences = new List<Sentence>()
        {
            SentenceFactory.Create("Князь Андрей открыл окно.", "Prince Andrew opened the window.", 
                sourceLanguage, targetLanguage, new ()
            {
                KeyValuePair.Create<string, string>("Князь", "Prince"),
                KeyValuePair.Create<string, string>("Андрей", "Andrew"),
                KeyValuePair.Create<string, string>("открыл", "opened"),
                KeyValuePair.Create<string, string>("окно", "the window"),
            }),
            SentenceFactory.Create("Он улыбнулся.", "He smiled.", 
                sourceLanguage, targetLanguage, new()
            {
                KeyValuePair.Create<string, string>("Он", "He"),
                KeyValuePair.Create<string, string>("улыбнулся", "smiled")
            })
        };
        
        var sourceText = string.Join(" ", expectedSentences.Select(s => s.SourceText));
        var targetText = string.Join(" ", expectedSentences.Select(s => s.AlignedTranslation));

        _mockWordAligner.Setup(s => s.AlignWords(expectedSentences[0].SourceText, expectedSentences[0].AlignedTranslation, 
                sourceLanguage, targetLanguage)).ReturnsAsync(expectedSentences[0].Words);
        _mockWordAligner.Setup(s => s.AlignWords(expectedSentences[1].SourceText, expectedSentences[1].AlignedTranslation, 
                sourceLanguage, targetLanguage)).ReturnsAsync(expectedSentences[1].Words);
        _mockSentenceAligner.Setup(s => s.AlignSentences(new()
        {
            { sourceLanguage, sourceText },
            { targetLanguage, targetText }
        })).ReturnsAsync(expectedSentences.Select(s => new Dictionary<string, string>()
        {
            { sourceLanguage.ShortName, s.SourceText },
            { targetLanguage.ShortName, s.AlignedTranslation }
        }).ToList());

        var biText = BiTextFactory.Create(sourceText, targetText, sourceLanguage, targetLanguage);
        
        // Act
        var actualSentences = await _annotationService.AlignSentencesWithWords(biText);
        
        // Assert
        Assert.Equal(expectedSentences.OrderBy(s => s.SourceText), 
            actualSentences.OrderBy(s => s.SourceText));
    }
    
    [Fact]
    public async Task AlignSentencesSingleOkTest()
    {
        // Arrange
        var sourceLanguage = LanguageFactory.Create("ru", _languagesConfiguration);
        var targetLanguage = LanguageFactory.Create("en", _languagesConfiguration);

        var expectedSentences = new List<Sentence>()
        {
            SentenceFactory.Create("Князь Андрей открыл окно.", "Prince Andrew opened the window.", 
                sourceLanguage, targetLanguage, new ()
            {
                KeyValuePair.Create<string, string>("Князь", "Prince"),
                KeyValuePair.Create<string, string>("Андрей", "Andrew"),
                KeyValuePair.Create<string, string>("открыл", "opened"),
                KeyValuePair.Create<string, string>("окно", "the window"),
            })
        };
        
        var sourceText = string.Join(" ", expectedSentences.Select(s => s.SourceText));
        var targetText = string.Join(" ", expectedSentences.Select(s => s.AlignedTranslation));

        _mockWordAligner.Setup(s => s.AlignWords(expectedSentences[0].SourceText, expectedSentences[0].AlignedTranslation, 
                sourceLanguage, targetLanguage)).ReturnsAsync(expectedSentences[0].Words);
        _mockSentenceAligner.Setup(s => s.AlignSentences(new()
        {
            { sourceLanguage, sourceText },
            { targetLanguage, targetText }
        })).ReturnsAsync(expectedSentences.Select(s => new Dictionary<string, string>()
        {
            { sourceLanguage.ShortName, s.SourceText },
            { targetLanguage.ShortName, s.AlignedTranslation }
        }).ToList());

        var biText = BiTextFactory.Create(sourceText, targetText, sourceLanguage, targetLanguage);
        
        // Act
        var actualSentences = await _annotationService.AlignSentencesWithWords(biText);
        
        // Assert
        Assert.Equal(expectedSentences, actualSentences);
    }
}