using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;
using Parcorpus.DB.Models;

namespace Parcorpus.UnitTests.Common.Factories.CoreModels;

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
    
    public static SearchHistoryRecord CreateFromQuery(int searchHistoryId = 0, 
        Guid? userId = null, 
        ConcordanceQuery? query = null,
        DateTime? queryTimestampUtc = null)
    {
        return new SearchHistoryRecord(searchHistoryId: searchHistoryId,
            userId: userId ?? Guid.Empty,
            word: query?.SourceWord.WordForm ?? "apple",
            sourceLanguageShortName: query?.SourceWord.Language.ShortName ?? "en",
            destinationLanguageShortName: query?.DestinationLanguage.ShortName ?? "ru",
            filters: query?.Filters ?? new Filter(),
            queryTimestampUtc: queryTimestampUtc ?? new DateTime(2023, 9, 1));
    }
    
    public static SearchHistoryRecord Create(SearchHistoryDbModel history)
    {
        return new SearchHistoryRecord(searchHistoryId: history.SearchHistoryId,
            userId: history.UserId,
            word: history.Query.Word,
            sourceLanguageShortName: history.Query.SourceLanguageShortName,
            destinationLanguageShortName: history.Query.DestinationLanguageShortName,
            filters: ConvertFilter(history.Query.Filter),
            queryTimestampUtc: history.QueryTimestampUtc);
    }
    
    private static Filter ConvertFilter(FilterDbModel filter)
    {
        return new Filter(filter?.Genre, filter?.StartYear, filter?.EndYear, filter?.Author);
    }
}