using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Context;
using Parcorpus.DataAccess.Converters;
using Parcorpus.DataAccess.Models;
using Parcorpus.DataAccess.Repositories.Extensions;

namespace Parcorpus.DataAccess.Repositories;

public class LanguageRepository : BaseRepository<LanguageRepository>, ILanguageRepository
{
    private readonly ParcorpusDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly CacheConfiguration _cacheConfiguration;

    public LanguageRepository(ILogger<LanguageRepository> logger, 
        ParcorpusDbContext context,
        IMemoryCache cache,
        IOptions<CacheConfiguration> cacheConfiguration) : base(logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheConfiguration = cacheConfiguration.Value ?? throw new ArgumentNullException(nameof(cacheConfiguration));
    }

    public async Task<List<Concordance>> GetConcordance(Word word, Language desiredLanguage, Filter filter)
    {
        try
        {
            var cacheKey = GetConcordanceCacheKey(word, desiredLanguage, filter);
            if (_cache.TryGetValue(cacheKey, out var cachedValue))
                return (List<Concordance>) cachedValue!;
            
            var concordance = _context.Words
                .Where(w => w.SourceWord == word.WordForm)
                .Include(w => w.SentenceNavigation)
                    .ThenInclude(s => s.TextNavigation)
                        .ThenInclude(t => t.MetaAnnotationNavigation)
                            .ThenInclude(m => m.MetaGenresNavigation)
                                .ThenInclude(mg => mg.GenreNavigation)
                .Include(w => w.SentenceNavigation)
                    .ThenInclude(s => s.TextNavigation)
                        .ThenInclude(t => t.LanguagePairNavigation)
                            .ThenInclude(lp => lp.FromLanguageNavigation)
                .Where(w => w.SentenceNavigation.TextNavigation.LanguagePairNavigation.FromLanguageNavigation.ShortName == word.Language.ShortName)
                .Include(w => w.SentenceNavigation)
                    .ThenInclude(s => s.TextNavigation)
                        .ThenInclude(lp => lp.LanguagePairNavigation)
                            .ThenInclude(lp => lp.ToLanguageNavigation)
                .Where(w => w.SentenceNavigation.TextNavigation.LanguagePairNavigation.ToLanguageNavigation.ShortName == desiredLanguage.ShortName)
                .Include(w => w.SentenceNavigation)
                    .ThenInclude(s => s.TextNavigation)
                        .ThenInclude(t => t.AddedByNavigation);

            var filtered = await concordance.ApplyFilters(filter).ToListAsync();
            var result = filtered.Select(ConcordanceConverter.ConvertWordToConcordance).ToList();

            _cache.Set(cacheKey, result, absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(_cacheConfiguration.ConcordanceExpirationMinutes));

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during getting concordance");
            throw new LanguageRepositoryException("Error during getting concordance", ex);
        }
    }

    private string GetConcordanceCacheKey(Word word, Language desiredLanguage, Filter filter)
    {
        var sb = new StringBuilder();

        sb.AppendJoin("_", word.WordForm, word.Language.ShortName, desiredLanguage.ShortName,
            filter.Author, filter.StartDateTime, filter.EndDateTime, filter.Genre);

        return sb.ToString();
    }

    public async Task<Text> GetTextById(int textId)
    {
        try
        {
            if (_cache.TryGetValue(textId, out var cachedValue))
                return (Text) cachedValue!;
            
            var text = await _context.Texts
                .Where(t => t.TextId == textId)
                .Include(t => t.MetaAnnotationNavigation)
                    .ThenInclude(m => m.MetaGenresNavigation)
                        .ThenInclude(mg => mg.GenreNavigation)
                .Include(t => t.LanguagePairNavigation)
                    .ThenInclude(lp => lp.FromLanguageNavigation)
                .Include(lp => lp.LanguagePairNavigation)
                    .ThenInclude(lp => lp.ToLanguageNavigation)
                .Include(t => t.AddedByNavigation)
                .Include(t => t.SentencesNavigation)
                    .ThenInclude(s => s.WordsNavigation)
                .FirstOrDefaultAsync();

            if (text is null)
            {
                Logger.LogError("Text with id = {textId} is not found", textId);
                throw new NotFoundException($"Text with id = {textId} is not found");
            }
            
            Logger.LogInformation("Successfully retrieved text with id = {id}", textId);

            var result = TextConverter.ConvertDbModelToAppModel(text);
            _cache.Set(textId, result);
            
            return result;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during getting text by id");
            throw new LanguageRepositoryException("Error during getting text by id", ex);
        }
    }

    public async Task<List<Text>> GetTextsAddedByUser(Guid userId)
    {
        try
        {
            if (_cache.TryGetValue(userId, out var cachedValue))
                return (List<Text>) cachedValue!;
            
            var texts = await _context.Texts
                .Include(t => t.AddedByNavigation)
                .Where(t => t.AddedByNavigation!.UserId == userId)
                .Include(t => t.MetaAnnotationNavigation)
                    .ThenInclude(m => m.MetaGenresNavigation)
                        .ThenInclude(mg => mg.GenreNavigation)
                .Include(t => t.LanguagePairNavigation)
                    .ThenInclude(lp => lp.FromLanguageNavigation)
                .Include(lp => lp.LanguagePairNavigation)
                    .ThenInclude(lp => lp.ToLanguageNavigation)
                .Include(t => t.SentencesNavigation)
                    .ThenInclude(s => s.WordsNavigation)
                .ToListAsync();
            
            Logger.LogInformation("Successfully retrieved {count} texts for userId = {userId}", texts.Count, userId);

            var result = texts.Select(TextConverter.ConvertDbModelToAppModel).ToList();
            _cache.Set(userId, result);
            
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during getting text by for userId");
            throw new LanguageRepositoryException("Error during getting text by for userId", ex);
        }
    }

    public async Task AddAlignedText(Guid userId, Text alignedText)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var sourceLanguage = alignedText.SourceLanguage;
            var targetLanguage = alignedText.TargetLanguage;
            var languagePair = await GetOrCreateLanguagePair(sourceLanguage, targetLanguage);

            var meta = await CreateMetaAnnotation(new MetaAnnotationDbModel(metaId: default,
                title: alignedText.Title, 
                author: alignedText.Author, 
                source: alignedText.Source, 
                creationYear: alignedText.CreationYear,
                addDate: DateTime.UtcNow));
            var text = await CreateText(userId, meta, languagePair);
            await CreateGenresToMeta(alignedText.Genres, meta);

            await CreateSentences(sentences: alignedText.Sentences, text: text);
            await transaction.CommitAsync();
            
            if (_cache.TryGetValue(userId, out _))
                _cache.Remove(userId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Logger.LogError(ex, "Error during adding aligned text");
            throw new LanguageRepositoryException("Error during adding aligned text", ex);
        }
    }

    private async Task<LanguagePairDbModel> GetOrCreateLanguagePair(Language sourceLanguage, Language targetLanguage)
    {
        try
        {
            var sourceLanguageEntity = await _context.Languages.FirstOrDefaultAsync(l => l.ShortName == sourceLanguage.ShortName);
            var targetLanguageEntity = await _context.Languages.FirstOrDefaultAsync(l => l.ShortName == targetLanguage.ShortName);

            if (sourceLanguageEntity is null || targetLanguageEntity is null)
            {
                Logger.LogError("Language pair with languages {source} & {target} cannot be created. " +
                    "One of the languages is invalid.", sourceLanguage, targetLanguage);
                throw new InvalidLanguageException($"Language pair with languages {sourceLanguage} & " +
                    $"{targetLanguage} cannot be created. One of the languages is invalid.");
            }

            var languagePair = await _context.LanguagePairs.FirstOrDefaultAsync(lp =>
                lp.FromLanguage == sourceLanguageEntity.LanguageId && lp.ToLanguage == targetLanguageEntity.LanguageId);
            if (languagePair is null)
            {
                Logger.LogInformation("Language pair for languages {source} & {target} does not exist. " +
                    "Language pair will be created.", sourceLanguage, targetLanguage);
                var addedPair = await _context.LanguagePairs.AddAsync(new LanguagePairDbModel(languagePairId: default,
                    fromLanguage: sourceLanguageEntity.LanguageId, toLanguage: targetLanguageEntity.LanguageId));
                
                await _context.SaveChangesAsync();
                Logger.LogInformation("Language pair {} for languages {source} & {target} is created", 
                    addedPair.Entity.LanguagePairId, sourceLanguage, targetLanguage);
                
                languagePair = addedPair.Entity;
            }
            
            Logger.LogInformation("Language pair with id = {id} is returned", languagePair.LanguagePairId);

            return languagePair;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during getting or creating language pair");
            throw new LanguageRepositoryException("Error during getting or creating language pair", ex);
        }
    }

    private async Task<MetaAnnotationDbModel> CreateMetaAnnotation(MetaAnnotationDbModel meta)
    {
        try
        {
            var addedMeta = await _context.MetaAnnotations.AddAsync(meta);
            var entity = addedMeta.Entity;

            await _context.SaveChangesAsync();
            Logger.LogInformation("Meta annotation is created with id = {id}", entity.MetaId);

            return entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during creating meta annotation");
            throw new LanguageRepositoryException("Error during creating meta annotation", ex);
        }
    }

    private async Task<TextDbModel> CreateText(Guid userId, MetaAnnotationDbModel meta, LanguagePairDbModel languagePair)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user is null)
            {
                Logger.LogError("User with id = {userId} is not found", userId);
                throw new NotFoundException($"User with id = {userId} is not found");
            }

            var added = await _context.Texts.AddAsync(new TextDbModel(textId: default, 
                metaAnnotation: meta.MetaId, 
                languagePair: languagePair.LanguagePairId, 
                addedBy: userId,
                addedByNavigation: user,
                languagePairNavigation: languagePair,
                metaAnnotationNavigation: meta));
            var entity = added.Entity;
            
            await _context.SaveChangesAsync();
            Logger.LogInformation("Text is created with id = {id}", entity.TextId);

            return entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during creating text");
            throw new LanguageRepositoryException("Error during creating text", ex);
        }
    }

    private async Task CreateGenresToMeta(List<string> genres, MetaAnnotationDbModel meta)
    {
        try
        {
            foreach (var genre in genres)
            {
                var genreDb = await GetOrCreateGenre(genre);
                await _context.MetaGenres.AddAsync(new MetaGenreDbModel(mgId: default, 
                    metaId: meta.MetaId, 
                    genreId: genreDb.GenreId));
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during creating genres to meta");
            throw new LanguageRepositoryException("Error during creating genres to meta", ex);
        }
    }

    private async Task<GenreDbModel> GetOrCreateGenre(string genre)
    {
        try
        {
            var existingGenre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genre);
            if (existingGenre is null)
            {
                Logger.LogInformation("Genre {genre} does not exist and will be created", genre);
                var added = await _context.Genres.AddAsync(new GenreDbModel(genreId: default, name: genre));
                await _context.SaveChangesAsync();
                
                existingGenre = added.Entity;
            }
            
            Logger.LogInformation("Genre with id = {id} is returned", existingGenre.GenreId);

            return existingGenre;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during getting or creating genre");
            throw new LanguageRepositoryException("Error during getting or creating genre", ex);
        }
    }

    private async Task CreateSentences(List<Sentence> sentences, TextDbModel text)
    {
        try
        {
            foreach (var sentence in sentences)
            {
                await CreateSentence(sentence, text);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during creating sentences");
            throw new LanguageRepositoryException("Error during creating sentences", ex);
        }
    }

    private async Task CreateSentence(Sentence sentence, TextDbModel text)
    {
        try
        {
            var sentenceDb = SentenceConverter.ConvertAppModelToDbModel(text.TextId, sentence);
            sentenceDb.TextNavigation = text;
            var entry = await _context.Sentences.AddAsync(sentenceDb);
            var addedSentence = entry.Entity;

            await _context.SaveChangesAsync();
            Logger.LogInformation("Sentence {id} for textId = {textId} is created", addedSentence.SentenceId, text.TextId);

            await CreateWords(sentenceId: addedSentence.SentenceId, words: sentence.Words);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during creating sentence");
            throw new LanguageRepositoryException("Error during creating sentence", ex);
        }
    }

    private async Task CreateWords(int sentenceId, List<WordCorrespondence> words)
    {
        try
        {
            foreach (var word in words)
            {
                await CreateWord(sentenceId, word);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during creating words");
            throw new LanguageRepositoryException("Error during creating words", ex);
        }
    }

    private async Task<WordDbModel> CreateWord(int sentenceId, WordCorrespondence word)
    {
        try
        {
            var addedWord = await _context.Words.AddAsync(new WordDbModel(wordId: default,
                sourceWord: word.SourceWord.WordForm.ToLower(),
                alignedWord: word.AlignedWord.WordForm.ToLower(),
                sentence: sentenceId));
            
            await _context.SaveChangesAsync();
            Logger.LogInformation("Word {id} for sentenceId = {sentenceId} is added", addedWord.Entity.WordId, sentenceId);
            
            return addedWord.Entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during creating a word");
            throw new LanguageRepositoryException("Error during creating a word", ex);
        }
    }

    public async Task DeleteTextById(Guid userId, int textId)
    {
        try
        {
            var text = await _context.Texts.FirstOrDefaultAsync(t => t.TextId == textId && t.AddedBy == userId);
            if (text is null)
            {
                Logger.LogError("Text with id = {textId} does not exist", textId);
                throw new NotFoundException($"Text with id = {textId} does not exist");
            }

            var meta = await _context.MetaAnnotations.FirstOrDefaultAsync(m => m.MetaId == text.MetaAnnotation);
            
            _context.Texts.Remove(text);
            _context.MetaAnnotations.Remove(meta!);

            await _context.SaveChangesAsync();
            
            if (_cache.TryGetValue(text.TextId, out _))
                _cache.Remove(text.TextId);
            
            Logger.LogInformation("Text with id = {textId} is deleted", textId);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during deleting text");
            throw new LanguageRepositoryException("Error during deleting text", ex);
        }
    }

    public async Task<bool> TextExists(MetaAnnotation metaAnnotation, Language sourceLanguage, Language targetLanguage)
    {
        try
        {
            var text = await _context.Texts
                .Include(t => t.MetaAnnotationNavigation)
                .Include(t => t.LanguagePairNavigation)
                    .ThenInclude(lp => lp.FromLanguageNavigation)
                .Include(t => t.LanguagePairNavigation)
                    .ThenInclude(lp => lp.ToLanguageNavigation)
                .FirstOrDefaultAsync(t => t.MetaAnnotationNavigation.Title.Trim() == metaAnnotation.Title.Trim() &&
                           t.MetaAnnotationNavigation.Author.Trim() == metaAnnotation.Author.Trim() &&
                           t.LanguagePairNavigation.FromLanguageNavigation!.ShortName == sourceLanguage.ShortName &&
                           t.LanguagePairNavigation.ToLanguageNavigation!.ShortName == targetLanguage.ShortName);

            return text is not null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during checking if text exists");
            throw new LanguageRepositoryException("Error during checking if text exists", ex);
        }
    }
}