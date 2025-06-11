using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AppReleases.Api.Helpers;

public class JwtBearerConfig
{
    public static string Issuer { get; } = Environment.GetEnvironmentVariable("JWT_TOKEN_ISSUER") ?? "Avalux.AppReleases";
    public static string Audience { get; } = "AccessToken";
    private static string Key => Environment.GetEnvironmentVariable("JWT_TOKEN_SECRET") ?? "123567890";

    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
}