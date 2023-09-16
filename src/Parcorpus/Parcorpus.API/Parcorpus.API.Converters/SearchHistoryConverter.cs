using Parcorpus.API.Dto;
using Parcorpus.Core.Models;

namespace Parcorpus.API.Converters;

public static class SearchHistoryConverter
{
    public static SearchHistoryDto ConvertAppModelToDto(SearchHistoryRecord historyRecord)
    {
        return new SearchHistoryDto(searchHistoryId: historyRecord.SearchHistoryId,
            word: historyRecord.Word,
            sourceLanguageShortName: historyRecord.SourceLanguageShortName,
            destinationLanguageShortName: historyRecord.DestinationLanguageShortName,
            filters: FilterConverter.ConvertAppModelToDto(historyRecord.Filters),
            queryTimestampUtc: historyRecord.QueryTimestampUtc);
    }
}