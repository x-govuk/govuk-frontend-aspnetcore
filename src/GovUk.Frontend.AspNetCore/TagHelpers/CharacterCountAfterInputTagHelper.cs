using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content after the input in a GDS character count component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CharacterCountTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = CharacterCountTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use after the generated textarea element.")]
public class CharacterCountAfterInputTagHelper : TagHelper
{
    private readonly ILogger<CharacterCountAfterInputTagHelper> _logger;

    internal const string TagName = "govuk-character-count-after-input";
    internal const string ShortTagName = ShortTagNames.AfterInput;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <summary>
    /// Creates a new <see cref="CharacterCountAfterInputTagHelper"/>.
    /// </summary>
    public CharacterCountAfterInputTagHelper(ILogger<CharacterCountAfterInputTagHelper> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var characterCountContext = context.GetContextItem<CharacterCountContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        if (output.Attributes.Count > 0)
        {
            _logger.AttributesAreNotSupportedOnTagNameAndWillBeIgnored(context.TagName);
        }

        characterCountContext.SetAfterInput(content.Snapshot(), context.TagName);

        output.SuppressOutput();
    }
}
