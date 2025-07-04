using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS back link component.
/// </summary>
[HtmlTargetElement(TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.BackLink)]
public class BackLinkTagHelper : TagHelper
{
    internal const string TagName = "govuk-back-link";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="BackLinkTagHelper"/>.
    /// </summary>
    public BackLinkTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        _componentGenerator = componentGenerator;
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        IHtmlContent? content = null;

        if (output.TagMode == TagMode.StartTagAndEndTag)
        {
            content = await output.GetChildContentAsync();
        }

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);
        attributes.Remove("href", out _);
        var href = output.GetUrlAttribute("href");

        var component = await _componentGenerator.GenerateBackLinkAsync(new BackLinkOptions()
        {
            Html = content.ToTemplateString(),
            Href = href,
            Classes = classes,
            Attributes = attributes
        });

        output.ApplyComponentHtml(component, HtmlEncoder.Default);
    }
}
