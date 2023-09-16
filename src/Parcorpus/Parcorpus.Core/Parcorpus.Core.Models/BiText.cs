namespace Parcorpus.Core.Models;

public sealed class BiText
{
    public string SourceText { get; set; }

    public string TargetText { get; set; }

    public Language SourceLanguage { get; set; }

    public Language TargetLanguage { get; set; }

    public MetaAnnotation MetaAnnotation { get; set; }

    public List<string> Genres { get; set; }

    public BiText(string sourceText, 
        string targetText, 
        Language sourceLanguage, 
        Language targetLanguage, 
        MetaAnnotation metaAnnotation, 
        List<string> genres)
    {
        SourceText = sourceText;
        TargetText = targetText;
        SourceLanguage = sourceLanguage;
        TargetLanguage = targetLanguage;
        MetaAnnotation = metaAnnotation;
        Genres = genres;
    }
}