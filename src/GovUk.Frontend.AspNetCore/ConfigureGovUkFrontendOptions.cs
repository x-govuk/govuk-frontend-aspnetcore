using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore;

internal class ConfigureGovUkFrontendOptions(ApplicationPartManager applicationPartManager) : IConfigureOptions<GovUkFrontendOptions>
{
    public void Configure(GovUkFrontendOptions options)
    {
        var buildInfoAttributes = applicationPartManager.ApplicationParts.OfType<AssemblyPart>()
            .Select(ap => ap.Assembly)
            .Select(a => a.GetCustomAttribute<GovUkFrontendBuildInfoAttribute>())
            .Where(attr => attr is not null)
            .ToArray();

        if (buildInfoAttributes.Length != 1)
        {
            return;
        }

        var buildInfo = buildInfoAttributes.Single()!;

        if (buildInfo.FrontendNpmPackageRestored)
        {
            options.FrontendPackageHostingOptions = FrontendPackageHostingOptions.None;
        }
    }
}
