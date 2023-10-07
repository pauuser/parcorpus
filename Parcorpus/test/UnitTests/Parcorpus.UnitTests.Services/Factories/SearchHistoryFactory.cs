using Parcorpus.Core.Models;

namespace Parcorpus.UnitTests.Services.Factories;

public static class SearchHistoryFactory
{
    public static SearchHistoryRecord Create(int searchHistoryId = 0, 
        Guid? userId = null, 
        string word = "", 
        string sourceLanguageShortName = "", 
        string destinationLanguageShortName = "", 
        Filter? filters = null,
        DateTime? queryTimestampUtc = null)
    {
        return new SearchHistoryRecord(searchHistoryId: searchHistoryId,
            userId: userId ?? Guid.Empty,
            word: word,
            sourceLanguageShortName: sourceLanguageShortName,
            destinationLanguageShortName: destinationLanguageShortName,
            filters: filters ?? new Filter(),
            queryTimestampUtc: queryTimestampUtc ?? new DateTime(2023, 9, 1));
    }
}