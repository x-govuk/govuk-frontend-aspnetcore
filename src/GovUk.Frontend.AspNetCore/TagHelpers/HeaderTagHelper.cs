using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS header component.
/// </summary>
[HtmlTargetElement(TagName, TagStructure = TagStructure.WithoutEndTag)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.Header)]
public class HeaderTagHelper : TagHelper
{
    internal const string TagName = "govuk-header";

    private const string ContainerAttributesPrefix = "container-";
    private const string HomePageUrlAttributeName = "home-page-url";
    private const string ProductNameAttributeName = "product-name";

    private readonly IComponentGenerator _componentGenerator;
    private readonly IOptions<GovUkFrontendOptions> _optionsAccessor;
    private readonly HtmlEncoder _encoder;

    /// <summary>
    /// Creates a new <see cref="HeaderTagHelper"/>.
    /// </summary>
    public HeaderTagHelper(
        IComponentGenerator componentGenerator,
        IOptions<GovUkFrontendOptions> optionsAccessor,
        HtmlEncoder encoder)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        ArgumentNullException.ThrowIfNull(optionsAccessor);
        ArgumentNullException.ThrowIfNull(encoder);

        _componentGenerator = componentGenerator;
        _optionsAccessor = optionsAccessor;
        _encoder = encoder;
    }

    /// <summary>
    /// The URL of the homepage.
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

    /// <summary>
    /// Product name, used when the product name follows on directly from &quot;GOV.UK&quot;. For example, GOV.UK Pay or GOV.UK Design System.
    /// </summary>
    [HtmlAttributeName(ProductNameAttributeName)]
    public string? ProductName { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        _ = await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var containerAttributes = new AttributeCollection(ContainerAttributes);
        containerAttributes.Remove("class", out var containerClasses);

        var component = await _componentGenerator.GenerateHeaderAsync(new HeaderOptions()
        {
            HomePageUrl = HomePageUrl,
            ProductName = ProductName,
            ServiceName = null,
            ServiceUrl = null,
            Navigation = null,
            NavigationAttributes = null,
            NavigationClasses = null,
            NavigationLabel = null,
            MenuButtonLabel = null,
            MenuButtonText = null,
            ContainerClasses = containerClasses,
            ContainerAttributes = containerAttributes,
            Classes = classes,
            Attributes = attributes,
            UseTudorCrown = true,
            Rebrand = _optionsAccessor.Value.Rebrand
        });

        output.ApplyComponentHtml(component, _encoder);
    }
}
