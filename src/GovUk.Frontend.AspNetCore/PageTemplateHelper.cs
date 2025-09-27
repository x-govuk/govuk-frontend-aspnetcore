using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Contains methods for generating script and stylesheet imports for the GDS page template.
/// </summary>
public class PageTemplateHelper
{
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
        var htmlContentBuilder = new HtmlContentBuilder();
        htmlContentBuilder.AppendHtml(GenerateImportScript());
        htmlContentBuilder.AppendLine();
        htmlContentBuilder.AppendHtml(GenerateInitScript());
        htmlContentBuilder.AppendLine();
        return htmlContentBuilder;

        TagBuilder GenerateImportScript()
        {
            var src = $"{pathBase}/{GetScriptFileName()}";

            var tagBuilder = new TagBuilder("script");
            tagBuilder.MergeAttribute("type", "module");
            tagBuilder.MergeAttribute("src", src);
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

            tagBuilder.InnerHtml.AppendHtml(new HtmlString(GetInitScriptContents(pathBase)));

            return tagBuilder;
        }
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
        var fileName = GetStylesheetFileName();
        return new HtmlString($"<link href=\"{pathBase}/{fileName}\" rel=\"stylesheet\">");
    }

    /// <summary>
    /// Gets all the CSP hashes for the inline scripts used in the page template.
    /// </summary>
    /// <returns>A list of hashes to be included in your site's <c>Content-Security-Policy</c> header within the <c>script-src</c> directive.</returns>
    public string GetCspScriptHashes() => GetCspScriptHashes(pathBase: "");

    /// <summary>
    /// Gets all the CSP hashes for the inline scripts used in the page template.
    /// </summary>
    /// <param name="pathBase">The base path that the script is hosted under.</param>
    /// <returns>A list of hashes to be included in your site's <c>Content-Security-Policy</c> header within the <c>script-src</c> directive.</returns>
    public string GetCspScriptHashes(PathString pathBase) => $"{GetInitScriptCspHash(pathBase)} {GetJsEnabledScriptCspHash()}";

    /// <summary>
    /// Gets the CSP hash for the script that adds a <c>js-enabled</c> CSS class.
    /// </summary>
    /// <returns>A hash to be included in your site's <c>Content-Security-Policy</c> header within the <c>script-src</c> directive.</returns>
    public string GetJsEnabledScriptCspHash() => GenerateCspHash(JsEnabledScript);

    /// <summary>
    /// Gets the CSP hash for the GOV.UK Frontend initialization script.
    /// </summary>
    /// <returns>A hash to be included in your site's <c>Content-Security-Policy</c> header within the <c>script-src</c> directive.</returns>
    public string GetInitScriptCspHash() => GetInitScriptCspHash(pathBase: "");

    /// <summary>
    /// Gets the CSP hash for the GOV.UK Frontend initialization script.
    /// </summary>
    /// <param name="pathBase">The base path that the script is hosted under.</param>
    /// <returns>A hash to be included in your site's <c>Content-Security-Policy</c> header within the <c>script-src</c> directive.</returns>
    public string GetInitScriptCspHash(PathString pathBase) =>
        GenerateCspHash(GetInitScriptContents(pathBase));

    private string GetInitScriptContents(PathString pathBase)
    {
        var fileName = GetScriptFileName();
        return $"\nimport {{ initAll }} from '{pathBase}/{fileName}'\ninitAll()\n";
    }

    private static string GenerateCspHash(string value)
    {
        using var algo = SHA256.Create();
#pragma warning disable CA1850
        var hash = algo.ComputeHash(Encoding.UTF8.GetBytes(value));
#pragma warning restore CA1850
        return $"'sha256-{Convert.ToBase64String(hash)}'";
    }

    private static string GetScriptFileName() =>
        $"govuk-frontend.min.js?{HostCompiledAssetsMiddleware.StaticAssetVersionQueryParamName}={GovUkFrontendVersion}";

    private static string GetStylesheetFileName() =>
        $"govuk-frontend.min.css?{HostCompiledAssetsMiddleware.StaticAssetVersionQueryParamName}={GovUkFrontendVersion}";
}
