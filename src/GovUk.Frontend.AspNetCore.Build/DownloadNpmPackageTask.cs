#nullable disable
using Microsoft.Build.Framework;
using MsBuildTask = Microsoft.Build.Utilities.Task;

namespace GovUk.Frontend.AspNetCore.Build;

public class DownloadNpmPackageTask : MsBuildTask
{
    [Required]
    public string Package { get; set; }

    [Required]
    public string Version { get; set; }

    [Required]
    public string PackageBaseDirectory { get; set; }

    [Required]
    public string DestinationDirectory { get; set; }

    [Required]
    public string Include { get; set; }

    public override bool Execute()
    {
        try
        {
            ExecuteAsync().GetAwaiter().GetResult();
            return true;
        }
        catch (Exception ex)
        {
            Log.LogError("Failed downloading NPM package.\n\n{0}", ex);
            return false;
        }

        async Task ExecuteAsync()
        {
            var includePatterns = Include?.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? ["*"];

            var downloader = new NpmPackageDownloader();
            await downloader.DownloadPackage(Package, Version, PackageBaseDirectory, DestinationDirectory, includePatterns);
        }
    }
}
