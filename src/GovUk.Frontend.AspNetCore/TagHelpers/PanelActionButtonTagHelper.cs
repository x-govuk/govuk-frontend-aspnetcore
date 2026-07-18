using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a button action in a GDS panel component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = PanelActionsTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = PanelActionsTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the button.")]
public class PanelActionButtonTagHelper : TagHelper
{
    internal const string TagName = "govuk-panel-action-button";
    internal const string ShortTagName = ShortTagNames.ActionButton;

    private const string TypeAttributeName = "type";

    /// <summary>
    /// The <c>type</c> attribute for the generated <c>button</c> element.
    /// </summary>
    /// <remarks>
    /// The default is <c>button</c>.
    /// </remarks>
    [HtmlAttributeName(TypeAttributeName)]
    public string? Type { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var actionsContext = context.GetContextItem<PanelActionsContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);
        attributes.Remove("formaction", out _);

        if (output.Attributes.ContainsName("formaction"))
        {
            attributes.Set("formaction", output.GetUrlAttribute("formaction")!);
        }

        actionsContext.Actions.Add(new PanelActionsItemOptions
        {
            Html = content.ToTemplateString(),
            Type = Type ?? "button",
            Href = null,
            Classes = classes,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
