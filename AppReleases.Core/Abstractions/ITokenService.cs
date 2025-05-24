using AppReleases.Core.Models;

namespace AppReleases.Core.Abstractions;

public interface ITokenService
{
    public Task<IEnumerable<Token>> GetAllTokensAsync(Guid userId);
    public Task<string> CreateUserTokenAsync(Guid userId, string name, DateTime expiresAt);
    public Task<string> CreateApplicationTokenAsync(Guid userId, string name, Guid applicationId, DateTime expiresAt);
    public Task RevokeTokenAsync(Guid tokenId);
    public Task<Token> GetTokenByIdAsync(Guid tokenId);
}