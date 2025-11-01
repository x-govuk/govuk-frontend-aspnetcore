using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the actions in a message in a GDS cookie banner component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CookieBannerMessageTagHelper.TagName)]
[RestrictChildren(CookieBannerMessageActionTagHelper.TagName, CookieBannerMessageActionLinkTagHelper.TagName)]
public class CookieBannerMessageActionsTagHelper : TagHelper
{
    internal const string TagName = "govuk-cookie-banner-message-actions";

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new CookieBannerMessageActionsContext());
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
            throw ExceptionHelper.OnlyOneElementIsPermittedIn([TagName], [CookieBannerMessageTagHelper.TagName]);
        }

        var attributes = new AttributeCollection(output.Attributes);

        actionsContext.Attributes = attributes;
        messageContext.Actions = actionsContext;

        output.SuppressOutput();
    }
}
