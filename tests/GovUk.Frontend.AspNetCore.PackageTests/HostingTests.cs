using System.Net;
using System.Security.Cryptography;
using System.Text;
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

    /// <summary>
    /// The init script embeds the URL of the JavaScript file, so the CSP hash and the emitted script have
    /// to be built from the same URL. A page whose hash doesn't cover its own script is one that a strict
    /// Content-Security-Policy would block.
    /// </summary>
    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public async Task TheCspHashesCoverTheScriptThePageActuallyEmitted(string targetFramework)
    {
        await using var app = await fixture.StartAsync(targetFramework);

        var page = await HostedPage.GetAsync(app);

        Assert.Contains(Sha256CspHash(page.InlineInitScript), page.CspScriptHashes, StringComparison.Ordinal);
    }

    internal static string Sha256CspHash(string value) =>
        $"'sha256-{Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(value)))}'";

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
    /// The page template has to follow the directories the build restored into, or a project that moves
    /// them gets a page pointing at URLs that aren't there.
    /// </summary>
    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public async Task ThePageTemplateAdvertisesTheCustomDirectoryUrlsAndTheyAreServed(string targetFramework)
    {
        await using var app = await fixture.StartAsync(targetFramework);

        var page = await HostedPage.GetAsync(app);

        Assert.StartsWith("/govuk/govuk-frontend.min", page.Stylesheet, StringComparison.Ordinal);
        Assert.StartsWith("/govuk/govuk-frontend.min", page.Script, StringComparison.Ordinal);
        Assert.StartsWith("/govuk/assets/", page.FavIcon, StringComparison.Ordinal);
        Assert.StartsWith("/govuk/assets/", page.Manifest, StringComparison.Ordinal);

        await HostingTests.AssertServedAsync(app, page.Stylesheet, "text/css");
        await HostingTests.AssertServedAsync(app, page.Script, "text/javascript");
        await HostingTests.AssertServedAsync(app, page.FavIcon, expectedContentType: null);
        await HostingTests.AssertServedAsync(app, page.Manifest, expectedContentType: null);
    }

    /// <summary>
    /// Where it matters most: the init script embeds the JavaScript URL, so if only one of the hash and
    /// the script picked up the custom directory they'd disagree.
    /// </summary>
    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public async Task TheCspHashesCoverTheScriptThePageActuallyEmitted(string targetFramework)
    {
        await using var app = await fixture.StartAsync(targetFramework);

        var page = await HostedPage.GetAsync(app);

        Assert.Contains("/govuk/govuk-frontend.min", page.InlineInitScript, StringComparison.Ordinal);
        Assert.Contains(HostingTests.Sha256CspHash(page.InlineInitScript), page.CspScriptHashes, StringComparison.Ordinal);
    }

    /// <summary>
    /// A view can still point the head icons somewhere else, which is what the assets path view data key
    /// is for.
    /// </summary>
    [Theory]
    [MemberData(nameof(TargetFrameworks))]
    public async Task TheHeadIconsFollowTheAssetPathViewDataKeyWhenItIsSet(string targetFramework)
    {
        await using var app = await fixture.StartAsync(targetFramework);

        var page = await HostedPage.GetAsync(app, "/?assetPath=/somewhere-else");

        Assert.StartsWith("/somewhere-else/", page.FavIcon, StringComparison.Ordinal);
    }
}
