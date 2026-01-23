using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents an action in the message in a GDS cookie banner component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CookieBannerMessageActionsTagHelper.TagName, TagStructure = TagStructure.WithoutEndTag)]
public class CookieBannerMessageActionLinkTagHelper : TagHelper
{
    internal const string TagName = "govuk-cookie-banner-message-action-link";

    private const string TextAttributeName = "text";

    /// <summary>
    /// The link text.
    /// </summary>
    /// <remarks>
    /// This attribute is required.
    /// </remarks>
    [HtmlAttributeName(TextAttributeName)]
    public string? Text { get; set; }

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var actionsContext = context.GetContextItem<CookieBannerMessageActionsContext>();

        if (Text is null)
        {
            throw ExceptionHelper.TheAttributeMustBeSpecified(TextAttributeName);
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);
        var href = output.GetUrlAttribute("href");
        attributes.Remove("href", out _);

        actionsContext.Actions.Add(new CookieBannerOptionsMessageAction
        {
            Text = Text,
            Type = null,
            Href = href,
            Name = null,
            Value = null,
            Classes = classes,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
