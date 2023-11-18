using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Parcorpus.DataAccess.Context;
using Parcorpus.DataAccess.Models;
using Parcorpus.Services.Helpers;
using Parcorpus.UnitTests.Common.Factories.CoreModels;
using Parcorpus.UnitTests.Common.Factories.DbModels;

namespace Parcorpus.IntegrationTests.Fixtures;

public static class DataInitializer
{
    public static Guid DefaultUserId { get; set; }

    public static int DefaultTextId { get; set; }

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
        } 
        while (country is null);
        user.CountryNavigation = country;
        
        LanguageDbModel language = null;
        do
        {
            language = await context.Languages.FirstOrDefaultAsync(l => l.LanguageId == user.NativeLanguage);
            await Task.Delay(300);
        } 
        while (language is null);
        
        user.NativeLanguageNavigation = language;
        var addedUser = (await context.Users.AddAsync(user)).Entity;
        DefaultUserId = addedUser.UserId;

        await context.SaveChangesAsync();

        var rawCredentials = CredentialsDbModelFactory
            .Create(userId: DefaultUserId, refreshToken: "1");
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

        var history = SearchHistoryDbModelFactory.Create(userId: DefaultUserId);
        history.UserNavigation = addedUser;
        await context.SearchHistory.AddAsync(history);
        
        var rawText = TextDbModelFactory.Create(metaAnnotation: meta.MetaId, languagePair: languagePair.LanguagePairId, addedBy: addedUser.UserId);
        var addedText = (await context.Texts.AddAsync(rawText)).Entity;
        
        await context.SaveChangesAsync();
        
        DefaultTextId = addedText.TextId;

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

    public static async Task InitStaticData(IServiceProvider serviceCollection)
    {
        var scope = serviceCollection.GetService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetService<ParcorpusDbContext>();
        
        var countriesCount = await context.Countries.CountAsync();
        var languagesCount = await context.Languages.CountAsync();

        if (countriesCount == 0)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Countries.csv");
            
            int i = 1;
            var countries = File.ReadAllLines(path)
                .Skip(1)
                .Select(c =>
                {
                    var splitString = c.Split(',');
                    return new CountryDbModel(i++, splitString[1]);
                })
                .ToList();
            
            foreach (var country in countries)
            {
                if (await context.Countries.FirstOrDefaultAsync(c => c.CountryId == country.CountryId) is null)
                {
                    await context.Countries.AddAsync(country);
                }
            }
            await context.SaveChangesAsync();
        }

        if (languagesCount == 0)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Languages.csv");
            int i = 1;
            var languages = File.ReadAllLines(path)
                .Skip(1)
                .Select(c =>
                {
                    var splitString = c.Split(',');
                    return new LanguageDbModel(i++, splitString[1], splitString[2]);
                })
                .ToList();
            
            foreach (var language in languages)
            {
                if (await context.Languages.FirstOrDefaultAsync(c => c.LanguageId == language.LanguageId) is null)
                {
                    await context.Languages.AddAsync(language);
                }
            }
            
            await context.Languages.AddRangeAsync(languages);
            await context.SaveChangesAsync();
        }
    }
}