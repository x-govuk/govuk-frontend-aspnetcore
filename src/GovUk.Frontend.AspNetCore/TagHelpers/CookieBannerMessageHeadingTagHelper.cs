using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the heading in a message in a GDS cookie banner component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CookieBannerMessageTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = CookieBannerMessageTagHelper.ShortTagName)]
public class CookieBannerMessageHeadingTagHelper : TagHelper
{
    internal const string TagName = "govuk-cookie-banner-message-heading";
    internal const string ShortTagName = ShortTagNames.Heading;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

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
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(AllTagNames, messageContext.TagName);
        }

        if (messageContext.Content is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(context.TagName, messageContext.Content.Value.TagName);
        }

        if (messageContext.Actions is not null)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(context.TagName, messageContext.Actions.TagName);
        }

        var attributes = new AttributeCollection(output.Attributes);

        messageContext.Heading = new(content.Snapshot(), context.TagName, attributes);

        output.SuppressOutput();
    }
}
