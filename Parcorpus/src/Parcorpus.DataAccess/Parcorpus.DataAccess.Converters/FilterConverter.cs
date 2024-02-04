using Parcorpus.Core.Models;
using Parcorpus.DB.Models;

namespace Parcorpus.DataAccess.Converters;

public static class FilterConverter
{
    public static FilterDbModel ConvertAppModelToDbModel(Filter filter)
    {
        return new FilterDbModel(filter?.Genre, filter?.StartYear, filter?.EndYear, filter?.Author);
    }
    
    public static Filter ConvertDbModelToAppModel(FilterDbModel filter)
    {
        return new Filter(filter?.Genre, filter?.StartYear, filter?.EndYear, filter?.Author);
    }
}