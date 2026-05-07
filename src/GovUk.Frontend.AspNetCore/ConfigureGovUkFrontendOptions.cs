using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore;

internal class ConfigureGovUkFrontendOptions(ApplicationPartManager applicationPartManager, IWebHostEnvironment environment) :
    IConfigureOptions<GovUkFrontendOptions>
{
    public void Configure(GovUkFrontendOptions options)
    {
        var buildInfoAttributes = applicationPartManager.ApplicationParts.OfType<AssemblyPart>()
            .Select(ap => ap.Assembly)
            .Select(a => a.GetCustomAttribute<GovUkFrontendBuildInfoAttribute>())
            .Where(attr => attr is not null)
            .ToArray();

        if (buildInfoAttributes.Length is 0)
        {
            return;
        }

        var buildInfo = buildInfoAttributes.Single()!;

        if (buildInfo.AssetsPath is var assetsPath && !string.IsNullOrEmpty(assetsPath) &&
            assetsPath.StartsWith(environment.WebRootPath, StringComparison.OrdinalIgnoreCase))
        {
            options.AssetsPath = assetsPath[environment.WebRootPath.Length..].TrimStart('/');
        }
    }
}
