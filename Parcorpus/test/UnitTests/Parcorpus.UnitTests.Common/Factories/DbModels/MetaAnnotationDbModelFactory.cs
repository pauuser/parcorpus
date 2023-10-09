using Parcorpus.DataAccess.Models;

namespace Parcorpus.UnitTests.Common.Factories.DbModels;

public static class MetaAnnotationDbModelFactory
{
    public static MetaAnnotationDbModel Create(int metaId = 1, 
        string title = "Title", 
        string author = "Author", 
        string source = "Source", 
        int creationYear = 2023, 
        DateTime? addDate = null)
    {
        return new MetaAnnotationDbModel(metaId, title, author, source, creationYear,
            addDate ?? new DateTime(2023, 9, 1));
    }
}