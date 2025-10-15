using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content after the input in a GDS input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TextInputTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = TextInputTagHelper.TagName)]
public class TextInputAfterInputTagHelper : TagHelper
{
    private readonly ILogger<TextInputAfterInputTagHelper> _logger;

    internal const string TagName = "govuk-input-after-input";
    //internal const string ShortTagName = ShortTagNames.AfterInput;

    /// <summary>
    /// Creates a new <see cref="TextInputAfterInputTagHelper"/>.
    /// </summary>
    public TextInputAfterInputTagHelper(ILogger<TextInputAfterInputTagHelper> logger)
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

        var content = (await output.GetChildContentAsync()).Snapshot();

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
