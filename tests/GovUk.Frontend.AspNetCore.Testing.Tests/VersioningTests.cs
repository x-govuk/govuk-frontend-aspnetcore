using System.Reflection;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

public class VersioningTests
{
    [Fact]
    public void TestingAssembly_HasTheLibrarysVersion()
    {
        // The packages are versioned in lockstep; both get their version from the same MinVer settings.
        var libraryVersion = GetInformationalVersion(typeof(GovUkFrontendOptions).Assembly);
        var testingVersion = GetInformationalVersion(Assembly.Load("GovUk.Frontend.AspNetCore.Testing"));

        Assert.Equal(libraryVersion, testingVersion);
    }

    private static string? GetInformationalVersion(Assembly assembly) =>
        assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
}
