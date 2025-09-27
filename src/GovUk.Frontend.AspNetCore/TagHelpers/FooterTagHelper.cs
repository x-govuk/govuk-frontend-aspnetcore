using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(
    FooterNavTagHelper.TagName,
    FooterMetaTagHelper.TagName,
    FooterContentLicenceTagHelper.TagName,
    FooterCopyrightTagHelper.TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.Footer)]
public class FooterTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer";

    private const string ContainerClassAttributeName = "container-class";

    private readonly IComponentGenerator _componentGenerator;
    private readonly IOptions<GovUkFrontendOptions> _optionsAccessor;
    private readonly HtmlEncoder _encoder;

    /// <summary>
    /// Creates a new <see cref="FooterTagHelper"/>.
    /// </summary>
    public FooterTagHelper(
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
    /// Classes to add to the inner container.
    /// </summary>
    [HtmlAttributeName(ContainerClassAttributeName)]
    public string? ContainerClass { get; set; }

    /// <inheritdoc />
    public override void Init(TagHelperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextItem(new FooterContext());
    }

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var footerContext = context.GetContextItem<FooterContext>();

        await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GenerateFooterAsync(new FooterOptions()
        {
            Meta = footerContext.Meta?.Options,
            Navigation = footerContext.Navigation,
            ContentLicence = footerContext.ContentLicence?.Options,
            Copyright = footerContext.Copyright?.Options,
            ContainerClasses = ContainerClass,
            Classes = classes,
            Attributes = attributes,
            Rebrand = _optionsAccessor.Value.Rebrand
        });

        output.ApplyComponentHtml(component, _encoder);
    }
}
