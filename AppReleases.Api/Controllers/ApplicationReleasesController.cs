using AppReleases.Core.Abstractions;
using AppReleases.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppReleases.Api.Controllers;

[ApiController]
[Route("/api/v1/apps/{applicationId:guid}/releases")]
public class ApplicationReleasesController(IReleaseService releaseService) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Release>>> GetApplicationReleases(Guid applicationId)
    {
        var releases = await releaseService.GetAllReleasesOfApplicationAsync(applicationId);
        return Ok(releases);
    }
}