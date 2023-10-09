using Parcorpus.DataAccess.Models;

namespace Parcorpus.UnitTests.Common.Factories.DbModels;

public static class GenreDbModelFactory
{
    public static GenreDbModel Create(int genreId = 1, string name = "Drama")
    {
        return new GenreDbModel(genreId, name);
    }
}