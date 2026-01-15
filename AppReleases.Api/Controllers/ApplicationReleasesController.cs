using AppReleases.Core.Abstractions;
using AppReleases.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppReleases.Api.Controllers;

[ApiController]
[Route("/api/v1/apps/{applicationId:guid}/releases")]
public class ApplicationReleasesController(IReleaseService releaseService) : Controller
{
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Basic")]
    public async Task<ActionResult<IEnumerable<Release>>> GetApplicationReleases(Guid applicationId)
    {
        var releases = await releaseService.GetAllReleasesOfApplicationAsync(applicationId);
        return Ok(releases);
    }

    [HttpGet("latest")]
    public async Task<ActionResult<Release>> GetLatestApplicationRelease(Guid applicationId, string platform)
    {
        var release = await releaseService.GetLatestReleaseAsync(applicationId, platform);
        if (release == null)
            return NotFound();
        return Ok(release);
    }
}