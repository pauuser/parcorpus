namespace Parcorpus.Core.Models;

public sealed class Language
{
    public string ShortName { get; set; }

    public string FullEnglishName { get; set; }

    public Language(string shortName, string fullEnglishName)
    {
        ShortName = shortName;
        FullEnglishName = fullEnglishName;
    }
}