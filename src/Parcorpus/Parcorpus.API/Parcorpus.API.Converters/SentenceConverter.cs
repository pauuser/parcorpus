using Parcorpus.API.Dto;
using Parcorpus.Core.Models;

namespace Parcorpus.API.Converters;

public static class SentenceConverter
{
    public static SentenceDto ConvertAppModelToDto(Sentence sentence)
    {
        return new(sentenceId: sentence.SentenceId, 
            sourceText: sentence.SourceText, 
            alignedTranslation: sentence.AlignedTranslation,
            words: sentence.Words.Select(w => new WordPairDto(w.SourceWord.WordForm, w.AlignedWord.WordForm)).ToList());
    }
}