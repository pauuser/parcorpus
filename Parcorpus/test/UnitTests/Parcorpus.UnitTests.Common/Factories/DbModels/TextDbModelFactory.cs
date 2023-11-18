using Parcorpus.DataAccess.Models;

namespace Parcorpus.UnitTests.Common.Factories.DbModels;

public static class TextDbModelFactory
{
    public static TextDbModel Create(int textId = default, 
        int metaAnnotation = 1, 
        int languagePair = 1, 
        Guid? addedBy = null)
    {
        return new TextDbModel(textId, metaAnnotation, languagePair, addedBy ?? Guid.Empty);
    }
}