using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Parcorpus.Core.Configuration;

namespace Parcorpus.UnitTests.Common.Helpers;

public static class JwtHelper
{
    public static string CreateJwt(Guid userId, TokenConfiguration jwtConfiguration, DateTime? expiresAtUtc = null)
    {
        var claims = new List<Claim>
        {
            new ("userId", userId.ToString())
        };
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var jwt = new JwtSecurityToken(issuer: jwtConfiguration.Issuer,
            audience: jwtConfiguration.Audience,
            claims: claims,
            expires: expiresAtUtc ?? DateTime.UtcNow.Add(jwtConfiguration.ExpiresIn),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}