namespace Parcorpus.DataAccess.Models;

public class WordDbModel
{
    public int WordId { get; set; }

    public string SourceWord { get; set; }

    public string AlignedWord { get; set; }

    public int Sentence { get; set; }
    
    public virtual SentenceDbModel? SentenceNavigation { get; set; }

    public WordDbModel(int wordId, 
        string sourceWord, 
        string alignedWord, 
        int sentence)
    {
        WordId = wordId;
        SourceWord = sourceWord;
        AlignedWord = alignedWord;
        Sentence = sentence;
    }

    public WordDbModel()
    {
    }
}