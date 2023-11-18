using Parcorpus.DataAccess.Models;

namespace Parcorpus.UnitTests.Common.Factories.DbModels;

public static class GenreDbModelFactory
{
    public static GenreDbModel Create(int genreId = default, string name = "Drama")
    {
        return new GenreDbModel(genreId, name);
    }
}