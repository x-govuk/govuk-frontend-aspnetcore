using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content in a message in a GDS cookie banner component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CookieBannerMessageTagHelper.TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.CookieBannerMessageContent)]
public class CookieBannerMessageContentTagHelper : TagHelper
{
    internal const string TagName = "govuk-cookie-banner-message-content";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var messageContext = context.GetContextItem<CookieBannerMessageContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        if (messageContext.Content is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn([TagName], [CookieBannerMessageTagHelper.TagName]);
        }

        if (messageContext.Actions is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(output.TagName, CookieBannerMessageActionsTagHelper.TagName);
        }

        var attributes = new AttributeCollection(output.Attributes);

        messageContext.Content = new(content.ToTemplateString(), output.TagName, attributes);

        output.SuppressOutput();
    }
}
