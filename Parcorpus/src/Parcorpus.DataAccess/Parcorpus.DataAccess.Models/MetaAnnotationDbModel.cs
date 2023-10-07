using Parcorpus.DB.Models;

namespace Parcorpus.DataAccess.Models;

public class MetaAnnotationDbModel
{
    public int MetaId { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }

    public string Source { get; set; }

    public int CreationYear { get; set; }

    public DateTime AddDate { get; set; }

    public virtual ICollection<MetaGenreDbModel> MetaGenresNavigation { get; set; } = new List<MetaGenreDbModel>();

    public virtual TextDbModel TextNavigation { get; set; }

    public MetaAnnotationDbModel(int metaId, 
        string title, 
        string author, 
        string source, 
        int creationYear, 
        DateTime addDate)
    {
        MetaId = metaId;
        Title = title;
        Author = author;
        Source = source;
        CreationYear = creationYear;
        AddDate = addDate;
    }

    public MetaAnnotationDbModel()
    {
    }
}