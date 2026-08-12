using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content before the input in a GDS select component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SelectTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = SelectTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use before the generated select element.")]
public class SelectBeforeInputTagHelper : TagHelper
{
    private readonly ILogger<SelectBeforeInputTagHelper> _logger;

    internal const string TagName = "govuk-select-before-input";
    internal const string ShortTagName = ShortTagNames.BeforeInput;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <summary>
    /// Creates a new <see cref="SelectBeforeInputTagHelper"/>.
    /// </summary>
    public SelectBeforeInputTagHelper(ILogger<SelectBeforeInputTagHelper> logger)
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

        selectContext.SetBeforeInput(content.Snapshot(), context.TagName);

        output.SuppressOutput();
    }
}
