using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the title of a navigation section of a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterNavTagHelper.TagName)]
public class FooterNavTitleTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-nav-title";

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var navContext = context.GetContextItem<FooterNavContext>();

        if (navContext.Title is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(output.TagName, FooterNavTagHelper.TagName);
        }

        if (navContext.Items?.TagName is string itemsTagName)
        {
            throw ExceptionHelper.ChildElementMustBeSpecifiedBefore(output.TagName, itemsTagName);
        }

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);

        navContext.Title = (content.ToTemplateString(), attributes, output.TagName);

        output.SuppressOutput();
    }
}
