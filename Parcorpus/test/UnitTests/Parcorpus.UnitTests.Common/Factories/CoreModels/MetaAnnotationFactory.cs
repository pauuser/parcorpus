using Parcorpus.Core.Models;

namespace Parcorpus.UnitTests.Common.Factories.CoreModels;

public static class MetaAnnotationFactory
{
    public static MetaAnnotation Create(string title = "War and Peace",
        string author = "L. N. Tolstoy",
        string source = "Russian state library",
        int creationYear = 1867,
        DateTime? addDate = null)
    {
        return new MetaAnnotation(title, author, source, creationYear, 
            addDate ?? new DateTime(2023, 9, 1));
    }
}