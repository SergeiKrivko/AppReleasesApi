using AppReleases.Core.Models;

namespace AppReleases.Core.Abstractions;

public interface ITokenRepository
{
    public Task<IEnumerable<Token>> GetAllTokensAsync();
    public Task<Token> GetTokenByIdAsync(Guid tokenId);
    public Task<Token> CreateTokenAsync(Token token);
    public Task MarkTokenAsRevokedAsync(Guid tokenId);
}