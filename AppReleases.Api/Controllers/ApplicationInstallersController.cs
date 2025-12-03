using AppReleases.Api.Helpers;
using AppReleases.Api.Schemas;
using AppReleases.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AppReleases.Api.Controllers;

[ApiController]
[Route("api/v1/apps/{applicationId:guid}/installers")]
public class ApplicationInstallersController(
    AuthorizationHelper authorizationHelper,
    IApplicationService applicationService,
    IInstallerService installerService
) : Controller
{
    [HttpPost]
    public async Task<ActionResult<Guid>> AddInstallerBuilder(Guid applicationId, AddInstallerBuilderSchema schema,
        CancellationToken cancellationToken)
    {
        var application = await applicationService.GetApplicationByIdAsync(applicationId);
        if (!await authorizationHelper.VerifyApplication(User, application))
            return Unauthorized();
        var id = await installerService.AddNewInstallerBuilderForApplicationAsync(schema.Key, schema.Name,
            applicationId,
            schema.InstallerLifetime, cancellationToken);
        return Ok(id);
    }

    [HttpGet]
    public async Task<ActionResult<Guid>> GetAllInstallerBuilders(Guid applicationId,
        CancellationToken cancellationToken)
    {
        var builders =
            await installerService.GetAllInstallerBuildersOfApplicationAsync(applicationId, cancellationToken);
        return Ok(builders);
    }
}