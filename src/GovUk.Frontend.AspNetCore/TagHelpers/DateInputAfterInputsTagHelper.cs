using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content after the inputs in a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputTagHelper.TagName)]
public class DateInputAfterInputsTagHelper : TagHelper
{
    private readonly ILogger<DateInputAfterInputsTagHelper> _logger;

    internal const string TagName = "govuk-date-input-after-inputs";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = new[]
    {
        TagName
    };

    /// <summary>
    /// Creates a new <see cref="DateInputAfterInputsTagHelper"/>.
    /// </summary>
    public DateInputAfterInputsTagHelper(ILogger<DateInputAfterInputsTagHelper> logger)
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
            _logger.AttributesAreNotSupportedOnTagNameAndWillBeIgnored(output.TagName);
        }

        dateInputContext.SetAfterInputs(content.ToTemplateString(), output.TagName);

        output.SuppressOutput();
    }
}
