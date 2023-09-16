using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Parcorpus.Core.Exceptions;

namespace Parcorpus.API.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpRequest request)
    {
        var authHeader = request.Headers["Authorization"];
        if (AuthenticationHeaderValue.TryParse(authHeader, out var headerValue))
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(headerValue.Parameter);
            var claim = token.Claims.First(c => c.Type == "userId").Value;
            
            return Guid.Parse(claim);
        }
        else
        {
            throw new NoTokenException("No token provided");
        }
    }
}