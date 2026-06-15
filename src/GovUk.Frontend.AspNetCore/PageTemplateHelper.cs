using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
#if NET9_0_OR_GREATER
using Microsoft.AspNetCore.Components;
#endif

namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Contains methods for generating script and stylesheet imports for the GDS page template.
/// </summary>
public class PageTemplateHelper
{
    internal const string JavascriptFileName = "govuk-frontend.min.js";
    internal const string StylesheetFileName = "govuk-frontend.min.css";

    private const string JsEnabledScript = "document.body.className += ' js-enabled' + ('noModule' in HTMLScriptElement.prototype ? ' govuk-frontend-supported' : '');";

    /// <summary>
    /// Gets the version of the GOV.UK Frontend library.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GovUkFrontendVersion => GovUkFrontendInfo.Version;

    internal static PathString DefaultAssetsPath => new("/assets");

    internal static PathString DefaultCompiledContentPath => new("");

    /// <summary>
    /// Generates the script that adds a <c>js-enabled</c> CSS class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The contents of this property should be inserted at the beginning of the <c>body</c> tag.
    /// </para>
    /// <para>
    /// Use the <see cref="GetJsEnabledScriptCspHash"/> method to retrieve a CSP hash if you are not specifying <paramref name="cspNonce"/>.
    /// </para>
    /// </remarks>
    /// <param name="cspNonce">The CSP nonce attribute to be added to the generated <c>script</c> tag.</param>
    /// <returns><see cref="IHtmlContent"/> containing the <c>script</c> tag.</returns>
    public IHtmlContent GenerateJsEnabledScript(string? cspNonce = null)
    {
        var tagBuilder = new TagBuilder("script");

        if (cspNonce is not null)
        {
            tagBuilder.MergeAttribute("nonce", cspNonce);
        }

        tagBuilder.InnerHtml.AppendHtml(new HtmlString(JsEnabledScript));

        return tagBuilder;
    }

    /// <summary>
    /// Generates the script that adds a <c>js-enabled</c> CSS class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The contents of this property should be inserted at the beginning of the <c>body</c> tag.
    /// </para>
    /// <para>
    /// Use the <see cref="GetJsEnabledScriptCspHash"/> method to retrieve a CSP hash if you are not specifying <paramref name="cspNonce"/>.
    /// </para>
    /// </remarks>
    /// <param name="cspNonce">The CSP nonce attribute to be added to the generated <c>script</c> tag.</param>
    /// <returns><see cref="IHtmlContent"/> containing the <c>script</c> tag.</returns>
    public IHtmlContent GenerateScriptImports(string? cspNonce = null) =>
        GenerateScriptImports(pathBase: DefaultCompiledContentPath, cspNonce);

    /// <summary>
    /// Generates the script that adds a <c>js-enabled</c> CSS class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The contents of this property should be inserted at the beginning of the <c>body</c> tag.
    /// </para>
    /// <para>
    /// Use the <see cref="GetJsEnabledScriptCspHash"/> method to retrieve a CSP hash if you are not specifying <paramref name="cspNonce"/>.
    /// </para>
    /// </remarks>
    /// <param name="pathBase">The base path that the script is hosted under.</param>
    /// <param name="cspNonce">The CSP nonce attribute to be added to the generated <c>script</c> tag.</param>
    /// <returns><see cref="IHtmlContent"/> containing the <c>script</c> tag.</returns>
    public IHtmlContent GenerateScriptImports(PathString pathBase, string? cspNonce)
    {
        var scriptPath = GetVersionedUrl(pathBase + "/" + JavascriptFileName);
        return GenerateScriptImports(scriptPath, cspNonce);
    }

    /// <summary>
    /// Generates the script that adds a <c>js-enabled</c> CSS class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The contents of this property should be inserted at the beginning of the <c>body</c> tag.
    /// </para>
    /// <para>
    /// Use the <see cref="GetJsEnabledScriptCspHash"/> method to retrieve a CSP hash if you are not specifying <paramref name="cspNonce"/>.
    /// </para>
    /// </remarks>
    /// <param name="viewContext">The <see cref="ViewContext"/> for the current request.</param>
    /// <param name="cspNonce">The CSP nonce attribute to be added to the generated <c>script</c> tag.</param>
    /// <returns><see cref="IHtmlContent"/> containing the <c>script</c> tag.</returns>
    public IHtmlContent GenerateScriptImports(ViewContext viewContext, string? cspNonce = null)
    {
        ArgumentNullException.ThrowIfNull(viewContext);

        var scriptPath = ResolveContentUrl(viewContext, JavascriptFileName);
        return GenerateScriptImports(scriptPath, cspNonce);
    }

    /// <summary>
    /// Generates the HTML that imports the GOV.UK Frontend library styles.
    /// </summary>
    /// <remarks>
    /// The contents of this property should be inserted in the <c>head</c> tag.
    /// </remarks>
    /// <returns><see cref="IHtmlContent"/> containing the <c>link</c> tags.</returns>
    public IHtmlContent GenerateStyleImports() => GenerateStyleImports(pathBase: DefaultCompiledContentPath);

    /// <summary>
    /// Generates the HTML that imports the GOV.UK Frontend library styles.
    /// </summary>
    /// <remarks>
    /// The contents of this property should be inserted in the <c>head</c> tag.
    /// </remarks>
    /// <param name="pathBase">The base path that the stylesheet is hosted under.</param>
    /// <returns><see cref="IHtmlContent"/> containing the <c>link</c> tags.</returns>
    public IHtmlContent GenerateStyleImports(PathString pathBase)
    {
        var fileName = GetVersionedUrl(pathBase + "/" + StylesheetFileName);
        return GenerateStyleImports(fileName);
    }

    /// <summary>
    /// Generates the HTML that imports the GOV.UK Frontend library styles.
    /// </summary>
    /// <remarks>
    /// The contents of this property should be inserted in the <c>head</c> tag.
    /// </remarks>
    /// <param name="viewContext">The <see cref="ViewContext"/> for the request.</param>
    /// <returns><see cref="IHtmlContent"/> containing the <c>link</c> tags.</returns>
    public IHtmlContent GenerateStyleImports(ViewContext viewContext)
    {
        ArgumentNullException.ThrowIfNull(viewContext);

        var fileName = ResolveContentUrl(viewContext, StylesheetFileName);
        return GenerateStyleImports(fileName);
    }

    /// <summary>
    /// Gets all the CSP hashes for the inline scripts used in the page template.
    /// </summary>
    /// <returns>A list of hashes to be included in your site's <c>Content-Security-Policy</c> header within the <c>script-src</c> directive.</returns>
    public string GetCspScriptHashes() => GetCspScriptHashes(pathBase: "/");

    /// <summary>
    /// Gets all the CSP hashes for the inline scripts used in the page template.
    /// </summary>
    /// <param name="pathBase">The base path that the script is hosted under.</param>
    /// <returns>A list of hashes to be included in your site's <c>Content-Security-Policy</c> header within the <c>script-src</c> directive.</returns>
    public string GetCspScriptHashes(PathString pathBase) => $"{GetInitScriptCspHash(pathBase)} {GetJsEnabledScriptCspHash()}";

    /// <summary>
    /// Gets all the CSP hashes for the inline scripts used in the page template.
    /// </summary>
    /// <param name="viewContext">The <see cref="ViewContext"/> for the current request.</param>
    /// <returns>A list of hashes to be included in your site's <c>Content-Security-Policy</c> header within the <c>script-src</c> directive.</returns>
    public string GetCspScriptHashes(ViewContext viewContext) => $"{GetInitScriptCspHash(viewContext)} {GetJsEnabledScriptCspHash()}";

    /// <summary>
    /// Gets the CSP hash for the script that adds a <c>js-enabled</c> CSS class.
    /// </summary>
    /// <returns>A hash to be included in your site's <c>Content-Security-Policy</c> header within the <c>script-src</c> directive.</returns>
    public string GetJsEnabledScriptCspHash() => GenerateCspHash(JsEnabledScript);

    /// <summary>
    /// Gets the CSP hash for the GOV.UK Frontend initialization script.
    /// </summary>
    /// <returns>A hash to be included in your site's <c>Content-Security-Policy</c> header within the <c>script-src</c> directive.</returns>
    public string GetInitScriptCspHash() => GetInitScriptCspHash(pathBase: "/");

    /// <summary>
    /// Gets the CSP hash for the GOV.UK Frontend initialization script.
    /// </summary>
    /// <param name="pathBase">The base path that the script is hosted under.</param>
    /// <returns>A hash to be included in your site's <c>Content-Security-Policy</c> header within the <c>script-src</c> directive.</returns>
    public string GetInitScriptCspHash(PathString pathBase) =>
        GenerateCspHash(GetInitScriptContents(pathBase));

    /// <summary>
    /// Gets the CSP hash for the GOV.UK Frontend initialization script.
    /// </summary>
    /// <param name="viewContext">The <see cref="ViewContext"/> for the current request.</param>
    /// <returns>A hash to be included in your site's <c>Content-Security-Policy</c> header within the <c>script-src</c> directive.</returns>
    public string GetInitScriptCspHash(ViewContext viewContext)
    {
        ArgumentNullException.ThrowIfNull(viewContext);

        var scriptPath = ResolveContentUrl(viewContext, JavascriptFileName);
        return GenerateCspHash(GetInitScriptContents(scriptPath));
    }

    internal string ResolveContentUrl(ViewContext viewContext, string path)
    {
        ArgumentNullException.ThrowIfNull(viewContext);
        ArgumentNullException.ThrowIfNull(path);

        path = "/" + path.TrimStart('/');

#if NET9_0_OR_GREATER
        if (ResourceCollectionUtilities.TryResolveFromAssetCollection(viewContext, path, out var resolvedUrl))
        {
            return resolvedUrl;
        }
#endif

        return GetVersionedUrl(viewContext.HttpContext.Request.PathBase + path);
    }

    private static IHtmlContent GenerateScriptImports(string scriptPath, string? cspNonce)
    {
        var htmlContentBuilder = new HtmlContentBuilder();
        htmlContentBuilder.AppendHtml(GenerateImportScript());
        htmlContentBuilder.AppendLine();
        htmlContentBuilder.AppendHtml(GenerateInitScript());
        htmlContentBuilder.AppendLine();
        return htmlContentBuilder;

        TagBuilder GenerateImportScript()
        {
            var tagBuilder = new TagBuilder("script");
            tagBuilder.MergeAttribute("type", "module");
            tagBuilder.MergeAttribute("src", scriptPath);
            return tagBuilder;
        }

        TagBuilder GenerateInitScript()
        {
            var tagBuilder = new TagBuilder("script");
            tagBuilder.MergeAttribute("type", "module");

            if (cspNonce is not null)
            {
                tagBuilder.MergeAttribute("nonce", cspNonce);
            }

            tagBuilder.InnerHtml.AppendHtml(new HtmlString(GetInitScriptContents(scriptPath)));

            return tagBuilder;
        }
    }

    private static IHtmlContent GenerateStyleImports(string stylesheetPath) =>
        new HtmlString($"<link href=\"{stylesheetPath}\" rel=\"stylesheet\">");

    private static string GetVersionedUrl(string path) =>
        QueryHelpers.AddQueryString(
            path,
            HostCompiledAssetsMiddleware.StaticAssetVersionQueryParamName,
            GovUkFrontendInfo.Version);

    private static string GetInitScriptContents(string scriptPath) =>
        $"\nimport {{ initAll }} from '{scriptPath}'\ninitAll()\n";

    private static string GenerateCspHash(string value)
    {
        using var algo = SHA256.Create();
#pragma warning disable CA1850
        var hash = algo.ComputeHash(Encoding.UTF8.GetBytes(value));
#pragma warning restore CA1850
        return $"'sha256-{Convert.ToBase64String(hash)}'";
    }

#if NET9_0_OR_GREATER
#nullable disable
    // https://github.com/dotnet/aspnetcore/blob/v9.0.0/src/Mvc/Mvc.TagHelpers/src/ResourceCollectionUtilities.cs
    internal static class ResourceCollectionUtilities
    {
        internal static bool TryResolveFromAssetCollection(ViewContext viewContext, string url, out string resolvedUrl)
        {
            var pathBase = viewContext.HttpContext.Request.PathBase;
            var assetCollection = viewContext.HttpContext.GetEndpoint()?.Metadata.GetMetadata<ResourceAssetCollection>();
            if (assetCollection != null)
            {
                var value = url.StartsWith('/') ? url[1..] : url;
                if (assetCollection.IsContentSpecificUrl(value))
                {
                    resolvedUrl = url;
                    return true;
                }

                var src = assetCollection[value];
                if (!string.Equals(src, value, StringComparison.Ordinal))
                {
                    resolvedUrl = url.StartsWith('/') ? $"/{src}" : src;
                    return true;
                }

                if (pathBase.HasValue && url.StartsWith(pathBase, StringComparison.OrdinalIgnoreCase))
                {
                    var length = pathBase.Value.EndsWith('/') ? pathBase.Value.Length : pathBase.Value.Length + 1;
                    var relativePath = url[length..];
                    if (assetCollection.IsContentSpecificUrl(relativePath))
                    {
                        resolvedUrl = url;
                        return true;
                    }

                    src = assetCollection[relativePath];
                    if (!string.Equals(src, relativePath, StringComparison.Ordinal))
                    {
                        if (pathBase.Value.EndsWith('/'))
                        {
                            resolvedUrl = $"{pathBase}{src}";
                            return true;
                        }
                        else
                        {
                            resolvedUrl = $"{pathBase}/{src}";
                            return true;
                        }
                    }
                }
            }

            resolvedUrl = null;
            return false;
        }
    }
#nullable enable
#endif
}
