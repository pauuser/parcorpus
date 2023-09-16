namespace Parcorpus.Core.Models;

public sealed class Country
{
    public string CountryName { get; set; }

    public Country(string countryName)
    {
        CountryName = countryName;
    }
}