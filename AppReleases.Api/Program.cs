using AppReleases.Api.BackgroundServices;
using AppReleases.Api.Helpers;
using AppReleases.Application.Services;
using AppReleases.Core.Abstractions;
using AppReleases.DataAccess;
using AppReleases.DataAccess.Repositories;
using AppReleases.S3;
using AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

ValidateRequiredSecrets();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IBundleRepository, BundleRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IReleaseRepository, ReleaseRepository>();
builder.Services.AddScoped<IInstallerBuilderRepository, InstallerBuilderRepository>();
builder.Services.AddScoped<IBuiltInstallerRepository, BuiltInstallerRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddSingleton<IFileRepository, S3Repository>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IReleaseService, ReleaseService>();
builder.Services.AddScoped<IBundleService, BundleService>();
builder.Services.AddScoped<IInstallerService, InstallerService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICleanerService, CleanerService>();

builder.Services.AddScoped<BasicAuthService>();
builder.Services.AddScoped<AuthorizationHelper>();
builder.Services.AddScoped<IMetricsHelper, MetricsHelper>();
builder.Services.AddSingleton<IMetricsRepository, MetricsRepository>();

builder.Services.AddHostedService<TempFileCleanerService>();
builder.Services.AddHostedService<ReleaseCleanerService>();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
    {
        Description = "Authorization with login and password",
        Name = "Basic Auth",
        Scheme = "Basic",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Authorization with API token",
        Name = "Token Auth",
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Basic"
                }
            },
            []
        }
    });
});

builder.Services.AddDbContext<AppReleasesDbContext>(
    options => { options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")); });

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = TokenService.Issuer,
            ValidateAudience = true,
            ValidAudience = TokenService.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = TokenService.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var tokenIdClaim = context.Principal?.Claims.FirstOrDefault(c => c.Type == "TokenId");
                if (tokenIdClaim is null || !Guid.TryParse(tokenIdClaim.Value, out var tokenId))
                {
                    context.Fail("TokenId claim is missing or invalid");
                    return;
                }

                var tokenRepository = context.HttpContext.RequestServices.GetRequiredService<ITokenRepository>();
                try
                {
                    var token = await tokenRepository.GetTokenByIdAsync(tokenId);
                    if (token.RevokedAt != null || token.ExpiresAt <= DateTime.UtcNow)
                        context.Fail("Token is revoked or expired");
                }
                catch (Exception)
                {
                    context.Fail("Token not found");
                }
            }
        };
    })
    .AddBasic<BasicAuthService>(options => { options.Realm = "Avalux.AppReleases"; });

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpMetrics();
app.MapMetrics();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapControllers();
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "wwwroot";
    spa.Options.DefaultPage = "/index.html";
});

app.Run();

void ValidateRequiredSecrets()
{
    var adminLogin = Environment.GetEnvironmentVariable("ADMIN_LOGIN");
    var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
    if (string.IsNullOrWhiteSpace(adminLogin))
        throw new InvalidOperationException("ADMIN_LOGIN is required");
    if (string.IsNullOrWhiteSpace(adminPassword))
        throw new InvalidOperationException("ADMIN_PASSWORD is required");

    TokenService.ValidateJwtSecret();
}
