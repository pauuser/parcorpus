using Parcorpus.API.Dto;
using Parcorpus.Core.Models;

namespace Parcorpus.API.Converters;

public static class TextConverter
{
    public static TextDto ConvertAppModelToDto(Text text)
    {
        return new TextDto(textId: text.TextId, 
            title: text.Title, 
            author: text.Author, 
            source: text.Source, 
            creationYear: text.CreationYear, 
            addDate: text.AddDate, 
            sourceLanguage: text.SourceLanguage.ShortName,
            targetLanguage: text.TargetLanguage.ShortName);
    }

    public static FullTextDto ConvertFullTextToDto(Text text)
    {
        return new FullTextDto(text: ConvertAppModelToDto(text),
            sentences: text.Sentences.Select(SentenceConverter.ConvertAppModelToDto).ToList());
    }
}