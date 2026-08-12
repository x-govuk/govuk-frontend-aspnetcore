using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content before the input in a GDS text input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TextInputTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = TextInputTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use before the generated input element.")]
public class TextInputBeforeInputTagHelper : TagHelper
{
    private readonly ILogger<TextInputBeforeInputTagHelper> _logger;

    internal const string TagName = "govuk-input-before-input";
    internal const string ShortTagName = ShortTagNames.BeforeInput;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <summary>
    /// Creates a new <see cref="TextInputBeforeInputTagHelper"/>.
    /// </summary>
    public TextInputBeforeInputTagHelper(ILogger<TextInputBeforeInputTagHelper> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var inputContext = context.GetContextItem<TextInputContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        if (output.Attributes.Count > 0)
        {
            _logger.AttributesAreNotSupportedOnTagNameAndWillBeIgnored(context.TagName);
        }

        inputContext.SetBeforeInput(content.Snapshot(), context.TagName);

        output.SuppressOutput();
    }
}
