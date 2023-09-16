namespace Parcorpus.Core.Models;

public sealed class UserName
{
    public string Name { get; set; }
    
    public string Surname { get; set; }

    public UserName(string name, string surname)
    {
        Name = name;
        Surname = surname;
    }
}