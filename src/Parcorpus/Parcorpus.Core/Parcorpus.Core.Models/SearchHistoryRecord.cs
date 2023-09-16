namespace Parcorpus.Core.Models;

public sealed class SearchHistoryRecord
{
    public int SearchHistoryId { get; set; }

    public Guid UserId { get; set; }
    
    public string Word { get; set; }

    public string SourceLanguageShortName { get; set; }

    public string DestinationLanguageShortName { get; set; }

    public Filter Filters { get; set; }

    public DateTime QueryTimestampUtc { get; set; }

    public SearchHistoryRecord(int searchHistoryId, 
        Guid userId, 
        string word, 
        string sourceLanguageShortName, 
        string destinationLanguageShortName, 
        Filter filters,
        DateTime queryTimestampUtc)
    {
        SearchHistoryId = searchHistoryId;
        UserId = userId;
        Word = word;
        SourceLanguageShortName = sourceLanguageShortName;
        DestinationLanguageShortName = destinationLanguageShortName;
        Filters = filters;
        QueryTimestampUtc = queryTimestampUtc;
    }
}