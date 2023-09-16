using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;

namespace Parcorpus.Services.AnnotationService;

public class AnnotationService : IAnnotationService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<AnnotationService> _logger;
    private readonly WebAlignerConfiguration _webAlignerConfiguration;
    private readonly IWordAligner _wordAligner;

    public AnnotationService(IHttpClientFactory clientFactory, 
        ILogger<AnnotationService> logger, 
        IWordAligner aligner,
        IOptions<WebAlignerConfiguration> configuration)
    {
        _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _wordAligner = aligner ?? throw new ArgumentNullException(nameof(aligner));
        _webAlignerConfiguration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<List<Sentence>> AlignSentences(BiText biText)
    {
        var sentences = new ConcurrentBag<Sentence>();
        var languageToTextDictionary = new Dictionary<Language, string>
        {
            { biText.SourceLanguage, biText.SourceText },
            { biText.TargetLanguage, biText.TargetText }
        };
        var alignedSentences = await AlignSentences(languageToTextDictionary);
        await Parallel.ForEachAsync(alignedSentences, new ParallelOptions(), async (sentence, _) =>
        {
            var sourceText = sentence[biText.SourceLanguage.ShortName];
            var targetText = sentence[biText.TargetLanguage.ShortName];

            var wordsAligned = await _wordAligner.AlignWords(sourceText: sourceText, 
                targetText: targetText, 
                sourceLanguage: biText.SourceLanguage, 
                targetLanguage: biText.TargetLanguage);
            
            sentences.Add(new Sentence(sentenceId: default,
                sourceText: sourceText,
                alignedTranslation: targetText,
                words: wordsAligned));
        });

        return sentences.ToList();
    }

    private async Task<List<Dictionary<string, string>>> AlignSentences(Dictionary<Language, string> languageData)
    {
        var dataForDownload = GetDataFromInput(languageData);
        
        return await DownloadResult(dataForDownload);
    }

    private List<KeyValuePair<string, string>> GetDataFromInput(Dictionary<Language, string> languageData)
    {
        var data = new List<KeyValuePair<string, string>>
        {
            new("aligner_direct", _webAlignerConfiguration.DefaultAligner),
            new("sessionId", "")
        };

        var idx = 1;
        foreach (var (language, text) in languageData)
        {
            var langContent = ClearAllTags(text);
            var langName = language.FullEnglishName;

            data.Add(new KeyValuePair<string, string>($"l{idx}", langContent));
            data.Add(new KeyValuePair<string, string>($"l{idx}Language", $"{langName}+({language.ShortName})"));

            idx++;
        }

        return data;
    }

    private async Task<List<Dictionary<string, string>>> DownloadResult(List<KeyValuePair<string, string>> data)
    {
        var url = GetUrl("align_text");
        var content = new FormUrlEncodedContent(data);
        
        var client = _clientFactory.CreateClient();
        try
        {
            var response = await client.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();
            
            return await ParseTmx(result);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Invalid URL {url} for sentence alignment", url);
            throw new InvalidUrlException($"Invalid URL {url} for sentence alignment", ex);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Operation canceled");
            throw new TimeoutException("Too much time for operation", ex);
        }
    }
    
    private async Task<List<Dictionary<string, string>>> ParseTmx(string tmxContent)
    {
        var table = new List<Dictionary<string, string>>();

        var page = await DownloadResultPage(tmxContent);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(page);
        
        var rows = htmlDocument.DocumentNode.SelectNodes("//tu");
    
        foreach (var row in rows)
        {
            var rowDict = new Dictionary<string, string>();
            var id = row.GetAttributeValue("tuid", string.Empty);
            rowDict["id"] = id;
            foreach (var cell in row.SelectNodes(".//tuv"))
            {
                var lang = cell.GetAttributeValue("xml:lang", string.Empty);
                var text = cell.InnerText.Trim();
                
                rowDict[lang] = text;
            }
            table.Add(rowDict);
        }
    
        return table;
    }

    private async Task<string> DownloadResultPage(string tmxContent)
    {
        var client = _clientFactory.CreateClient();
        var url = $"{_webAlignerConfiguration.Server}/{tmxContent}";
        try
        {
            var page = await client.GetAsync(url);

            return await page.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Invalid URL {url} for getting tmx page", url);
            throw new InvalidUrlException($"Invalid URL {url} for getting tmx page", ex);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Operation canceled");
            throw new TimeoutException("Too much time for operation", ex);
        }
    }
    
    private string GetUrl(string endpoint)
    {
        return $"{_webAlignerConfiguration.Server}/{_webAlignerConfiguration.Path}/{_webAlignerConfiguration.Sitemap[endpoint]}";
    }

    private static string ClearAllTags(string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }
}