using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content after the input in a GDS select component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SelectTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = SelectTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use after the generated select element.")]
public class SelectAfterInputTagHelper : TagHelper
{
    private readonly ILogger<SelectAfterInputTagHelper> _logger;

    internal const string TagName = "govuk-select-after-input";
    internal const string ShortTagName = ShortTagNames.AfterInput;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <summary>
    /// Creates a new <see cref="SelectAfterInputTagHelper"/>.
    /// </summary>
    public SelectAfterInputTagHelper(ILogger<SelectAfterInputTagHelper> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var selectContext = context.GetContextItem<SelectContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        if (output.Attributes.Count > 0)
        {
            _logger.AttributesAreNotSupportedOnTagNameAndWillBeIgnored(context.TagName);
        }

        selectContext.SetAfterInput(content.Snapshot(), context.TagName);

        output.SuppressOutput();
    }
}
