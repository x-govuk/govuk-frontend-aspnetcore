using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a message in a GDS cookie banner component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CookieBannerTagHelper.TagName)]
[RestrictChildren(
    CookieBannerMessageHeadingTagHelper.TagName,
    CookieBannerMessageContentTagHelper.TagName,
    CookieBannerMessageActionsTagHelper.TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.CookieBannerMessage)]
public class CookieBannerMessageTagHelper : TagHelper
{
    internal const string TagName = "govuk-cookie-banner-message";

    private const string HiddenAttributeName = "hidden";
    private const string RoleAttributeName = "role";

    /// <summary>
    /// Whether the message should be hidden.
    /// </summary>
    /// <remarks>
    /// If not specified, <see langword="false" /> is used.
    /// </remarks>
    [HtmlAttributeName(HiddenAttributeName)]
    public bool? Hidden { get; set; }

    /// <summary>
    /// The <c>role</c> attribute for the message.
    /// </summary>
    /// <remarks>
    /// Set role to <c>alert</c> on confirmation messages to allow assistive tech to automatically read the message.
    /// You will also need to move focus to the confirmation message using JavaScript you have written yourself.
    /// </remarks>
    [HtmlAttributeName(RoleAttributeName)]
    public string? Role { get; set; }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new CookieBannerMessageContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var cookieBannerContext = context.GetContextItem<CookieBannerContext>();
        var messageContext = context.GetContextItem<CookieBannerMessageContext>();

        await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        cookieBannerContext.Messages.Add(new CookieBannerOptionsMessage()
        {
            HeadingText = null,
            HeadingHtml = messageContext.Heading?.Html,
            HeadingAttributes = messageContext.Heading?.Attributes,
            Text = null,
            Html = messageContext.Content?.Html,
            ContentAttributes = messageContext.Content?.Attributes,
            Actions = messageContext.Actions?.Actions,
            ActionsAttributes = messageContext.Actions?.Attributes,
            Hidden = Hidden,
            Role = Role,
            Classes = classes,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
