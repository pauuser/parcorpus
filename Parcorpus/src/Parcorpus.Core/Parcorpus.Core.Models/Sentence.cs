namespace Parcorpus.Core.Models;

public sealed class Sentence : IEquatable<Sentence>
{
    public int SentenceId { get; set; }
    
    public string SourceText { get; set; }

    public string AlignedTranslation { get; set; }

    public List<WordCorrespondence> Words { get; set; }

    public Sentence(int sentenceId, string sourceText, string alignedTranslation, List<WordCorrespondence> words)
    {
        SentenceId = sentenceId;
        SourceText = sourceText;
        AlignedTranslation = alignedTranslation;
        Words = words;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(Sentence? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return SentenceId == other.SentenceId && 
               SourceText == other.SourceText && 
               AlignedTranslation == other.AlignedTranslation && 
               Words.SequenceEqual(other.Words);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SentenceId, SourceText, AlignedTranslation, Words);
    }
}