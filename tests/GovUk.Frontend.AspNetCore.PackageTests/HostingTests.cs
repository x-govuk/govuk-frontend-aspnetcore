using System.Net;
using System.Text.RegularExpressions;
using GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

namespace GovUk.Frontend.AspNetCore.PackageTests;

/// <summary>
/// Whether the files the targets restored are actually served, at the URLs the library tells you to use
/// and with the caching the versioned asset middleware promises.
/// </summary>
public partial class HostingTests(DefaultConfigurationAppFixture fixture, PackageTestContext context) :
    IClassFixture<DefaultConfigurationAppFixture>
{
    public static TheoryData<string> TargetFrameworks => [.. FixtureProject.AllTargetFrameworks];

    /// <summary>
    /// The strongest form of the assertion: rather than hard-coding paths, ask the page template which
    /// URLs it emits and check those are the ones that work.
    /// </summary>
    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public async Task ThePageTemplateAdvertisesUrlsThatAreServed(string targetFramework)
    {
        await using var app = await fixture.StartAsync(targetFramework);

        var page = await HostedPage.GetAsync(app);

        await AssertServedAsync(app, page.Stylesheet, "text/css");
        await AssertServedAsync(app, page.Script, "text/javascript");

        // These live under the assets directory, so they cover the recursive copy as well.
        await AssertServedAsync(app, page.FavIcon, expectedContentType: null);
        await AssertServedAsync(app, page.Manifest, expectedContentType: null);
    }

    /// <summary>
    /// On .NET 9 and later the page template resolves its URLs through the static asset collection that
    /// <c>MapStaticAssets</c> builds from the static web assets manifest, so this is also what proves the
    /// restored files were added to <c>@(Content)</c> early enough to be in that manifest.
    /// </summary>
    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public async Task ThePageTemplateUsesFingerprintedUrlsOnlyWhereTheFrameworkSupportsThem(string targetFramework)
    {
        await using var app = await fixture.StartAsync(targetFramework);

        var page = await HostedPage.GetAsync(app);

        if (targetFramework == "net8.0")
        {
            Assert.Equal($"/govuk-frontend.min.css?v={context.GovUkFrontendVersion}", page.Stylesheet);
        }
        else
        {
            Assert.Matches(@"^/govuk-frontend\.min\.[a-z0-9]+\.css$", page.Stylesheet);
        }
    }

    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public async Task RequestForARestoredFileWithTheCurrentVersion_IsCacheableForever(string targetFramework)
    {
        await using var app = await fixture.StartAsync(targetFramework);

        foreach (var path in RestoredFilePaths())
        {
            using var response = await app.Client.GetAsync(Versioned(path, context.GovUkFrontendVersion));

            AssertCacheableForever(response, path);
        }
    }

    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public async Task RequestForARestoredFileWithAnotherVersion_IsNotCacheableForever(string targetFramework)
    {
        await using var app = await fixture.StartAsync(targetFramework);

        foreach (var path in RestoredFilePaths())
        {
            using var withWrongVersion = await app.Client.GetAsync(Versioned(path, "0.0.0"));
            AssertNotCacheableForever(withWrongVersion, path);

            using var withNoVersion = await app.Client.GetAsync(path);
            AssertNotCacheableForever(withNoVersion, path);
        }
    }

    /// <summary>
    /// The middleware keys off the directories in the build info, so a versioned request for something it
    /// didn't restore must be left alone.
    /// </summary>
    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public async Task RequestForAnUnrelatedFileWithTheCurrentVersion_IsNotCacheableForever(string targetFramework)
    {
        await using var app = await fixture.StartAsync(targetFramework);

        var path = "/" + HostedAppFixture.UnrelatedStaticFile;

        using var response = await app.Client.GetAsync(Versioned(path, context.GovUkFrontendVersion));

        AssertNotCacheableForever(response, path);
    }

    /// <summary>
    /// The stylesheet references the fonts and images itself, with the version already appended, so those
    /// URLs have to resolve and pick up the same caching as everything else.
    /// </summary>
    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public async Task TheAssetUrlsInsideTheStylesheetAreServedAndCacheableForever(string targetFramework)
    {
        await using var app = await fixture.StartAsync(targetFramework);

        var stylesheet = await app.Client.GetStringAsync("/govuk-frontend.min.css");

        var assetUrls = AssetUrlRegex().Matches(stylesheet).Select(m => m.Value).Distinct().ToArray();

        Assert.NotEmpty(assetUrls);
        Assert.All(assetUrls, url => Assert.Contains($"?v={context.GovUkFrontendVersion}", url, StringComparison.Ordinal));

        foreach (var url in assetUrls)
        {
            using var response = await app.Client.GetAsync(url);

            AssertCacheableForever(response, url);
        }
    }

    private static IEnumerable<string> RestoredFilePaths() =>
    [
        "/govuk-frontend.min.css",
        "/govuk-frontend.min.js",
        "/assets/images/favicon.ico",
        "/assets/manifest.json"
    ];

    internal static string Versioned(string path, string version) => $"{path}?v={version}";

    internal static async Task AssertServedAsync(FixtureApp app, string url, string? expectedContentType)
    {
        using var response = await app.Client.GetAsync(url);

        Assert.True(
            response.StatusCode == HttpStatusCode.OK,
            $"GET {url} returned {(int)response.StatusCode}.");

        if (expectedContentType is not null)
        {
            Assert.Equal(expectedContentType, response.Content.Headers.ContentType?.MediaType);
        }

        Assert.NotEmpty(await response.Content.ReadAsByteArrayAsync());
    }

    internal static void AssertCacheableForever(HttpResponseMessage response, string path)
    {
        Assert.True(response.StatusCode == HttpStatusCode.OK, $"GET {path} returned {(int)response.StatusCode}.");

        var cacheControl = response.Headers.CacheControl;

        Assert.NotNull(cacheControl);
        Assert.True(cacheControl.Public, $"{path}: expected a public Cache-Control, got '{cacheControl}'.");
        Assert.Equal(TimeSpan.FromDays(365), cacheControl.MaxAge);
        Assert.Contains("immutable", cacheControl.ToString(), StringComparison.Ordinal);

        // A validator would let the browser revalidate a response that never changes.
        Assert.Null(response.Headers.ETag);
        Assert.Null(response.Content.Headers.LastModified);
    }

    internal static void AssertNotCacheableForever(HttpResponseMessage response, string path)
    {
        Assert.True(response.StatusCode == HttpStatusCode.OK, $"GET {path} returned {(int)response.StatusCode}.");

        var cacheControl = response.Headers.CacheControl?.ToString() ?? "";

        Assert.DoesNotContain("immutable", cacheControl, StringComparison.Ordinal);
    }

    [GeneratedRegex(@"/assets/[^)""']+")]
    private static partial Regex AssetUrlRegex();
}

/// <summary>
/// The same, for a project that has moved everything the package restores into a subdirectory.
/// </summary>
public class CustomDirectoryHostingTests(CustomDirectoryAppFixture fixture, PackageTestContext context) :
    IClassFixture<CustomDirectoryAppFixture>
{
    public static TheoryData<string> TargetFrameworks => [.. FixtureProject.AllTargetFrameworks];

    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public async Task RestoredFilesAreServedFromTheCustomDirectoryAndCacheableForever(string targetFramework)
    {
        await using var app = await fixture.StartAsync(targetFramework);

        string[] paths =
        [
            "/govuk/govuk-frontend.min.css",
            "/govuk/govuk-frontend.min.js",
            "/govuk/assets/images/favicon.ico"
        ];

        foreach (var path in paths)
        {
            await HostingTests.AssertServedAsync(app, path, expectedContentType: null);

            using var response = await app.Client.GetAsync(HostingTests.Versioned(path, context.GovUkFrontendVersion));

            HostingTests.AssertCacheableForever(response, path);
        }
    }

    /// <summary>
    /// Pins a known gap: <c>PageTemplateHelper</c> builds its URLs from the file names alone and never
    /// consults the directories in the build info, so moving the stylesheet or the script leaves the page
    /// template pointing at a URL that isn't there. Callers have to pass a <c>pathBase</c> themselves.
    /// </summary>
    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public async Task ThePageTemplateStillAdvertisesTheDefaultUrls(string targetFramework)
    {
        await using var app = await fixture.StartAsync(targetFramework);

        var page = await HostedPage.GetAsync(app);

        Assert.StartsWith("/govuk-frontend.min.css", page.Stylesheet, StringComparison.Ordinal);
        Assert.StartsWith("/govuk-frontend.min.js", page.Script, StringComparison.Ordinal);

        using var stylesheet = await app.Client.GetAsync(page.Stylesheet);
        Assert.Equal(HttpStatusCode.NotFound, stylesheet.StatusCode);
    }

    /// <summary>
    /// The assets path is configurable through view data, so the head icons can be pointed at the moved
    /// directory even though the stylesheet and script can't.
    /// </summary>
    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public async Task TheHeadIconsFollowTheConfiguredAssetPath(string targetFramework)
    {
        await using var app = await fixture.StartAsync(targetFramework);

        var page = await HostedPage.GetAsync(app, "/?assetPath=/govuk/assets");

        Assert.StartsWith("/govuk/assets/", page.FavIcon, StringComparison.Ordinal);

        await HostingTests.AssertServedAsync(app, page.FavIcon, expectedContentType: null);
    }
}
