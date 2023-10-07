namespace Parcorpus.Core.Models;

public sealed class Country : IEquatable<Country>
{
    public string CountryName { get; set; }

    public Country(string countryName)
    {
        CountryName = countryName;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(Country? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return CountryName == other.CountryName;
    }

    public override int GetHashCode()
    {
        return CountryName.GetHashCode();
    }
}