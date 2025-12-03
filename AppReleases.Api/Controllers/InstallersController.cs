using AppReleases.Api.Schemas;
using AppReleases.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AppReleases.Api.Controllers;

[ApiController]
[Route("api/v1/installers")]
public class InstallersController(IInstallerService installerService) : Controller
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InstallerBuilderSchema>>> GetAllBuilders(CancellationToken cancellationToken)
    {
        var builders = await installerService.GetAllBuildersAsync(cancellationToken);
        return Ok(builders.Select(e => new InstallerBuilderSchema
        {
            Key = e.Key,
            DisplayName = e.DisplayName,
            Description = e.Description,
        }));
    }
}