using AppReleases.Api.Helpers;
using AppReleases.Api.Schemas;
using AppReleases.Core.Abstractions;
using AppReleases.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing;

namespace AppReleases.Api.Controllers;

[ApiController]
[Route("api/v1/apps")]
public class ApplicationController(
    AuthorizationHelper authorizationHelper,
    ITokenService tokenService,
    IApplicationService applicationService) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Models.Application>>> GetAllApplications()
    {
        var applications = await applicationService.GetAllApplicationsAsync();
        if (authorizationHelper.VerifyAdmin(User))
            return Ok(applications);

        Guid tokenId;
        try
        {
            tokenId = Guid.Parse(User.Claims.Single(c => c.Type == "tokenId").Value);
        }
        catch (Exception)
        {
            return Unauthorized();
        }

        var token = await tokenService.GetTokenByIdAsync(tokenId);
        var matcher = new Matcher();
        matcher.AddInclude(token.Mask);
        return Ok(applications.Where(a => matcher.Match(a.Key).HasMatches));
    }

    [HttpGet("{applicationId:guid}")]
    public async Task<ActionResult<Models.Application>> GetApplicationById(Guid applicationId)
    {
        var application = await applicationService.GetApplicationByIdAsync(applicationId);
        if (!await authorizationHelper.VerifyApplication(User, application))
            return Unauthorized();
        return Ok(application);
    }

    [HttpGet("search")]
    public async Task<ActionResult<Models.Application>> SearchApplication(
        [FromQuery] string applicationKey)
    {
        if (!await authorizationHelper.VerifyApplication(User, applicationKey))
            return Unauthorized();
        var application = await applicationService.GetApplicationByKeyAsync(applicationKey);
        return Ok(application);
    }

    [HttpPost]
    public async Task<ActionResult<Models.Application>> CreateApplication(
        [FromBody] CreateApplicationSchema schema)
    {
        if (!await authorizationHelper.VerifyApplication(User, schema.Key))
            return Unauthorized();
        var result =
            await applicationService.CreateApplicationAsync(schema.Key, schema.Name, schema.Description,
                schema.MainBranch);
        return Ok(result);
    }

    [HttpPut("{applicationId:guid}")]
    public async Task<ActionResult> UpdateApplication(Guid applicationId, [FromBody] UpdateApplicationSchema schema)
    {
        var application = await applicationService.GetApplicationByIdAsync(applicationId);
        if (!await authorizationHelper.VerifyApplication(User, application))
            return Unauthorized();
        await applicationService.UpdateApplicationAsync(applicationId, schema.Name, schema.Description,
            schema.MainBranch, schema.DefaultDuration);
        return Ok();
    }

    [HttpDelete("{applicationId:guid}")]
    public async Task<ActionResult> DeleteApplication(Guid applicationId)
    {
        var application = await applicationService.GetApplicationByIdAsync(applicationId);
        if (!await authorizationHelper.VerifyApplication(User, application))
            return Unauthorized();
        await applicationService.DeleteApplicationAsync(applicationId);
        return Ok();
    }
}