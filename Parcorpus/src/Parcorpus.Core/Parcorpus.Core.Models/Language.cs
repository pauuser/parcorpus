namespace Parcorpus.Core.Models;

public sealed class Language : IEquatable<Language>
{
    public string ShortName { get; set; }

    public string FullEnglishName { get; set; }

    public Language(string shortName, string fullEnglishName)
    {
        ShortName = shortName;
        FullEnglishName = fullEnglishName;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(Language? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return ShortName == other.ShortName && FullEnglishName == other.FullEnglishName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ShortName, FullEnglishName);
    }
}