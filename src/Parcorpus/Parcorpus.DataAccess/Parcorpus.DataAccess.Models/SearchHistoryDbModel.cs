namespace Parcorpus.DataAccess.Models;

public class SearchHistoryDbModel
{
    public int SearchHistoryId { get; set; }
    
    public Guid UserId { get; set; }

    public UserDbModel UserNavigation { get; set; }
    
    public HistoryJsonDbModel Query { get; set; }

    public DateTime QueryTimestampUtc { get; set; }

    public SearchHistoryDbModel(int searchHistoryId, Guid userId, HistoryJsonDbModel query, DateTime queryTimestampUtc)
    {
        SearchHistoryId = searchHistoryId;
        UserId = userId;
        Query = query;
        QueryTimestampUtc = queryTimestampUtc;
    }
}