using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Parcorpus.DataAccess.Context;
using Parcorpus.DataAccess.Models;
using Parcorpus.Services.Helpers;
using Parcorpus.UnitTests.Common.Factories.DbModels;

namespace Parcorpus.E2ETests.Seed;

public static class DataSeeder
{
    public static async Task PopulateDatabase(IServiceProvider serviceCollection)
    {
        var scope = serviceCollection.GetService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetService<ParcorpusDbContext>();

        var user = UserDbModelFactory.Create(passwordHash: HashHelper.Sha256Hash("123"));
        CountryDbModel country = null;
        do
        {
            country = await context.Countries.FirstOrDefaultAsync(c => c.CountryId == user.Country);
            await Task.Delay(300);
        } while (country is null);

        user.CountryNavigation = country;

        LanguageDbModel language = null;
        do
        {
            language = await context.Languages.FirstOrDefaultAsync(l => l.LanguageId == user.NativeLanguage);
            await Task.Delay(300);
        } while (language is null);

        user.NativeLanguageNavigation = language;
        var addedUser = (await context.Users.AddAsync(user)).Entity;

        await context.SaveChangesAsync();

        var rawCredentials = CredentialsDbModelFactory
            .Create(userId: addedUser.UserId, refreshToken: "1");
        rawCredentials.UserNavigation = addedUser;
        var credentials = (await context.Credentials.AddAsync(rawCredentials)).Entity;

        // Adding initial text

        var genre = (await context.Genres.AddAsync(GenreDbModelFactory.Create())).Entity;

        var lp = LanguagePairDbModelFactory.Create();
        lp.FromLanguageNavigation = context.Languages.FirstOrDefault(l => l.LanguageId == lp.FromLanguage);
        lp.ToLanguageNavigation = context.Languages.FirstOrDefault(l => l.LanguageId == lp.ToLanguage);
        var languagePair = (await context.LanguagePairs.AddAsync(lp)).Entity;

        var rawMeta = MetaAnnotationDbModelFactory.Create();
        var meta = (await context.MetaAnnotations
            .AddAsync(rawMeta)).Entity;

        await context.SaveChangesAsync();

        var metaGenre = MetaGenreDbModelFactory.Create(metaId: meta.MetaId, genreId: genre.GenreId);
        metaGenre.MetaNavigation = meta;
        metaGenre.GenreNavigation = genre;
        await context.MetaGenres.AddAsync(metaGenre);

        var history = SearchHistoryDbModelFactory.Create(userId: addedUser.UserId);
        history.UserNavigation = addedUser;
        await context.SearchHistory.AddAsync(history);

        var rawText = TextDbModelFactory.Create(metaAnnotation: meta.MetaId, languagePair: languagePair.LanguagePairId,
            addedBy: addedUser.UserId);
        var addedText = (await context.Texts.AddAsync(rawText)).Entity;

        await context.SaveChangesAsync();

        var sentences = Enumerable.Range(1, 5).Select(async _ =>
            {
                var rawSentence = SentenceDbModelFactory.Create();
                rawSentence.TextNavigation = addedText;
                var s = (await context.AddAsync(rawSentence)).Entity;
                await context.SaveChangesAsync();

                return s;
            })
            .Select(t => t.Result)
            .ToList();

        var words = new List<WordDbModel>();
        foreach (var sentence in sentences)
        {
            var newWords = Enumerable.Range(1, 5)
                .Select(_ =>
                {
                    var word = WordDbModelFactory.Create(sourceWord: "apple", alignedWord: "яблоко",
                        sentence: sentence.SentenceId, sentenceModel: sentence);
                    word.SentenceNavigation = sentence;
                    return word;
                }).ToList();

            words.AddRange(newWords);
            sentence.WordsNavigation = newWords;
        }

        await context.Words.AddRangeAsync(words);
        await context.SaveChangesAsync();
    }
}