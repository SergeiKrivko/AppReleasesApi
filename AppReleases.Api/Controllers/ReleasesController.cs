using AppReleases.Api.Helpers;
using AppReleases.Api.Schemas;
using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppReleases.Api.Controllers;

[ApiController]
[Route(("api/v1/releases"))]
public class ReleasesController(AuthorizationHelper authorizationHelper, IReleaseService releaseService) : Controller
{
    [HttpPost("diff")]
    public async Task<ActionResult<ReleaseDifference>> GetReleaseDifference(
        [FromBody] ReleaseDifferenceRequestSchema schema)
    {
        if (!await authorizationHelper.VerifyApplication(User, schema.ApplicationKey))
            return Unauthorized();

        var result = await releaseService.GetReleaseDifferenceAsync(schema.Assets);
        return Ok(result);
    }
}