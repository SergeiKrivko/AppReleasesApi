using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;
using AppReleases.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.DataAccess.Repositories;

public class TokenRepository(AppReleasesDbContext dbContext) : ITokenRepository
{
    public async Task<IEnumerable<Token>> GetAllTokensAsync(Guid userId)
    {
        var entities = await dbContext.Tokens.Where(e => e.UserId == userId).ToArrayAsync();
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
            OwnerId = entity.UserId,
            IssuedAt = entity.IssuedAt,
            ExpiresAt = entity.ExpiresAt,
            RevokedAt = entity.RevokedAt,
            Type = entity.Type,
            ApplicationId = entity.ApplicationId,
        };
    }

    private static TokenEntity EntityFromToken(Token token)
    {
        return new TokenEntity
        {
            TokenId = token.Id,
            UserId = token.OwnerId,
            IssuedAt = token.IssuedAt,
            ExpiresAt = token.ExpiresAt,
            RevokedAt = token.RevokedAt,
            Type = token.Type,
            ApplicationId = token.ApplicationId,
        };
    }
}