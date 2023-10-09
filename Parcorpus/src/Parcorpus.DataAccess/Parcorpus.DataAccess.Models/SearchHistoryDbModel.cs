namespace Parcorpus.DataAccess.Models;

public class SearchHistoryDbModel : IEquatable<SearchHistoryDbModel>
{
    public int SearchHistoryId { get; set; }
    
    public Guid UserId { get; set; }

    public virtual UserDbModel UserNavigation { get; set; }
    
    public HistoryJsonDbModel Query { get; set; }

    public DateTime QueryTimestampUtc { get; set; }

    public SearchHistoryDbModel(int searchHistoryId, Guid userId, HistoryJsonDbModel query, DateTime queryTimestampUtc)
    {
        SearchHistoryId = searchHistoryId;
        UserId = userId;
        Query = query;
        QueryTimestampUtc = queryTimestampUtc;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(SearchHistoryDbModel? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;

        return SearchHistoryId == other.SearchHistoryId &&
               UserId.Equals(other.UserId) &&
               Query.Equals(other.Query);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SearchHistoryId, UserId, UserNavigation, Query, QueryTimestampUtc);
    }
}