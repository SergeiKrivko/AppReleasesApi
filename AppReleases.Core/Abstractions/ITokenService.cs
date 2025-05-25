using AppReleases.Core.Models;

namespace AppReleases.Core.Abstractions;

public interface ITokenService
{
    public Task<IEnumerable<Token>> GetAllTokensAsync();
    public Task<string> CreateUserTokenAsync(string name, DateTime expiresAt);
    public Task<string> CreateApplicationTokenAsync(string name, Guid applicationId, DateTime expiresAt);
    public Task RevokeTokenAsync(Guid tokenId);
    public Task<Token> GetTokenByIdAsync(Guid tokenId);
}