using System.Diagnostics.CodeAnalysis;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using static GovUk.Frontend.AspNetCore.ErrorSummaryGenerationOptions;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// <see cref="ITagHelper"/> implementation targeting &lt;form&gt; elements and elements with a 'prepend-error-summary' attribute.
/// </summary>
[HtmlTargetElement("form")]
[HtmlTargetElement("main")]
public class GeneratedErrorSummaryTagHelper : TagHelper
{
    private const string PrependErrorSummaryAttributeName = "prepend-error-summary";

    private readonly IComponentGenerator _componentGenerator;
    private readonly IOptions<GovUkFrontendOptions> _optionsAccessor;
    private readonly IGovUkFrontendLocalizer _localizer;

    /// <summary>
    /// Creates a <see cref="GeneratedErrorSummaryTagHelper"/>.
    /// </summary>
    public GeneratedErrorSummaryTagHelper(
        IComponentGenerator componentGenerator,
        IOptions<GovUkFrontendOptions> optionsAccessor,
        IGovUkFrontendLocalizer localizer)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        ArgumentNullException.ThrowIfNull(optionsAccessor);
        ArgumentNullException.ThrowIfNull(localizer);

        _componentGenerator = componentGenerator;
        _optionsAccessor = optionsAccessor;
        _localizer = localizer;
    }

    /// <summary>
    /// Whether to prepend an error summary component to this form.
    /// </summary>
    /// <remarks>
    /// The default is set for the application in <see cref="GovUkFrontendOptions.ErrorSummaryGeneration"/>.
    /// </remarks>
    [HtmlAttributeName(PrependErrorSummaryAttributeName)]
    public bool? PrependErrorSummary { get; set; }

    /// <summary>
    /// Gets the <see cref="ViewContext"/> of the executing view.
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    [DisallowNull]
    public ViewContext? ViewContext { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        await output.GetChildContentAsync();

        var generateErrorSummariesOptions = _optionsAccessor.Value.ErrorSummaryGeneration;

        var isMainElement = context.TagName?.Equals("main", StringComparison.OrdinalIgnoreCase) is true;

        var prependErrorSummary = PrependErrorSummary ??
            ((context.TagName?.Equals("form", StringComparison.OrdinalIgnoreCase) is true && generateErrorSummariesOptions.HasFlag(PrependToFormElements)) ||
            (isMainElement && generateErrorSummariesOptions.HasFlag(PrependToMainElement)));

        if (!prependErrorSummary)
        {
            return;
        }

        var pageErrorContext = ViewContext!.HttpContext.GetPageErrorContext();

        // The main element wraps everything on the page, so if a summary has already been rendered
        // - by a form inside it, say - a second one here would be a duplicate.
        if (isMainElement && pageErrorContext.ErrorSummaryHasBeenRendered)
        {
            return;
        }

        var errorSummaryItems = pageErrorContext.GetErrorSummaryItems();

        if (errorSummaryItems.Count == 0)
        {
            return;
        }

        var disableAutoFocus = generateErrorSummariesOptions.HasFlag(DisableAutoFocus);

        var (titleText, titleHtml) = DefaultComponentGenerator.GetErrorSummaryTitle(_localizer, specifiedTitleHtml: null);

        var errorSummary = await _componentGenerator.GenerateErrorSummaryAsync(new ErrorSummaryOptions
        {
            TitleText = titleText,
            TitleHtml = titleHtml,
            DescriptionText = null,
            DescriptionHtml = null,
            ErrorList = errorSummaryItems,
            Classes = null,
            Attributes = null,
            DisableAutoFocus = disableAutoFocus,
            TitleAttributes = null,
            DescriptionAttributes = null
        });

        output.PreContent.AppendHtml(errorSummary.GetContent());

        pageErrorContext.ErrorSummaryHasBeenRendered = true;
    }
}
