using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the heading in a message in a GDS cookie banner component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CookieBannerMessageTagHelper.TagName)]
public class CookieBannerMessageHeadingTagHelper : TagHelper
{
    internal const string TagName = "govuk-cookie-banner-message-heading";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var messageContext = context.GetContextItem<CookieBannerMessageContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        if (messageContext.Heading is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn([TagName], [CookieBannerMessageTagHelper.TagName]);
        }

        if (messageContext.Content is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(output.TagName, messageContext.Content.Value.TagName);
        }

        if (messageContext.Actions is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(output.TagName, CookieBannerMessageActionsTagHelper.TagName);
        }

        var attributes = new AttributeCollection(output.Attributes);

        messageContext.Heading = new(content.ToTemplateString(), output.TagName, attributes);

        output.SuppressOutput();
    }
}
