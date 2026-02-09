using AspNetCore.Authentication.Basic;

namespace AppReleases.Api.Helpers;

public class BasicAuthService : IBasicUserValidationService
{
    internal string Login { get; }
    private string Password { get; }

    public BasicAuthService()
    {
        Login = Environment.GetEnvironmentVariable("ADMIN_LOGIN")
            ?? throw new InvalidOperationException("ADMIN_LOGIN is required");
        Password = Environment.GetEnvironmentVariable("ADMIN_PASSWORD")
            ?? throw new InvalidOperationException("ADMIN_PASSWORD is required");

        if (string.IsNullOrWhiteSpace(Login))
            throw new InvalidOperationException("ADMIN_LOGIN is required");
        if (string.IsNullOrWhiteSpace(Password))
            throw new InvalidOperationException("ADMIN_PASSWORD is required");
    }

    public Task<bool> IsValidAsync(string username, string password)
    {
        return Task.FromResult(username == Login && password == Password);
    }
}
