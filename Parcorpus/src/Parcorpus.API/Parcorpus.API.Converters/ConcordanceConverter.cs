using Parcorpus.API.Dto;
using Parcorpus.Core.Models;

namespace Parcorpus.API.Converters;

public static class ConcordanceConverter
{
    public static ConcordanceDto ConvertAppModelToDto(Concordance concordance)
    {
        return new(concordance.SourceWord, concordance.AlignedWord, concordance.SourceText,
            concordance.AlignedTranslation, concordance.Title, concordance.Author, concordance.Source,
            concordance.CreationYear, concordance.AddDate);
    }
}