namespace Parcorpus.Core.Models;

public sealed class UserName : IEquatable<UserName>
{
    public string Name { get; set; }
    
    public string Surname { get; set; }

    public UserName(string name, string surname)
    {
        Name = name;
        Surname = surname;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(UserName? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return Name == other.Name && Surname == other.Surname;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Surname);
    }
}