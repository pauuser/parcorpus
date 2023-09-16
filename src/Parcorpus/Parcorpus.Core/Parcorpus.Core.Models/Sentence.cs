namespace Parcorpus.Core.Models;

public sealed class Sentence
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
}