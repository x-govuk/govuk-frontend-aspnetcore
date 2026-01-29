using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content before the inputs in a GDS checkboxes component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesTagHelper.TagName)]
public class CheckboxesBeforeInputsTagHelper : TagHelper
{
    private readonly ILogger<CheckboxesBeforeInputsTagHelper> _logger;

    internal const string TagName = "govuk-checkboxes-before-inputs";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = new[]
    {
        TagName
    };

    /// <summary>
    /// Creates a new <see cref="CheckboxesBeforeInputsTagHelper"/>.
    /// </summary>
    public CheckboxesBeforeInputsTagHelper(ILogger<CheckboxesBeforeInputsTagHelper> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var checkboxesContext = context.GetContextItem<CheckboxesContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        if (output.Attributes.Count > 0)
        {
            _logger.AttributesAreNotSupportedOnTagNameAndWillBeIgnored(output.TagName);
        }

        checkboxesContext.SetBeforeInputs(content.ToTemplateString(), output.TagName);

        output.SuppressOutput();
    }
}
