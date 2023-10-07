using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;

namespace Parcorpus.Services.AnnotationService;

public class AnnotationService : IAnnotationService
{
    private readonly ISentenceAligner _sentenceAligner;
    private readonly IWordAligner _wordAligner;
    
    private readonly ILogger<AnnotationService> _logger;

    public AnnotationService(ILogger<AnnotationService> logger,
        IWordAligner wordAligner,
        ISentenceAligner sentenceAligner)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _wordAligner = wordAligner ?? throw new ArgumentNullException(nameof(wordAligner));
        _sentenceAligner = sentenceAligner ?? throw new ArgumentNullException(nameof(sentenceAligner));
    }

    public async Task<List<Sentence>> AlignSentencesWithWords(BiText biText)
    {
        var sentences = new ConcurrentBag<Sentence>();
        var languageToTextDictionary = new Dictionary<Language, string>
        {
            { biText.SourceLanguage, biText.SourceText },
            { biText.TargetLanguage, biText.TargetText }
        };
        var alignedSentences = await _sentenceAligner.AlignSentences(languageToTextDictionary);
        await Parallel.ForEachAsync(alignedSentences, new ParallelOptions(), async (sentence, _) =>
        {
            var sourceText = sentence[biText.SourceLanguage.ShortName];
            var targetText = sentence[biText.TargetLanguage.ShortName];

            var wordsAligned = await _wordAligner.AlignWords(sourceText: sourceText,
                targetText: targetText,
                sourceLanguage: biText.SourceLanguage,
                targetLanguage: biText.TargetLanguage);

            sentences.Add(new Sentence(sentenceId: default,
                sourceText: sourceText,
                alignedTranslation: targetText,
                words: wordsAligned));
        });

        return sentences.ToList();
    }
}