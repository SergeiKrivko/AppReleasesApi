using AppReleases.Api.Helpers;
using AppReleases.Application.Services;
using AppReleases.Core.Abstractions;
using AppReleases.DataAccess;
using AppReleases.DataAccess.Repositories;
using AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IReleaseRepository, ReleaseRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<BasicAuthService>();
builder.Services.AddScoped<AuthorizationHelper>();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Basic Auth", new OpenApiSecurityScheme
    {
        Description = "Authorization with login and password",
        Name = "Basic Auth",
        Scheme = "Basic",
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
                    Id = "basic"
                }
            },
            []
        }
    });
});

builder.Services.AddDbContext<AppReleasesDbContext>(
    options => { options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")); });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
    });
builder.Services.AddAuthentication(BasicDefaults.AuthenticationScheme)
    .AddBasic<BasicAuthService>(options => { options.Realm = "Avalux.AppReleases"; });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();