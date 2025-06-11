using System.Security.Claims;
using AppReleases.Core.Abstractions;
using AspNetCore.Authentication.Basic;

namespace AppReleases.Api.Helpers;

public class AuthorizationHelper(ITokenService tokenService)
{
    public async Task<bool> VerifyUser(ClaimsPrincipal principal)
    {
        var tokenIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "tokenId");
        if (tokenIdClaim == null)
        {
            return true;
        }
        else
        {
            var tokenId = Guid.Parse(tokenIdClaim.Value);
            var token = await tokenService.GetTokenByIdAsync(tokenId);
            if (token.ExpiresAt < DateTime.UtcNow || token.RevokedAt < DateTime.UtcNow)
                return false;
            return token.ApplicationId == null;
        }
    }

    public async Task<bool> VerifyUser(ClaimsPrincipal principal, Guid applicationId)
    {
        var tokenIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "tokenId");
        if (tokenIdClaim == null)
        {
            return true;
        }
        else
        {
            var tokenId = Guid.Parse(tokenIdClaim.Value);
            var token = await tokenService.GetTokenByIdAsync(tokenId);
            if (token.ExpiresAt < DateTime.UtcNow || token.RevokedAt < DateTime.UtcNow)
                return false;
            return token.ApplicationId == null || token.ApplicationId == applicationId;
        }
    }
}