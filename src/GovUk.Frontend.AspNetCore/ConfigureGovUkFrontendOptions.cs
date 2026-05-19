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

        if (buildInfoAttributes.Length is 0)
        {
            return;
        }

        var buildInfo = buildInfoAttributes.Single()!;

        if (buildInfo.EnableGovUkFrontendSupport)
        {
            options.FrontendPackageHostingOptions = FrontendPackageHostingOptions.None;
        }
    }
}
