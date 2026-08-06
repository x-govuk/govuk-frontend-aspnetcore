using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS generic header component.
/// </summary>
[HtmlTargetElement(TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.GenericHeader)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use after the logo.")]
public class GenericHeaderTagHelper : TagHelper
{
    internal const string TagName = "govuk-generic-header";

    private const string ContainerAttributesPrefix = "container-";
    private const string HomePageUrlAttributeName = "home-page-url";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="GenericHeaderTagHelper"/>.
    /// </summary>
    public GenericHeaderTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);

        _componentGenerator = componentGenerator;
    }

    /// <summary>
    /// The URL of the homepage link.
    /// </summary>
    /// <remarks>
    /// If not specified, <c>/</c> will be used.
    /// </remarks>
    [HtmlAttributeName(HomePageUrlAttributeName)]
#pragma warning disable CA1056
    public string? HomePageUrl { get; set; }
#pragma warning restore CA1056

    /// <summary>
    /// Additional attributes to add to the generated container element.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = ContainerAttributesPrefix)]
    public IDictionary<string, string?> ContainerAttributes { get; set; } = new Dictionary<string, string?>();

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextItem(new GenericHeaderContext());
    }

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

        headerContext.ThrowIfNotComplete();
        var logo = headerContext.Logo!.Value;

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var containerAttributes = new AttributeCollection(ContainerAttributes);
        containerAttributes.Remove("class", out var containerClasses);

        var component = await _componentGenerator.GenerateGenericHeaderAsync(new GenericHeaderOptions
        {
            Url = HomePageUrl,
            LogoHtml = logo.Content,
            LogoAttributes = logo.Attributes,
            LinkAttributes = logo.LinkAttributes,
            ContainerClasses = containerClasses,
            ContainerAttributes = containerAttributes,
            Classes = classes,
            Attributes = attributes,
            Html = content.Snapshot()
        });

        component.ApplyToTagHelper(output);
    }
}
