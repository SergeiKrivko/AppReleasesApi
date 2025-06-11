using AspNetCore.Authentication.Basic;

namespace AppReleases.Api.Helpers;

public class BasicAuthService : IBasicUserValidationService
{
    internal string Login { get; } = Environment.GetEnvironmentVariable("ADMIN_LOGIN") ?? "admin";
    private string Password { get; } = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "admin";

    public Task<bool> IsValidAsync(string username, string password)
    {
        return Task.FromResult(username == Login && password == Password);
    }
}