using Parcorpus.DB.Models;

namespace Parcorpus.DataAccess.Models;

public class SentenceDbModel
{
    public int SentenceId { get; set; }

    public string SourceText { get; set; }

    public string AlignedTranslation { get; set; }

    public int SourceTextId { get; set; }

    public virtual TextDbModel TextNavigation { get; set; }

    public virtual ICollection<WordDbModel> WordsNavigation { get; set; } = new List<WordDbModel>();

    public SentenceDbModel(int sentenceId, 
        string sourceText, 
        string alignedTranslation, 
        int sourceTextId)
    {
        SentenceId = sentenceId;
        SourceText = sourceText;
        AlignedTranslation = alignedTranslation;
        SourceTextId = sourceTextId;
    }

    public SentenceDbModel()
    {
    }
}