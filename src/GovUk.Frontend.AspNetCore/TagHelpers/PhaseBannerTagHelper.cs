using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS phase banner component.
/// </summary>
[HtmlTargetElement(TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.PhaseBanner)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use in the phase banner.")]
public class PhaseBannerTagHelper : TagHelper
{
    internal const string TagName = "govuk-phase-banner";

    private readonly IComponentGenerator _componentGenerator;
    private readonly HtmlEncoder _encoder;

    /// <summary>
    /// Creates a <see cref="PhaseBannerTagHelper"/>.
    /// </summary>
    public PhaseBannerTagHelper(IComponentGenerator componentGenerator, HtmlEncoder encoder)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        ArgumentNullException.ThrowIfNull(encoder);

        _componentGenerator = componentGenerator;
        _encoder = encoder;
    }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextItem(new PhaseBannerContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var phaseBannerContext = context.GetContextItem<PhaseBannerContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        phaseBannerContext.ThrowIfIncomplete();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GeneratePhaseBannerAsync(new PhaseBannerOptions()
        {
            Text = null,
            Html = content.ToTemplateString(),
            Tag = phaseBannerContext.Tag?.Options,
            Classes = classes,
            Attributes = attributes
        });

        output.ApplyComponentHtml(component, _encoder);
    }
}
