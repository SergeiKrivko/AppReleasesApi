using System.Collections;
using System.Text.Json;
using AppReleases.Api.Helpers;
using AppReleases.Api.Schemas;
using AppReleases.Core.Abstractions;
using AppReleases.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppReleases.Api.Controllers;

[ApiController]
[Route("api/v1/releases")]
public class ReleasesController(
    AuthorizationHelper authorizationHelper,
    IReleaseService releaseService,
    IApplicationService applicationService,
    IBranchService branchService,
    IBundleService bundleService,
    IMetricsHelper metricsHelper) : Controller
{
    [HttpPost("diff")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<ReleaseDifference>> GetReleaseDifference([FromBody] AssetInfo[] assets)
    {
        var result = await releaseService.GetReleaseDifferenceAsync(assets);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<ReleaseDifference>> CreateRelease(CreateReleaseSchema schema)
    {
        if (!await authorizationHelper.VerifyApplication(User, schema.ApplicationKey))
            return Unauthorized();

        var application = await applicationService.GetApplicationByKeyAsync(schema.ApplicationKey);
        var branch =
            await branchService.GetOrCreateBranchAsync(application.Id, schema.Branch ?? application.MainBranch);

        if (await releaseService.FindReleaseAsync(branch.Id, schema.Platform, schema.Version) is not null)
            return Conflict("Release already exists");

        var result = await releaseService.CreateReleaseAsync(branch.Id, schema.Platform, schema.Version);

        metricsHelper.PublishDownloadRelease(application.Key, branch.Name, result);

        return Ok(result);
    }

    [HttpPut("{releaseId:guid}")]
    [Authorize(AuthenticationSchemes = "Bearer,Basic")]
    public async Task<ActionResult<Release>> UpdateRelease(Guid releaseId, UpdateReleaseSchema schema)
    {
        var release = await releaseService.GetReleaseByIdAsync(releaseId);
        var branch = await branchService.GetBranchByIdAsync(release.BranchId);
        var application = await applicationService.GetApplicationByIdAsync(branch.ApplicationId);
        if (!await authorizationHelper.VerifyApplication(User, application))
            return Unauthorized();

        await releaseService.UpdateReleaseAsync(releaseId, schema.Description);
        release.ReleaseNotes = schema.Description;
        return Ok(release);
    }

    [HttpPut("{releaseId:guid}/assets")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [RequestSizeLimit(104857600)]
    public async Task<ActionResult> UploadReleaseAssets(Guid releaseId,
        [FromForm] AssetInfo[] assets, IFormFile zip)
    {
        assets = JsonSerializer.Deserialize<AssetInfo[]>(Request.Form["assets"]);
        var release = await releaseService.GetReleaseByIdAsync(releaseId);
        var branch = await branchService.GetBranchByIdAsync(release.BranchId);
        var application = await applicationService.GetApplicationByIdAsync(branch.ApplicationId);
        if (!await authorizationHelper.VerifyApplication(User, application))
            return Unauthorized();

        await releaseService.UploadAssetsAsync(releaseId, assets ?? [], zip.OpenReadStream());

        metricsHelper.PublishDownloadAssets(application.Key, branch.Name, release);

        return Ok();
    }

    [HttpGet("{releaseId:guid}/assets")]
    public async Task<ActionResult<AssetInfo[]>> ListReleaseAssets(Guid releaseId)
    {
        var assets = await releaseService.ListAssetsAsync(releaseId);
        return Ok(assets);
    }

    [HttpGet("{releaseId:guid}/assets/download")]
    public async Task<ActionResult<DownloadUrlResponseSchema>> DownloadReleaseAssets(Guid releaseId)
    {
        var release = await releaseService.GetReleaseByIdAsync(releaseId);
        var branch = await branchService.GetBranchByIdAsync(release.BranchId);
        var application = await applicationService.GetApplicationByIdAsync(branch.ApplicationId);
        var url = await metricsHelper.MeasureDownloadAssets(() => releaseService.PackAssetsAsync(releaseId),
            application.Key, branch.Name, release);
        return Ok(new DownloadUrlResponseSchema
        {
            Url = url
        });
    }

    [HttpPost("{releaseId:guid}/assets/download")]
    public async Task<ActionResult<AssetsPack>> DownloadReleaseAssetsDifference(Guid releaseId,
        [FromBody] AssetInfo[] assets)
    {
        var release = await releaseService.GetReleaseByIdAsync(releaseId);
        var branch = await branchService.GetBranchByIdAsync(release.BranchId);
        var application = await applicationService.GetApplicationByIdAsync(branch.ApplicationId);
        var result = await metricsHelper.MeasureDownloadAssets(() => releaseService.PackAssetsAsync(releaseId, assets),
            application.Key, branch.Name, release);
        return Ok(result);
    }

    [HttpGet("{releaseId:guid}/bundles")]
    public async Task<ActionResult<IEnumerable<Bundle>>> GetReleaseBundles(Guid releaseId)
    {
        var bundles = await bundleService.GetAllBundlesAsync(releaseId);
        return Ok(bundles);
    }

    [HttpPost("{releaseId:guid}/bundles")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [RequestSizeLimit(104857600)]
    public async Task<ActionResult<Bundle>> CreateReleaseBundle(Guid releaseId, IFormFile file)
    {
        var release = await releaseService.GetReleaseByIdAsync(releaseId);
        var branch = await branchService.GetBranchByIdAsync(release.BranchId);
        var application = await applicationService.GetApplicationByIdAsync(branch.ApplicationId);
        if (!await authorizationHelper.VerifyApplication(User, application))
            return Unauthorized();

        if (await bundleService.FindBundleAsync(releaseId, file.FileName) is not null)
            return Conflict("Bundle already exists");

        var bundle = await bundleService.CreateBundleAsync(releaseId, file.FileName, file.OpenReadStream());

        metricsHelper.PublishDownloadBundle(application.Key, branch.Name, release, bundle.BundleId);

        return Ok(bundle);
    }

    [HttpGet("{releaseId:guid}/bundles/{bundleId:guid}/download")]
    public async Task<ActionResult<DownloadUrlResponseSchema>> GetDownloadBundleUrl(Guid releaseId, Guid bundleId)
    {
        var release = await releaseService.GetReleaseByIdAsync(releaseId);
        var branch = await branchService.GetBranchByIdAsync(release.BranchId);
        var application = await applicationService.GetApplicationByIdAsync(branch.ApplicationId);
        var url = await bundleService.GetDownloadUrlAsync(bundleId);
        metricsHelper.AddDownloadBundle(application.Key, branch.Name, release, bundleId);
        return Ok(new DownloadUrlResponseSchema
        {
            Url = url
        });
    }
}