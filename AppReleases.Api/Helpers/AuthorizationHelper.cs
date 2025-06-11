using System.Security.Claims;
using AppReleases.Core.Abstractions;
using AspNetCore.Authentication.Basic;

namespace AppReleases.Api.Helpers;

public class AuthorizationHelper(ITokenService tokenService, BasicAuthService basicAuthService)
{
    public bool VerifyAdmin(ClaimsPrincipal principal)
    {
        var tokenIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "tokenId");
        return tokenIdClaim is null && principal.Identity?.Name == basicAuthService.Login;
    }
}