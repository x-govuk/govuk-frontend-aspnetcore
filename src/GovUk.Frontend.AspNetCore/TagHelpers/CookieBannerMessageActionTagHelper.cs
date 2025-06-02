using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents an action in the message in a GDS cookie banner component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CookieBannerMessageActionsTagHelper.TagName, TagStructure = TagStructure.WithoutEndTag)]
[OutputElementHint("button")]
public class CookieBannerMessageActionTagHelper : TagHelper
{
    internal const string TagName = "govuk-cookie-banner-message-action";

    private const string NameAttributeName = "name";
    private const string TextAttributeName = "text";
    private const string TypeAttributeName = "type";
    private const string ValueAttributeName = "value";

    /// <summary>
    /// The <c>name</c> attribute for the generated <c>button</c> element.
    /// </summary>
    [HtmlAttributeName(NameAttributeName)]
    public string? Name { get; set; }

    /// <summary>
    /// The button text.
    /// </summary>
    /// <remarks>
    /// This attribute is required.
    /// </remarks>
    [HtmlAttributeName(TextAttributeName)]
    public string? Text { get; set; }

    /// <summary>
    /// The <c>type</c> attribute for the generated <c>button</c> element.
    /// </summary>
    [HtmlAttributeName(TypeAttributeName)]
    public string? Type { get; set; }

    /// <summary>
    /// The <c>value</c> attribute for the generated <c>button</c> element.
    /// </summary>
    [HtmlAttributeName(ValueAttributeName)]
    public string? Value { get; set; }

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var actionsContext = context.GetContextItem<CookieBannerMessageActionsContext>();

        if (Text is null)
        {
            throw ExceptionHelper.TheAttributeMustBeSpecified(TextAttributeName);
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        actionsContext.Actions.Add(new CookieBannerOptionsMessageAction()
        {
            Text = Text,
            Type = Type,
            Href = null,
            Name = Name,
            Value = Value,
            Classes = classes,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
