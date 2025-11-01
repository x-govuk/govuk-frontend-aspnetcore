using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the items in a navigation section of a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterNavTagHelper.TagName)]
[RestrictChildren(FooterNavItemTagHelper.TagName)]
public class FooterNavItemsTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-nav-items";

    /// <inheritdoc />
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new FooterNavItemsContext());
    }

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var navContext = context.GetContextItem<FooterNavContext>();
        var itemsContext = context.GetContextItem<FooterNavItemsContext>();

        if (navContext.Items is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(output.TagName, FooterNavTagHelper.TagName);
        }

        _ = await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);

        navContext.Items = (itemsContext.Items, attributes, output.TagName);

        output.SuppressOutput();
    }
}
