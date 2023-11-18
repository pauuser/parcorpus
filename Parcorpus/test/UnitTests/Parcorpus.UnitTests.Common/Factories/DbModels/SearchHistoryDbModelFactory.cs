using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;
using Parcorpus.DB.Models;

namespace Parcorpus.UnitTests.Common.Factories.DbModels;

public static class SearchHistoryDbModelFactory
{
    public static SearchHistoryDbModel Create(SearchHistoryRecord history)
    {
        return new SearchHistoryDbModel(searchHistoryId: history.SearchHistoryId,
            userId: history.UserId,
            query: new HistoryJsonDbModel(word: history.Word,
                sourceLanguageShortName: history.SourceLanguageShortName,
                destinationLanguageShortName: history.DestinationLanguageShortName,
                filter: new FilterDbModel(genre: history.Filters.Genre,
                    startDateTime: history.Filters.StartDateTime,
                    endDateTime: history.Filters.EndDateTime,
                    author: history.Filters.Author)),
            queryTimestampUtc: history.QueryTimestampUtc);
    }
    
    public static SearchHistoryDbModel Create(int searchHistoryId = default, 
        Guid? userId = null, 
        HistoryJsonDbModel? query = null, 
        DateTime? queryTimestampUtc = null)
    {
        query ??= new HistoryJsonDbModel("", "", "", new FilterDbModel());
        
        return new SearchHistoryDbModel(searchHistoryId: searchHistoryId,
            userId: userId ?? Guid.Empty,
            query: new HistoryJsonDbModel(word: query.Word,
                sourceLanguageShortName: query.SourceLanguageShortName,
                destinationLanguageShortName: query.DestinationLanguageShortName,
                filter: new FilterDbModel(genre: query.Filter.Genre,
                    startDateTime: query.Filter.StartDateTime,
                    endDateTime: query.Filter.EndDateTime,
                    author: query.Filter.Author)),
            queryTimestampUtc: queryTimestampUtc ?? new DateTime(2023, 9, 1));
    }
}