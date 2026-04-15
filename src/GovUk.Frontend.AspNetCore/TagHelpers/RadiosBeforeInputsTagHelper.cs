using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content before the inputs in a GDS radios component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = RadiosTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = RadiosFieldsetTagHelper.TagName)]
public class RadiosBeforeInputsTagHelper : TagHelper
{
    private readonly ILogger<RadiosBeforeInputsTagHelper> _logger;

    internal const string TagName = "govuk-radios-before-inputs";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = new[]
    {
        TagName
    };

    /// <summary>
    /// Creates a new <see cref="RadiosBeforeInputsTagHelper"/>.
    /// </summary>
    public RadiosBeforeInputsTagHelper(ILogger<RadiosBeforeInputsTagHelper> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var radiosContext = context.GetContextItem<RadiosContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        if (output.Attributes.Count > 0)
        {
            _logger.AttributesAreNotSupportedOnTagNameAndWillBeIgnored(context.TagName);
        }

        radiosContext.SetBeforeInputs(content.ToTemplateString(), context.TagName);

        output.SuppressOutput();
    }
}
