namespace Parcorpus.Core.Models;

public class WordCorrespondence
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
}