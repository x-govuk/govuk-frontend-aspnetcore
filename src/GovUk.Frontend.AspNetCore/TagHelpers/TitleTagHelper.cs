using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.Localization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// <see cref="ITagHelper"/> that adds 'Error:' to the page's title if there are errors.
/// </summary>
/// <remarks>
/// Creates a new <see cref="TitleTagHelper"/>.
/// </remarks>
[HtmlTargetElement("title", ParentTag = "head")]
public class TitleTagHelper : TagHelper
{
    private const string DefaultErrorPrefix = "Error:";
    private const string ErrorPrefixAttributeName = "error-prefix";

    private readonly IOptions<GovUkFrontendOptions> _optionsAccessor;
    private readonly IGovUkFrontendLocalizer _localizer;

    /// <summary>
    /// Creates a new <see cref="TitleTagHelper"/>.
    /// </summary>
    public TitleTagHelper(IOptions<GovUkFrontendOptions> optionsAccessor, IGovUkFrontendLocalizer localizer)
    {
        ArgumentNullException.ThrowIfNull(optionsAccessor);
        ArgumentNullException.ThrowIfNull(localizer);

        _optionsAccessor = optionsAccessor;
        _localizer = localizer;
    }

    /// <summary>
    /// The prefix to add to the <c>title</c> when the page has errors.
    /// </summary>
    /// <remarks>
    ///  The default is <c>Error:</c>.
    /// </remarks>
    [HtmlAttributeName(ErrorPrefixAttributeName)]
    public string? ErrorPrefix { get; set; }

    /// <summary>
    /// Gets the <see cref="ViewContext"/> of the executing view.
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    [DisallowNull]
    public ViewContext? ViewContext { get; set; }

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (!_optionsAccessor.Value.PrependErrorToTitle)
        {
            return;
        }

        var pageErrorContext = ViewContext!.HttpContext.GetPageErrorContext();

        var errorPrefix = ErrorPrefix ??
            _localizer.GetString(GovUkFrontendResourceNames.TitleErrorPrefix) ??
            DefaultErrorPrefix;

        // The <title> is usually processed before the error summary is rendered - _GovUkPageTemplate
        // generates the summary in <main>, which comes after <head> - so whether there's a summary isn't known yet.
        // Razor buffers the page's output, so defer the decision until the content is written.
        output.PreContent.AppendHtml(new ErrorPrefixContent(pageErrorContext, errorPrefix + " "));
    }

    private sealed class ErrorPrefixContent(PageErrorContext pageErrorContext, string prefix) : IHtmlContent
    {
        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentNullException.ThrowIfNull(encoder);

            if (pageErrorContext.ErrorSummaryHasBeenRendered)
            {
                encoder.Encode(writer, prefix);
            }
        }
    }
}
