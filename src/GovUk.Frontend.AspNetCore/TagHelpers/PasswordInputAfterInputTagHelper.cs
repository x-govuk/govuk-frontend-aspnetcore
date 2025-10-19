using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content after the input in a GDS password input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = PasswordInputTagHelper.TagName)]
#if SHORT_TAG_NAMES
[HtmlTargetElement(ShortTagName, ParentTag = PasswordInputTagHelper.TagName)]
#endif
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use after the generated <input> element.")]
public class PasswordInputAfterInputTagHelper : TagHelper
{
    private readonly ILogger<PasswordInputAfterInputTagHelper> _logger;

    internal const string TagName = "govuk-password-input-after-input";
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
    /// Creates a new <see cref="PasswordInputAfterInputTagHelper"/>.
    /// </summary>
    public PasswordInputAfterInputTagHelper(ILogger<PasswordInputAfterInputTagHelper> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var inputContext = context.GetContextItem<PasswordInputContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        if (output.Attributes.Count > 0)
        {
            _logger.AttributesAreNotSupportedOnTagNameAndWillBeIgnored(output.TagName);
        }

        inputContext.SetAfterInput(content.ToTemplateString(), output.TagName);

        output.SuppressOutput();
    }
}
