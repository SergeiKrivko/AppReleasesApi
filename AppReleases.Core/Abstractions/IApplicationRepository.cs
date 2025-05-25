using AppReleases.Core.Models;

namespace AppReleases.Core.Abstractions;

public interface IApplicationRepository
{
    public Task<IEnumerable<Application>> GetAllApplicationsAsync();
    public Task<Application> GetApplicationByIdAsync(Guid applicationId);
    public Task<Application> GetApplicationByKeyAsync(string key);
    public Task<Application> CreateApplicationAsync(Application application);
    public Task UpdateApplicationAsync(Guid applicationId, string name, string description);
    public Task DeleteApplicationAsync(Guid applicationId);
}