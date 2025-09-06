using System.Security.Claims;
using AppReleases.Core.Abstractions;
using AspNetCore.Authentication.Basic;
using Microsoft.Extensions.FileSystemGlobbing;

namespace AppReleases.Api.Helpers;

public class AuthorizationHelper(ITokenService tokenService, BasicAuthService basicAuthService)
{
    public bool VerifyAdmin(ClaimsPrincipal principal)
    {
        var tokenIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "tokenId");
        return tokenIdClaim is null && principal.Identity?.Name == basicAuthService.Login;
    }

    public Task<bool> VerifyApplication(ClaimsPrincipal principal, Core.Models.Application application) =>
        VerifyApplication(principal, application.Name);

    public async Task<bool> VerifyApplication(ClaimsPrincipal principal, string applicationName)
    {
        var tokenIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "tokenId");
        if (tokenIdClaim is null && principal.Identity?.Name == basicAuthService.Login)
            return true;
        if (tokenIdClaim is null)
            return false;
        var token = await tokenService.GetTokenByIdAsync(Guid.Parse(tokenIdClaim.Value));
        var matcher = new Matcher();
        matcher.AddInclude(token.Mask);
        return matcher.Match(applicationName).HasMatches;
    }
}