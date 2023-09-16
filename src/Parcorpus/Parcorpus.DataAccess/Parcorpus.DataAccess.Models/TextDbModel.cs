using Parcorpus.DB.Models;

namespace Parcorpus.DataAccess.Models;

public class TextDbModel
{
    public int TextId { get; set; }

    public int MetaAnnotation { get; set; }

    public int LanguagePair { get; set; }
    
    public Guid AddedBy { get; set; }

    public virtual UserDbModel AddedByNavigation { get; set; }

    public virtual LanguagePairDbModel LanguagePairNavigation { get; set; }

    public virtual MetaAnnotationDbModel MetaAnnotationNavigation { get; set; }

    public virtual ICollection<SentenceDbModel> SentencesNavigation { get; set; } = new List<SentenceDbModel>();

    public TextDbModel(int textId, 
        int metaAnnotation, 
        int languagePair, 
        Guid addedBy)
    {
        TextId = textId;
        MetaAnnotation = metaAnnotation;
        LanguagePair = languagePair;
        AddedBy = addedBy;
    }

    public TextDbModel(int textId, 
        int metaAnnotation, 
        int languagePair, 
        Guid addedBy, 
        UserDbModel addedByNavigation, 
        LanguagePairDbModel languagePairNavigation, 
        MetaAnnotationDbModel metaAnnotationNavigation)
    {
        TextId = textId;
        MetaAnnotation = metaAnnotation;
        LanguagePair = languagePair;
        AddedBy = addedBy;
        AddedByNavigation = addedByNavigation;
        LanguagePairNavigation = languagePairNavigation;
        MetaAnnotationNavigation = metaAnnotationNavigation;
    }

    public TextDbModel()
    {
    }
}