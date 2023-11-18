using System.Text;
using Newtonsoft.Json;
using Parcorpus.API.Dto;

namespace Parcorpus.IntegrationTests.Common;

public static class TokenHelper
{
    public static async Task<TokensDto> GetDefaultUserTokens(this HttpClient client)
    {
        var request = new Dictionary<string, string>
        {
            { "email", "inbox@mail.ru" },
            { "password", "123" }
        };
        var payload = JsonConvert.SerializeObject(request);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var tokensResponse = await client.PostAsync("/api/v1/auth/login", content);
        
        return System.Text.Json.JsonSerializer
            .Deserialize<TokensDto>(await tokensResponse.Content.ReadAsStringAsync())!;
    }
    
    public static async Task<Guid> GetUserId(this HttpClient client, string access_token)
    {
        var response = await client.GetAsync("/api/v1/users/me");
        
        return System.Text.Json.JsonSerializer
            .Deserialize<UserDto>(await response.Content.ReadAsStringAsync())!.UserId;
    }
}