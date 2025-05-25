using System.Security.Claims;
using AppReleases.Core.Abstractions;

namespace AppReleases.Api.Helpers;

public class AuthorizationHelper(ITokenService tokenService)
{
    public async Task VerifyUser(ClaimsPrincipal principal)
    {
        var tokenIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "tokenId");
        if (tokenIdClaim == null)
        {
            
        }
        else
        {
            var tokenId = Guid.Parse(tokenIdClaim.Value);
            var token = await tokenService.GetTokenByIdAsync(tokenId);
        }
    }
}