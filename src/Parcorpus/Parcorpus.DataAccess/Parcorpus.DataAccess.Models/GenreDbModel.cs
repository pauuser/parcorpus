using Parcorpus.DB.Models;

namespace Parcorpus.DataAccess.Models;

public class GenreDbModel
{
    public int GenreId { get; set; }

    public string Name { get; set; }

    public virtual ICollection<MetaGenreDbModel> MetaGenres { get; set; } = new List<MetaGenreDbModel>();

    public GenreDbModel(int genreId, string name)
    {
        GenreId = genreId;
        Name = name;
    }

    public GenreDbModel()
    {
    }
}