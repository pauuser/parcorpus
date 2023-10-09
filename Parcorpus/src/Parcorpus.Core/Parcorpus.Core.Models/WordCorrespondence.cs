namespace Parcorpus.Core.Models;

public class WordCorrespondence : IEquatable<WordCorrespondence>
{
    public Word SourceWord { get; set; }

    public Word AlignedWord { get; set; }

    public WordCorrespondence(Word sourceWord, Word alignedWord)
    {
        SourceWord = sourceWord;
        AlignedWord = alignedWord;
    }

    public WordCorrespondence()
    {
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(WordCorrespondence? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return SourceWord.Equals(other.SourceWord) && 
               AlignedWord.Equals(other.AlignedWord);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SourceWord, AlignedWord);
    }
}