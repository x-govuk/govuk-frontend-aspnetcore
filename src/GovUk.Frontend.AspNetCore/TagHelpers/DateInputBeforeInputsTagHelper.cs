using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content before the inputs in a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use before the date input.")]
public class DateInputBeforeInputsTagHelper : TagHelper
{
    private readonly ILogger<DateInputBeforeInputsTagHelper> _logger;

    internal const string TagName = "govuk-date-input-before-inputs";
    internal const string ShortTagName = ShortTagNames.BeforeInputs;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <summary>
    /// Creates a new <see cref="DateInputBeforeInputsTagHelper"/>.
    /// </summary>
    public DateInputBeforeInputsTagHelper(ILogger<DateInputBeforeInputsTagHelper> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var dateInputContext = context.GetContextItem<DateInputContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        if (output.Attributes.Count > 0)
        {
            _logger.AttributesAreNotSupportedOnTagNameAndWillBeIgnored(context.TagName);
        }

        dateInputContext.SetBeforeInputs(content.Snapshot(), context.TagName);

        output.SuppressOutput();
    }
}
