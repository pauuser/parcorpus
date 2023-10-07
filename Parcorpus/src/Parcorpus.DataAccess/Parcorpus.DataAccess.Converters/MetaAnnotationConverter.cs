using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.DataAccess.Converters;

public static class MetaAnnotationConverter
{
    public static MetaAnnotationDbModel ConvertAppModelToDbModel(MetaAnnotation meta)
    {
        return new MetaAnnotationDbModel(default, meta.Title, meta.Author, meta.Source, meta.CreationYear, meta.AddDate);
    }
    
    public static MetaAnnotation ConvertDbModelToAppModel(MetaAnnotationDbModel meta)
    {
        return new MetaAnnotation(meta.Title, meta.Author, meta.Source, meta.CreationYear, meta.AddDate);
    }
}