using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.DataAccess.Converters;

public static class SearchHistoryConverter
{
    public static SearchHistoryRecord ConvertDbModelToAppModel(SearchHistoryDbModel history)
    {
        return new SearchHistoryRecord(searchHistoryId: history.SearchHistoryId,
            userId: history.UserId,
            word: history.Query.Word,
            sourceLanguageShortName: history.Query.SourceLanguageShortName,
            destinationLanguageShortName: history.Query.DestinationLanguageShortName,
            filters: FilterConverter.ConvertDbModelToAppModel(history.Query.Filter),
            queryTimestampUtc: history.QueryTimestampUtc);
    }
}