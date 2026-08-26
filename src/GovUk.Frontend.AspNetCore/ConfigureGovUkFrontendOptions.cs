using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore;

internal class ConfigureGovUkFrontendOptions(IHostEnvironment? hostEnvironment) : IConfigureOptions<GovUkFrontendOptions>
{
    public void Configure(GovUkFrontendOptions options)
    {
        // The targets that emit the attribute are transitive, so every project in the build that references
        // the package carries one, describing how that project was built. Only the application's own answer
        // is meaningful here; a class library's says nothing about where the app serves its files from.
        var buildInfo = GetApplicationAssembly()?.GetCustomAttribute<GovUkFrontendBuildInfoAttribute>();

        if (buildInfo is null)
        {
            return;
        }

        options.BuildInfo = buildInfo;
    }

    private Assembly? GetApplicationAssembly()
    {
        // How the framework itself identifies the application; a test host sets it to the assembly under test.
        var applicationName = hostEnvironment?.ApplicationName;

        if (string.IsNullOrEmpty(applicationName))
        {
            return Assembly.GetEntryAssembly();
        }

        try
        {
            return Assembly.Load(new AssemblyName(applicationName));
        }
        catch (Exception ex) when (ex is FileNotFoundException or FileLoadException or BadImageFormatException)
        {
            // ApplicationName is just a string and need not name a loadable assembly. The build info is
            // optional, so fall back rather than bringing the application down over it.
            return Assembly.GetEntryAssembly();
        }
    }
}
