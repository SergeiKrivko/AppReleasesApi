using AppReleases.Api.Helpers;
using AppReleases.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class GeneralController(AuthorizationHelper authorizationHelper, AppReleasesDbContext dbContext) : Controller
{
    [HttpPost("migrate")]
    [Authorize(AuthenticationSchemes = "Basic")]
    public async Task<ActionResult<string>> MigrationHandler()
    {
        await dbContext.Database.MigrateAsync();
        return Ok();
    }
}