using Parcorpus.Core.Models;

namespace Parcorpus.UnitTests.Common.Factories.CoreModels;

public static class FilterFactory
{
    public static Filter Create(string? genre = null, 
        DateTime? startDateTime = null, 
        DateTime? endDateTime = null, 
        string? author = null)
    {
        return new Filter(genre, startDateTime, endDateTime, author);
    }
}