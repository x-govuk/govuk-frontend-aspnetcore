using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the actions in a message in a GDS cookie banner component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CookieBannerMessageTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = CookieBannerMessageTagHelper.ShortTagName)]
#pragma warning disable GFA0006 // Type or member is obsolete
[RestrictChildren(
    CookieBannerMessageActionButtonTagHelper.TagName,
    CookieBannerMessageActionButtonTagHelper.ShortTagName,
    CookieBannerMessageActionLinkTagHelper.TagName,
    CookieBannerMessageActionLinkTagHelper.ShortTagName,
    CookieBannerMessageActionTagHelper.TagName)]
#pragma warning restore GFA0006 // Type or member is obsolete
public class CookieBannerMessageActionsTagHelper : TagHelper
{
    internal const string TagName = "govuk-cookie-banner-message-actions";
    internal const string ShortTagName = ShortTagNames.MessageActions;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextItem(new CookieBannerMessageActionsContext(context.TagName));
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var messageContext = context.GetContextItem<CookieBannerMessageContext>();
        var actionsContext = context.GetContextItem<CookieBannerMessageActionsContext>();

        _ = await output.GetChildContentAsync();

        if (messageContext.Actions is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(AllTagNames, messageContext.TagName);
        }

        var attributes = new AttributeCollection(output.Attributes);

        actionsContext.Attributes = attributes;
        messageContext.Actions = actionsContext;

        output.SuppressOutput();
    }
}
