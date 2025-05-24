using AppReleases.Core.Abstractions;

namespace AppReleases.Application.Services;

public class ApplicationService(IApplicationRepository applicationRepository) : IApplicationService
{
    public Task<IEnumerable<Core.Models.Application>> GetAllApplicationsAsync()
    {
        return applicationRepository.GetAllApplicationsAsync();
    }

    public Task<IEnumerable<Core.Models.Application>> GetApplicationsByUserAsync(Guid userId)
    {
        return applicationRepository.GetAllApplicationsOfUserAsync(userId);
    }

    public Task<Core.Models.Application> GetApplicationByIdAsync(Guid applicationId)
    {
        return applicationRepository.GetApplicationByIdAsync(applicationId);
    }

    public Task<Core.Models.Application> GetApplicationByKeyAsync(string key)
    {
        return applicationRepository.GetApplicationByKeyAsync(key);
    }

    public Task CreateApplicationAsync(Guid ownerId, string key, string name, string description)
    {
        var application = new Core.Models.Application()
        {
            Id = Guid.NewGuid(),
            Key = key,
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            OwnerId = ownerId
        };
        return applicationRepository.CreateApplicationAsync(application);
    }

    public Task UpdateApplicationAsync(Guid applicationId, string name, string description)
    {
        return applicationRepository.UpdateApplicationAsync(applicationId, name, description);
    }

    public Task DeleteApplicationAsync(Guid applicationId)
    {
        return applicationRepository.DeleteApplicationAsync(applicationId);
    }
}