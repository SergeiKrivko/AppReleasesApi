using AppReleases.Core.Models;

namespace AppReleases.Core.Abstractions;

public interface ITokenService
{
    public Task<IEnumerable<Token>> GetAllTokensAsync();
    public Task<string> CreateTokenAsync(string name, string mask, DateTime expiresAt);
    public Task RevokeTokenAsync(Guid tokenId);
    public Task<Token> GetTokenByIdAsync(Guid tokenId);
}