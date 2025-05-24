using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppReleases.Core.Abstractions;
using AppReleases.Core.Enums;
using AppReleases.Core.Models;
using Microsoft.IdentityModel.Tokens;

namespace AppReleases.Application.Services;

public class TokenService(ITokenRepository tokenRepository) : ITokenService
{
    public const string Issuer = "AppReleasesAPI";
    public const string Audience = "AppDeveloperAccessToken";

    private static string Key => Environment.GetEnvironmentVariable("TOKEN_SECRET") ?? "";

    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

    public Task<IEnumerable<Token>> GetAllTokensAsync(Guid userId)
    {
        return tokenRepository.GetAllTokensAsync(userId);
    }

    public Task<string> CreateUserTokenAsync(Guid userId, string name, DateTime expiresAt)
    {
        var id = Guid.NewGuid();

        return CreateToken([
            new Claim("TokenId", id.ToString()),
            new Claim("UserId", userId.ToString()),
        ], new Token
        {
            Id = id,
            Name = name,
            OwnerId = userId,
            Type = TokenType.User,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
        });
    }

    public Task<string> CreateApplicationTokenAsync(Guid userId, string name, Guid applicationId, DateTime expiresAt)
    {
        var id = Guid.NewGuid();

        return CreateToken([
            new Claim("TokenId", id.ToString()),
            new Claim("UserId", userId.ToString()),
            new Claim("ApplicationId", userId.ToString()),
        ], new Token
        {
            Id = id,
            Name = name,
            OwnerId = userId,
            Type = TokenType.Application,
            IssuedAt = DateTime.UtcNow,
            ApplicationId = applicationId,
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