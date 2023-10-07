namespace Parcorpus.Core.Models;

public sealed class Word
{
    public string WordForm { get; set; }

    public Language Language { get; set; }

    public Word(string wordForm, Language language)
    {
        WordForm = wordForm;
        Language = language;
    }
}