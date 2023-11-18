using Parcorpus.DataAccess.Models;

namespace Parcorpus.UnitTests.Common.Factories.DbModels;

public static class CountryDbModelFactory
{
    public static CountryDbModel Create(int countryId = default, string name = "Armenia")
    {
        return new CountryDbModel(countryId, name);
    }
}