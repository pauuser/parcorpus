namespace Parcorpus.DataAccess.Models;

public class MetaGenreDbModel
{
    public int MgId { get; set; }

    public int MetaId { get; set; }

    public int GenreId { get; set; }

    public virtual GenreDbModel GenreNavigation { get; set; }

    public virtual MetaAnnotationDbModel MetaNavigation { get; set; }

    public MetaGenreDbModel(int mgId, int metaId, int genreId)
    {
        MgId = mgId;
        MetaId = metaId;
        GenreId = genreId;
    }

    public MetaGenreDbModel()
    {
    }
}