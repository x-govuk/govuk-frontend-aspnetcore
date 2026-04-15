using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content before the input in a GDS character count component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CharacterCountTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = CharacterCountTagHelper.TagName)]
#endif
public class CharacterCountBeforeInputTagHelper : TagHelper
{
    private readonly ILogger<CharacterCountBeforeInputTagHelper> _logger;

    internal const string TagName = "govuk-character-count-before-input";
#if SHORT_TAG_NAMES
    internal const string ShortTagName = ShortTagNames.BeforeInput;
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
    /// Creates a new <see cref="CharacterCountBeforeInputTagHelper"/>.
    /// </summary>
    public CharacterCountBeforeInputTagHelper(ILogger<CharacterCountBeforeInputTagHelper> logger)
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

        characterCountContext.SetBeforeInput(content.ToTemplateString(), context.TagName);

        output.SuppressOutput();
    }
}
