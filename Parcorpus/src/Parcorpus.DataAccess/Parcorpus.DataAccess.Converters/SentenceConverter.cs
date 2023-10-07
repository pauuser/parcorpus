using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.DataAccess.Converters;

public static class SentenceConverter
{
    public static SentenceDbModel ConvertAppModelToDbModel(int textId, Sentence sentence)
    {
        return new SentenceDbModel(sentenceId: default, 
            sourceText: sentence.SourceText, 
            alignedTranslation: sentence.AlignedTranslation, 
            sourceTextId: textId);
    }

    public static Sentence ConvertDbModelToAppModel(SentenceDbModel sentence, 
        LanguageDbModel sourceLanguage, 
        LanguageDbModel targetLanguage)
    {
        return new Sentence(sentenceId: sentence.SentenceId,
            sourceText: sentence.SourceText,
            alignedTranslation: sentence.AlignedTranslation,
            words: sentence.WordsNavigation.Select(w => 
                ConvertWordDbModelToCorrespondence(w, sourceLanguage, targetLanguage)).ToList());
    }

    private static WordCorrespondence ConvertWordDbModelToCorrespondence(WordDbModel word,
        LanguageDbModel sourceLanguage, 
        LanguageDbModel targetLanguage)
    {
        var sourceLanguageAppModel = LanguageConverter.ConvertDbModelToAppModel(sourceLanguage);
        var targetLanguageAppModel = LanguageConverter.ConvertDbModelToAppModel(targetLanguage);
        
        return new WordCorrespondence(sourceWord: new Word(word.SourceWord, sourceLanguageAppModel),
            alignedWord: new Word(word.AlignedWord, targetLanguageAppModel));
    }
}