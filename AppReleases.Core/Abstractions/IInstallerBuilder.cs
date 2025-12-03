using System.Text.Json.Nodes;
using AppReleases.Models;

namespace AppReleases.Core.Abstractions;

public interface IInstallerBuilder
{
    public string Key { get; }
    public Task<BuiltInstaller> Build(Application application, Release release, JsonObject settings);
}