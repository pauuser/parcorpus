using Parcorpus.DB.Models;

namespace Parcorpus.DataAccess.Models;

public class CountryDbModel
{
    public int CountryId { get; set; }

    public string Name { get; set; }

    public virtual ICollection<UserDbModel> Users { get; set; } = new List<UserDbModel>();

    public CountryDbModel()
    {
    }

    public CountryDbModel(int countryId, string name)
    {
        CountryId = countryId;
        Name = name;
    }
}