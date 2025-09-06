using AppReleases.Api.Helpers;
using AppReleases.Api.Schemas;
using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppReleases.Api.Controllers;

[ApiController]
[Route("api/v1/apps")]
public class ApplicationController(
    AuthorizationHelper authorizationHelper,
    IBranchService branchService,
    IApplicationService applicationService) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppReleases.Core.Models.Application>>> GetAllApplications()
    {
        if (!authorizationHelper.VerifyAdmin(User))
            return Unauthorized();
        var result = await applicationService.GetAllApplicationsAsync();
        return Ok(result);
    }

    [HttpGet("/{applicationId:guid}")]
    public async Task<ActionResult<AppReleases.Core.Models.Application>> GetApplicationById(Guid applicationId)
    {
        var application = await applicationService.GetApplicationByIdAsync(applicationId);
        if (!await authorizationHelper.VerifyApplication(User, application))
            return Unauthorized();
        return Ok(application);
    }

    [HttpGet("/search")]
    public async Task<ActionResult<AppReleases.Core.Models.Application>> SearchApplication([FromQuery] string applicationKey)
    {
        if (!await authorizationHelper.VerifyApplication(User, applicationKey))
            return Unauthorized();
        var application = await applicationService.GetApplicationByKeyAsync(applicationKey);
        return Ok(application);
    }

    [HttpPost]
    public async Task<ActionResult<Core.Models.Application>> CreateApplication(
        [FromBody] CreateApplicationSchema schema)
    {
        if (!await authorizationHelper.VerifyApplication(User, schema.Key))
            return Unauthorized();
        var result = applicationService.CreateApplicationAsync(schema.Key, schema.Name, schema.Description);
        return Ok(result);
    }

    [HttpPut("/{applicationId:guid}")]
    public async Task<ActionResult> UpdateApplication(Guid applicationId, [FromBody] CreateApplicationSchema schema)
    {
        var application = await applicationService.GetApplicationByIdAsync(applicationId);
        if (!await authorizationHelper.VerifyApplication(User, application))
            return Unauthorized();
        await applicationService.UpdateApplicationAsync(applicationId, schema.Name, schema.Description);
        return Ok();
    }

    [HttpDelete("/{applicationId:guid}")]
    public async Task<ActionResult> DeleteApplication(Guid applicationId)
    {
        var application = await applicationService.GetApplicationByIdAsync(applicationId);
        if (!await authorizationHelper.VerifyApplication(User, application))
            return Unauthorized();
        await applicationService.DeleteApplicationAsync(applicationId);
        return Ok();
    }
}