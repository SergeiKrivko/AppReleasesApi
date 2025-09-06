using AppReleases.Api.Helpers;
using AppReleases.Api.Schemas;
using AppReleases.Core.Abstractions;
using AppReleases.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppReleases.Api.Controllers;

[ApiController]
[Route("api/v1/apps/{applicationId:guid}/branches")]
public class BranchesController(
    AuthorizationHelper authorizationHelper,
    IBranchService branchService,
    IApplicationService applicationService) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Branch>>> GetApplicationBranches(Guid applicationId)
    {
        var application = await applicationService.GetApplicationByIdAsync(applicationId);
        if (!(await authorizationHelper.VerifyApplication(User, application)))
            return Unauthorized();
        var result = await branchService.GetAllBranchesAsync(applicationId);
        return Ok(result);
    }

    [HttpGet("/{branchId:guid}")]
    public async Task<ActionResult<Branch>> GetApplicationBranchById(Guid applicationId, Guid branchId)
    {
        var application = await applicationService.GetApplicationByIdAsync(applicationId);
        if (!await authorizationHelper.VerifyApplication(User, application))
            return Unauthorized();
        var result = await branchService.GetBranchByIdAsync(branchId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Branch>> CreateApplicationBranch(Guid applicationId,
        [FromBody] CreateBranchSchema schema)
    {
        var application = await applicationService.GetApplicationByIdAsync(applicationId);
        if (!await authorizationHelper.VerifyApplication(User, application))
            return Unauthorized();
        var branch = await branchService.CreateBranchAsync(applicationId, schema.Name);
        return Ok(branch);
    }

    [HttpPut("/{branchId:guid}")]
    public async Task<ActionResult> UpdateApplicationBranch(Guid applicationId, Guid branchId,
        [FromBody] UpdateBranchSchema schema)
    {
        var application = await applicationService.GetApplicationByIdAsync(applicationId);
        if (!await authorizationHelper.VerifyApplication(User, application))
            return Unauthorized();

        var branch = await branchService.GetBranchByIdAsync(branchId);
        if (branch.ApplicationId != applicationId)
            return NotFound("Branch exists, but in another application"); // Потом переделать в целях безопасности

        await branchService.UpdateBranchAsync(branchId, schema.Duration);
        return Ok();
    }

    [HttpDelete("/{branchId:guid}")]
    public async Task<ActionResult> DeleteApplicationBranch(Guid applicationId, Guid branchId)
    {
        var application = await applicationService.GetApplicationByIdAsync(applicationId);
        if (!await authorizationHelper.VerifyApplication(User, application))
            return Unauthorized();

        var branch = await branchService.GetBranchByIdAsync(branchId);
        if (branch.ApplicationId != applicationId)
            return NotFound("Branch exists, but in another application"); // Потом переделать в целях безопасности

        await branchService.DeleteBranchAsync(branchId);
        return Ok();
    }
}