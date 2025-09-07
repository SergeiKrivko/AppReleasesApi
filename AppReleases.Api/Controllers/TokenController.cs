using AppReleases.Api.Helpers;
using AppReleases.Api.Schemas;
using AppReleases.Core.Abstractions;
using AppReleases.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppReleases.Api.Controllers;

[ApiController]
[Route("api/v1/tokens")]
public class TokenController(AuthorizationHelper authorizationHelper, ITokenService tokenService) : Controller
{
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Basic")]
    public async Task<ActionResult<Token[]>> GetAllHandler()
    {
        if (!authorizationHelper.VerifyAdmin(User))
            return Unauthorized();
        return Ok(await tokenService.GetAllTokensAsync());
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Basic")]
    public async Task<ActionResult<string>> CreateHandler(CreateTokenSchema token)
    {
        if (!authorizationHelper.VerifyAdmin(User))
            return Unauthorized();
        var res = await tokenService.CreateTokenAsync(token.Name, token.Name,
            token.ExpiresAt ?? DateTime.Now.AddMonths(1));
        return Ok(res);
    }

    [HttpDelete("{tokenId:guid}")]
    [Authorize(AuthenticationSchemes = "Basic")]
    public async Task<ActionResult> DeleteHandler(Guid tokenId)
    {
        if (!authorizationHelper.VerifyAdmin(User))
            return Unauthorized();
        await tokenService.RevokeTokenAsync(tokenId);
        return Ok();
    }
}