using Parcorpus.API.Dto;
using Parcorpus.Core.Models;

namespace Parcorpus.API.Converters;

public static class TokenConverter
{
    public static TokensDto ConvertAppModelToDto(TokenPair tokens)
    {
        return new TokensDto(accessToken: tokens.AccessToken, refreshToken: tokens.RefreshToken);
    }
}