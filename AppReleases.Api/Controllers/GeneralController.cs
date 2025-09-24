using AppReleases.Api.Helpers;
using AppReleases.Core.Abstractions;
using AppReleases.DataAccess;
using AppReleases.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppReleases.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class GeneralController(
    AuthorizationHelper authorizationHelper,
    AppReleasesDbContext dbContext,
    IMetricsRepository metricsRepository) : Controller
{
    [HttpPost("migrate")]
    [Authorize(AuthenticationSchemes = "Basic")]
    public async Task<ActionResult<string>> MigrationHandler()
    {
        if (!authorizationHelper.VerifyAdmin(User))
            return Unauthorized();
        await dbContext.Database.MigrateAsync();
        return Ok();
    }

    [HttpGet("metrics")]
    public async Task<ActionResult<IEnumerable<Metric>>> GetMetrics([FromQuery] string query,
        [FromQuery] DateTime? time = null)
    {
        if (time == null)
            return Ok(await metricsRepository.GetMetricsAsync(query));
        return Ok(await metricsRepository.GetMetricsAsync(query, time.Value));
    }
}