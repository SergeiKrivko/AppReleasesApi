using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;
using Microsoft.IdentityModel.Tokens;

namespace AppReleases.Application.Services;

public class TokenService(ITokenRepository tokenRepository) : ITokenService
{
    public const string Issuer = "AppReleasesAPI";
    public const string Audience = "AppDeveloperAccessToken";

    private static string Key => Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "";

    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

    public Task<IEnumerable<Token>> GetAllTokensAsync()
    {
        return tokenRepository.GetAllTokensAsync();
    }

    public Task<string> CreateTokenAsync(string name, string mask, DateTime expiresAt)
    {
        var id = Guid.NewGuid();

        return CreateToken([
            new Claim("TokenId", id.ToString()),
            new Claim("Mask", mask),
        ], new Token
        {
            Id = id,
            Name = name,
            IssuedAt = DateTime.UtcNow,
            Mask = mask,
            ExpiresAt = expiresAt,
        });
    }

    private async Task<string> CreateToken(IEnumerable<Claim> claims, Token token)
    {
        var id = Guid.NewGuid();

        // создаем JWT-токен
        var jwt = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: token.ExpiresAt,
            signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256)
        );

        await tokenRepository.CreateTokenAsync(token);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public Task RevokeTokenAsync(Guid tokenId)
    {
        return tokenRepository.MarkTokenAsRevokedAsync(tokenId);
    }

    public Task<Token> GetTokenByIdAsync(Guid tokenId)
    {
        return tokenRepository.GetTokenByIdAsync(tokenId);
    }
}