namespace Parcorpus.Core.Models;

public sealed class SearchHistoryRecord : IEquatable<SearchHistoryRecord>
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

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(SearchHistoryRecord? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return SearchHistoryId == other.SearchHistoryId && 
               UserId.Equals(other.UserId) && 
               Word == other.Word && 
               SourceLanguageShortName == other.SourceLanguageShortName && 
               DestinationLanguageShortName == other.DestinationLanguageShortName && 
               Filters.Equals(other.Filters) && 
               QueryTimestampUtc.Equals(other.QueryTimestampUtc);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SearchHistoryId, UserId, Word, SourceLanguageShortName, DestinationLanguageShortName, 
            Filters, QueryTimestampUtc);
    }
}