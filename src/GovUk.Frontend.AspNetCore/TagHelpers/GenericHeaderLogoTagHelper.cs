using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the logo in a GDS generic header component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = GenericHeaderTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = GenericHeaderTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the logo's link.")]
public class GenericHeaderLogoTagHelper : TagHelper
{
    internal const string TagName = "govuk-generic-header-logo";
    internal const string ShortTagName = ShortTagNames.Logo;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    private const string LinkAttributesPrefix = "link-";

    /// <summary>
    /// Creates a new <see cref="GenericHeaderLogoTagHelper"/>.
    /// </summary>
    public GenericHeaderLogoTagHelper()
    {
    }

    /// <summary>
    /// Additional attributes to add to the generated homepage link element.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = LinkAttributesPrefix)]
    public IDictionary<string, string?> LinkAttributes { get; set; } = new Dictionary<string, string?>();

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var headerContext = context.GetContextItem<GenericHeaderContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        headerContext.SetLogo(
            content.Snapshot(),
            new AttributeCollection(output.Attributes),
            new AttributeCollection(LinkAttributes));

        output.SuppressOutput();
    }
}
