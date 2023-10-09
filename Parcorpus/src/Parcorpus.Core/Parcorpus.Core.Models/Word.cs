namespace Parcorpus.Core.Models;

public sealed class Word : IEquatable<Word>
{
    public string WordForm { get; set; }

    public Language Language { get; set; }

    public Word(string wordForm, Language language)
    {
        WordForm = wordForm;
        Language = language;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(Word? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return WordForm == other.WordForm && Language.Equals(other.Language);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(WordForm, Language);
    }
}