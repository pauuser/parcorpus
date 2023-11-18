using System.Net;
using Parcorpus.API.Dto;

namespace Parcorpus.E2ETests.Tests;

public class UploadTextAndGetConcordanceTest : TestBase
{
    /*
     * E2E Test to test registration and concordance obtaining (basic usage of the application)
     *
     * First user registers in the systems and gets auth credentials
     * Then he searches for word "apple" translations with usage examples
     */
    [Fact]
    public void UploadTextAndGetConcordanceOkTest()
    {
        // User registration
        
        var registrationRequestBody = new Dictionary<string, string>
        {
            { "name", "login" },
            { "surname", "login" },
            { "email", "login@mail.ru" },
            { "country_name", "Russian Federation" },
            { "language_short_name", "en" },
            { "password", "login" }
        };

        HttpHost.Post
            .Url("/api/v1/auth/register")
            .Json(registrationRequestBody)
            .Send()
            .Response
                .AssertStatusCode(HttpStatusCode.OK)
                .AsJson
                    .CopyResponseTo(out TokensDto tokens);
        
        var accessToken = tokens.AccessToken;
        
        // As token is obtained, now we are getting concordance
        
        HttpHost.Get
            .Url("/api/v1/texts/concordance")
            .WithJwtToken(accessToken)
            .AddQuery("Word", "apple")
            .AddQuery("SourceLanguageShortName", "aa")
            .AddQuery("TargetLanguageShortName", "ab")
            .Send()
            .Response
                .AssertStatusCode(HttpStatusCode.OK)
                .AsJson
            .CopyResponseTo(out PagedDto<ConcordanceDto> concor)
                    .AssertThat<PagedDto<ConcordanceDto>>(c =>
                    {
                        Assert.NotEmpty(c.Items);
                        Assert.Contains("яблоко", c.Items.First().AlignedTranslation);
                    }); // proof that "apple" is actually "яблоко"
    }
}