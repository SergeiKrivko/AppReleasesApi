using AppReleases.Core.Abstractions;

namespace AppReleases.Application.Services;

public class ApplicationService(IApplicationRepository applicationRepository, IBranchService branchService)
    : IApplicationService
{
    public Task<IEnumerable<Models.Application>> GetAllApplicationsAsync()
    {
        return applicationRepository.GetAllApplicationsAsync();
    }

    public Task<Models.Application> GetApplicationByIdAsync(Guid applicationId)
    {
        return applicationRepository.GetApplicationByIdAsync(applicationId);
    }

    public Task<Models.Application> GetApplicationByKeyAsync(string key)
    {
        return applicationRepository.GetApplicationByKeyAsync(key);
    }

    private static TimeSpan DefaultReleaseLifetime => TimeSpan.Zero;
    private static TimeSpan DefaultLatestReleaseLifetime => TimeSpan.FromDays(30);

    public async Task<Models.Application> CreateApplicationAsync(string key, string name, string description,
        string? mainBranch = null)
    {
        mainBranch ??= "main";
        var application = new Models.Application
        {
            Id = Guid.NewGuid(),
            Key = key,
            Name = name,
            Description = description,
            MainBranch = mainBranch,
            DefaultReleaseLifetime = DefaultReleaseLifetime,
            DefaultLatestReleaseLifetime = DefaultLatestReleaseLifetime,
            CreatedAt = DateTime.UtcNow,
        };
        await applicationRepository.CreateApplicationAsync(application);
        await branchService.CreateBranchAsync(application.Id, mainBranch, null, null, false);
        return application;
    }

    public Task UpdateApplicationAsync(Guid applicationId, string name, string description, string mainBranch,
        TimeSpan? defaultReleaseLifetime, TimeSpan? defaultLatestReleaseLifetime)
    {
        return applicationRepository.UpdateApplicationAsync(applicationId, name, description, mainBranch,
            defaultReleaseLifetime, defaultLatestReleaseLifetime);
    }

    public Task DeleteApplicationAsync(Guid applicationId)
    {
        return applicationRepository.DeleteApplicationAsync(applicationId);
    }
}