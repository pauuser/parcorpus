using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.UnitTests.Common.Factories.CoreModels;

public static class TextFactory
{
    private static readonly Language DefaultSourceLanguage = new(shortName: "ru", fullEnglishName: "Russian");
    private static readonly Language DefaultTargetLanguage = new(shortName: "en", fullEnglishName: "English");
    
    public static Text Create(int textId = 1, 
        string title = "Title", 
        string author = "Author", 
        string source = "Source", 
        int creationYear = 2023, 
        DateTime? addDate = null, 
        Language? sourceLanguage = null, 
        Language? targetLanguage = null,
        List<string>? genres = null,
        List<Sentence>? sentences = null,
        Guid? addedBy = null)
    {
        return new Text(textId, title, author, source, creationYear,
            addDate ?? new DateTime(2023, 9, 1),
            sourceLanguage ?? DefaultSourceLanguage,
            targetLanguage ?? DefaultTargetLanguage,
            genres ?? new(),
            sentences ?? new(),
            addedBy ?? Guid.Empty);
    }
    
    public static Text Create(TextDbModel text)
    {
        return new(textId: text.TextId,
            title: text.MetaAnnotationNavigation.Title,
            author: text.MetaAnnotationNavigation.Author,
            source: text.MetaAnnotationNavigation.Source,
            creationYear: text.MetaAnnotationNavigation.CreationYear,
            addDate: text.MetaAnnotationNavigation.AddDate,
            sourceLanguage: ConvertLanguage(text.LanguagePairNavigation.FromLanguageNavigation),
            targetLanguage: ConvertLanguage(text.LanguagePairNavigation.ToLanguageNavigation),
            genres: text.MetaAnnotationNavigation.MetaGenresNavigation.Select(mg => mg.GenreNavigation?.Name).ToList(),
            sentences: text.SentencesNavigation?.Select(s => ConvertSentence(sentence: s, 
                    sourceLanguage: text.LanguagePairNavigation.FromLanguageNavigation, 
                    targetLanguage: text.LanguagePairNavigation.ToLanguageNavigation)).ToList() ?? new List<Sentence>(),
            addedBy: text.AddedBy);
    }
    
    private static Language ConvertLanguage(LanguageDbModel languageDbModel)
    {
        return new Language(shortName: languageDbModel.ShortName,
            fullEnglishName: languageDbModel.FullName);
    }
    
    private static Sentence ConvertSentence(SentenceDbModel sentence, 
        LanguageDbModel sourceLanguage, 
        LanguageDbModel targetLanguage)
    {
        return new Sentence(sentenceId: sentence.SentenceId,
            sourceText: sentence.SourceText,
            alignedTranslation: sentence.AlignedTranslation,
            words: sentence.WordsNavigation.Select(w => 
                ConvertWordCorrespondence(w, sourceLanguage, targetLanguage)).ToList());
    }

    private static WordCorrespondence ConvertWordCorrespondence(WordDbModel word,
        LanguageDbModel sourceLanguage, 
        LanguageDbModel targetLanguage)
    {
        var sourceLanguageAppModel = ConvertLanguage(sourceLanguage);
        var targetLanguageAppModel = ConvertLanguage(targetLanguage);
        
        return new WordCorrespondence(sourceWord: new Word(word.SourceWord, sourceLanguageAppModel),
            alignedWord: new Word(word.AlignedWord, targetLanguageAppModel));
    }
}