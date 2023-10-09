using Parcorpus.Core.Configuration;

namespace Parcorpus.UnitTests.Common.Factories.CoreModels;

public static class TokenConfigurationFactory
{
    public static TokenConfiguration Create(string issuer = "123123",
        string audience = "123123",
        string key = "E1PRVBD4-V70Z-5D36-3BD2-14F335465661",
        TimeSpan? expiresIn = null,
        TimeSpan? refreshTokenExpiresIn = null)
    {
        return new TokenConfiguration()
        {
            Issuer = issuer,
            Audience = audience,
            Key = key,
            ExpiresIn = expiresIn ?? TimeSpan.FromHours(1),
            RefreshTokenExpiresIn = refreshTokenExpiresIn ?? TimeSpan.FromDays(1)
        };
    }
}