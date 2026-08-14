using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the items in a navigation section of a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterNavTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = FooterNavTagHelper.ShortTagName)]
[RestrictChildren(FooterNavItemTagHelper.TagName, FooterNavItemTagHelper.ShortTagName)]
public class FooterNavItemsTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-nav-items";
    internal const string ShortTagName = ShortTagNames.NavItems;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

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
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(AllTagNames, navContext.TagName!);
        }

        _ = await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);

        navContext.Items = (itemsContext.Items, attributes, context.TagName);

        output.SuppressOutput();
    }
}
