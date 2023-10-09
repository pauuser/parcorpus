using Parcorpus.DB.Models;

namespace Parcorpus.DataAccess.Models;

public class HistoryJsonDbModel : IEquatable<HistoryJsonDbModel>
{
    public string Word { get; set; }

    public string SourceLanguageShortName { get; set; }

    public string DestinationLanguageShortName { get; set; }

    public FilterDbModel Filter { get; set; }

    public HistoryJsonDbModel(string word, 
        string sourceLanguageShortName, 
        string destinationLanguageShortName, 
        FilterDbModel filter)
    {
        Word = word;
        SourceLanguageShortName = sourceLanguageShortName;
        DestinationLanguageShortName = destinationLanguageShortName;
        Filter = filter;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(HistoryJsonDbModel? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other))
            return true;
        
        return Word == other.Word && 
               SourceLanguageShortName == other.SourceLanguageShortName && 
               DestinationLanguageShortName == other.DestinationLanguageShortName && 
               Filter.Equals(other.Filter);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Word, SourceLanguageShortName, DestinationLanguageShortName, Filter);
    }
}