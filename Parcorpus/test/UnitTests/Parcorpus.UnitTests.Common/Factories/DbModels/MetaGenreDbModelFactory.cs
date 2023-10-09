using Parcorpus.DataAccess.Models;

namespace Parcorpus.UnitTests.Common.Factories.DbModels;

public static class MetaGenreDbModelFactory
{
    public static MetaGenreDbModel Create(int mgId = 1, int metaId = 1, int genreId = 1)
    {
        return new MetaGenreDbModel(mgId, metaId, genreId);
    }
}