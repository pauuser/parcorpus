using Parcorpus.DB.Models;

namespace Parcorpus.DataAccess.Models;

public class HistoryJsonDbModel
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
}