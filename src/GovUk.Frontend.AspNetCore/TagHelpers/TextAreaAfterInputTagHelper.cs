using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content after the textarea in a GDS textarea component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TextAreaTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = TextAreaTagHelper.TagName)]
#endif
public class TextAreaAfterInputTagHelper : TagHelper
{
    private readonly ILogger<TextAreaAfterInputTagHelper> _logger;

    internal const string TagName = "govuk-textarea-after-input";
#if SHORT_TAG_NAMES
    internal const string ShortTagName = ShortTagNames.AfterInput;
#endif

    internal static IReadOnlyCollection<string> AllTagNames { get; } = new[]
    {
        TagName
#if SHORT_TAG_NAMES
        ,
        ShortTagName
#endif
    };

    /// <summary>
    /// Creates a new <see cref="TextAreaAfterInputTagHelper"/>.
    /// </summary>
    public TextAreaAfterInputTagHelper(ILogger<TextAreaAfterInputTagHelper> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var textAreaContext = context.GetContextItem<TextAreaContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        if (output.Attributes.Count > 0)
        {
            _logger.AttributesAreNotSupportedOnTagNameAndWillBeIgnored(output.TagName);
        }

        textAreaContext.SetAfterInput(content.ToTemplateString(), output.TagName);

        output.SuppressOutput();
    }
}
