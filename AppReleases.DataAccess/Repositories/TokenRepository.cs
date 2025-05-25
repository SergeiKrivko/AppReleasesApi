using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;
using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess.Repositories;

public class TokenRepository(AppReleasesDbContext dbContext) : ITokenRepository
{
    public async Task<IEnumerable<Token>> GetAllTokensAsync()
    {
        var entities = await dbContext.Tokens
            .Where(x => x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow)
            .ToArrayAsync();
        return entities.Select(TokenFromEntity);
    }

    public async Task<Token> GetTokenByIdAsync(Guid tokenId)
    {
        var entity = await dbContext.Tokens.FindAsync(tokenId);
        if (entity == null)
            throw new InvalidOperationException("Token not found");
        return TokenFromEntity(entity);
    }

    public async Task<Token> CreateTokenAsync(Token token)
    {
        var entity = EntityFromToken(token);
        await dbContext.Tokens.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return token;
    }

    public async Task MarkTokenAsRevokedAsync(Guid tokenId)
    {
        await dbContext.Tokens
            .Where(x => x.TokenId == tokenId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(o => o.RevokedAt, DateTime.UtcNow)
            );
        await dbContext.SaveChangesAsync();
    }

    private static Token TokenFromEntity(TokenEntity entity)
    {
        return new Token
        {
            Id = entity.TokenId,
            IssuedAt = entity.IssuedAt,
            ExpiresAt = entity.ExpiresAt,
            RevokedAt = entity.RevokedAt,
            Name = entity.Name,
            ApplicationId = entity.ApplicationId,
        };
    }

    private static TokenEntity EntityFromToken(Token token)
    {
        return new TokenEntity
        {
            TokenId = token.Id,
            IssuedAt = token.IssuedAt,
            ExpiresAt = token.ExpiresAt,
            RevokedAt = token.RevokedAt,
            Name = token.Name,
            ApplicationId = token.ApplicationId,
        };
    }
}