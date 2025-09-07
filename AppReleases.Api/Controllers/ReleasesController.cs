using AppReleases.Api.Helpers;
using AppReleases.Api.Schemas;
using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppReleases.Api.Controllers;

[ApiController]
[Route("api/v1/releases")]
public class ReleasesController(
    AuthorizationHelper authorizationHelper,
    IReleaseService releaseService,
    IApplicationService applicationService,
    IBranchService branchService) : Controller
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

    [HttpPost]
    public async Task<ActionResult<ReleaseDifference>> CreateRelease(
        CreateReleaseSchema schema)
    {
        if (!await authorizationHelper.VerifyApplication(User, schema.ApplicationKey))
            return Unauthorized();

        var application = await applicationService.GetApplicationByKeyAsync(schema.ApplicationKey);
        var branch = await branchService.GetOrCreateBranchAsync(application.Id, schema.Branch ?? application.MainBranch);
        var result = await releaseService.CreateReleaseAsync(branch.Id, schema.Platform, schema.Version);
        return Ok(result);
    }
}