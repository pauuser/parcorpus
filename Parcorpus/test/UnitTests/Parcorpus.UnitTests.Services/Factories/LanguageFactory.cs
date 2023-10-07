using Parcorpus.Core.Configuration;
using Parcorpus.Core.Models;

namespace Parcorpus.UnitTests.Services.Factories;

public static class LanguageFactory
{
    public static Language Create(string shortName, LanguagesConfiguration configuration)
    {
        return new Language(shortName, configuration.LanguagesForms[shortName]);
    }
}