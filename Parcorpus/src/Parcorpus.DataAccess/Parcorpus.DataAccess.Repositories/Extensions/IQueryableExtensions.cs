using System.Text;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Primitives;
using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.DataAccess.Repositories.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<WordDbModel> ApplyFilters(this IIncludableQueryable<WordDbModel, UserDbModel> query, Filter filter)
    {
        var result = query.Where(_ => true);
        if (FilterPresent(filter.Author))
            result = result.Where(w =>
                w.SentenceNavigation.TextNavigation.MetaAnnotationNavigation.Author == filter.Author);
        
        if (FilterPresent(filter.Genre))
            result = result.Where(w => w.SentenceNavigation.TextNavigation.MetaAnnotationNavigation
                .MetaGenresNavigation.Any(g => g.GenreNavigation.Name == filter.Genre));
        
        if (FilterPresent(filter.StartDateTime) && FilterPresent(filter.EndDateTime))
        {
            result = result.Where(w =>
                filter.StartDateTime < new DateTime(w.SentenceNavigation.TextNavigation.MetaAnnotationNavigation.CreationYear, 6, 15) &&
                new DateTime(w.SentenceNavigation.TextNavigation.MetaAnnotationNavigation.CreationYear) < filter.EndDateTime);
        }

        return result;
    }
    
    private static bool FilterPresent(object? filter)
    {
        return filter is not null;
    }
}