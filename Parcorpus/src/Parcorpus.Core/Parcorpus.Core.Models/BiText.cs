namespace Parcorpus.Core.Models;

public sealed class BiText : IEquatable<BiText>
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

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(BiText? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return SourceText == other.SourceText && 
               TargetText == other.TargetText && 
               SourceLanguage.Equals(other.SourceLanguage) && 
               TargetLanguage.Equals(other.TargetLanguage) && 
               MetaAnnotation.Equals(other.MetaAnnotation) && 
               Genres.Equals(other.Genres);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SourceText, TargetText, SourceLanguage, TargetLanguage, MetaAnnotation, Genres);
    }
}